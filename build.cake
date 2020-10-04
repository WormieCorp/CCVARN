#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0
#addin nuget:?package=Cake.Codecov&version=0.9.1
#addin nuget:?package=Cake.Json&version=5.2.0
#addin nuget:?package=Newtonsoft.Json&version=12.0.3
#tool dotnet:https://f.feedz.io/wormiecorp/packages/nuget/index.json?package=dotnet-ccvarn&version=1.0.0-alpha*&prerelease
#tool dotnet:?package=GitReleaseManager.Tool&version=0.11.0
using System;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var artifactsDir = Argument<DirectoryPath>("artifacts", "./.artifacts");
var solution = "./CCVARN.sln";
var dotnetExec = Context.Tools.Resolve("dotnet") ?? Context.Tools.Resolve("dotnet.exe");
var plainTextReleaseNotes = artifactsDir.CombineWithFilePath("release-notes.txt");
var markdownReleaseNotes = artifactsDir.CombineWithFilePath("release-notes.md");

public class BuildData
{
	public BuildVersion Version { get; set; }
}

public class BuildVersion
{
	public string MajorMinorPatch { get; set; }
	public string SemVer { get; set; }
	public string FullSemVer { get; set; }
	public string PreReleaseTag { get; set; }
	public string Metadata { get; set; }
}

Setup((context) =>
{
	var exec = context.Tools.Resolve("dotnet-ccvarn") ?? context.Tools.Resolve("dotnet-ccvarn.exe");
	var outputPath = artifactsDir.CombineWithFilePath("data.json");

	// Temporary fix
	if (!DirectoryExists(artifactsDir))
		CreateDirectory(artifactsDir);

	var exitCode = StartProcess(exec, new ProcessSettings
	{
		Arguments = new ProcessArgumentBuilder()
			.Append("parse")
			.AppendQuoted(outputPath.ToString())
			.AppendSwitchQuoted("--output", " ", plainTextReleaseNotes.ToString())
			.AppendSwitchQuoted("--output", " ", markdownReleaseNotes.ToString()),
	});

	var buildData = DeserializeJsonFromFile<BuildData>(outputPath);

	context.Information("Building CCVARN v{0}", buildData.Version.FullSemVer);

	return buildData.Version;
});

Task("Clean")
	.Does(() =>
{
	var dirs = new[] {
		artifactsDir.Combine("packages"),
		artifactsDir.Combine("coverage"),
	};
	CleanDirectories(dirs);
});

Task("Build")
	.IsDependentOn("Clean")
	.Does<BuildVersion>((version) =>
{
	var plainTextNotes = System.IO.File.ReadAllText(plainTextReleaseNotes.ToString(), System.Text.Encoding.UTF8);
	DotNetCoreBuild(solution, new DotNetCoreBuildSettings
	{
		Configuration = configuration,
		NoIncremental = HasArgument("no-incremental"),
		MSBuildSettings = new DotNetCoreMSBuildSettings()
			.SetVersionPrefix(version.MajorMinorPatch)
			.WithProperty("PackageReleaseNotes", plainTextNotes),
		VersionSuffix = version.PreReleaseTag + "+" + version.Metadata,
	});
});

Task("Test")
	.IsDependentOn("Build")
	.Does(() =>
{
	DotNetCoreTest(solution, new DotNetCoreTestSettings
	{
		ArgumentCustomization = (args) =>
			args.AppendSwitchQuoted("--collect", ":", "XPlat Code Coverage")
			.Append("--")
			.AppendSwitchQuoted("DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format", "=", "opencover,cobertura")
			.AppendSwitchQuoted("DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.SkipAutoProps","=", "true")
			.AppendSwitchQuoted("DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.UseSourceLink","=", "true")
			.AppendSwitchQuoted("DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Exclude","=", "[*]DryIoc.*,[*]FastExpressionCompiler.*,[*]ImTools.*"),
		Configuration = configuration,
		NoBuild = true,
		ResultsDirectory = artifactsDir.Combine("coverage/tests"),
	});
});

Task("Pack")
	.IsDependentOn("Test")
	.Does<BuildVersion>((version) =>
{
	var plainTextNotes = System.IO.File.ReadAllText(plainTextReleaseNotes.ToString(), System.Text.Encoding.UTF8);
	DotNetCorePack(solution, new DotNetCorePackSettings
	{
		Configuration = configuration,
		NoBuild = true,
		OutputDirectory = artifactsDir.Combine("packages"),
		MSBuildSettings = new DotNetCoreMSBuildSettings()
			.SetVersionPrefix(version.MajorMinorPatch)
			.WithProperty("PackageReleaseNotes", plainTextNotes),
		VersionSuffix = version.PreReleaseTag + "+" + version.Metadata,
	});
});

Task("Generate-LocalReport")
	.WithCriteria(() => BuildSystem.IsLocalBuild)
	.IsDependentOn("Test")
	.Does(() =>
{
	var files = GetFiles(artifactsDir + "/coverage/tests/**/*.xml");
	ReportGenerator(files, artifactsDir.Combine("coverage"), new ReportGeneratorSettings
	{
		ArgumentCustomization = args => args.Prepend("reportgenerator"),
		ToolPath = dotnetExec,
	});
});

Task("Upload-CoverageToCodecov")
	.WithCriteria(() => !BuildSystem.IsLocalBuild)
	.IsDependentOn("Test")
	.Does(() =>
{
	Codecov(new CodecovSettings
	{
		ArgumentCustomization = args => args.Prepend("codecov"),
		Files = new[]{ artifactsDir + "/coverage/tests/**/*.xml", },
		ToolPath = dotnetExec,
	});
});

Task("Push-NuGetPackages")
	.WithCriteria((context) => context.Environment.Platform.Family == PlatformFamily.Linux)
	.WithCriteria(() => HasEnvironmentVariable("NUGET_SOURCE"))
	.WithCriteria(() => HasEnvironmentVariable("NUGET_API_KEY"))
	.IsDependentOn("Pack")
	.Does(() =>
{
	var settings = new DotNetCoreNuGetPushSettings
	{
		ApiKey = EnvironmentVariable("NUGET_API_KEY"),
		SkipDuplicate = true,
		Source = EnvironmentVariable("NUGET_SOURCE"),
	};

	DotNetCoreNuGetPush(artifactsDir + "/packages/*.nupkg", settings);
});

Task("Publish-Release")
	.WithCriteria(() => HasEnvironmentVariable("GITHUB_TOKEN"))
	.WithCriteria(() => !String.IsNullOrEmpty(EnvironmentVariable("GITHUB_TOKEN")))
	.Does<BuildVersion>((version) =>
{
	var token = EnvironmentVariable("GITHUB_TOKEN");
	const string organization = "WormieCorp";
	const string repository = "CCVARN";
	GitReleaseManagerCreate(token, organization, repository, new GitReleaseManagerCreateSettings
	{
		Name            = version.SemVer,
		Milestone       = version.MajorMinorPatch,
		InputFilePath   = markdownReleaseNotes,
		TargetCommitish = "master",
		Prerelease      = version.MajorMinorPatch != version.SemVer,
	});

	if (version.MajorMinorPatch == version.SemVer)
		GitReleaseManagerClose(token, organization, repository, version.MajorMinorPatch);

	GitReleaseManagerPublish(token, organization, repository, version.SemVer);
});

Task("Default")
	.IsDependentOn("Pack")
	.IsDependentOn("Generate-LocalReport")
	.IsDependentOn("Upload-CoverageToCodecov")
	.IsDependentOn("Push-NuGetPackages");

RunTarget(target);
