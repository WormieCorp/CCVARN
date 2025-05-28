#addin nuget:?package=Cake.Codecov&version=1.1.0
#addin nuget:?package=Cake.Json&version=7.0.1
#addin nuget:?package=Newtonsoft.Json&version=13.0.1
#tool nuget:?package=CodecovUploader&version=0.2.3
#tool dotnet:?package=gitreleasemanager.tool&version=0.13.0

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
	var outputPath = artifactsDir.CombineWithFilePath("data.json");

	// Temporary fix
	if (!DirectoryExists(artifactsDir))
		CreateDirectory(artifactsDir);

	var exitCode = StartProcess("dotnet", new ProcessSettings
	{
		Arguments = new ProcessArgumentBuilder()
			.Append("ccvarn")
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
			.SetVersion(version.FullSemVer)
			.WithProperty("PackageReleaseNotes", plainTextNotes),
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
			.SetVersion(version.FullSemVer)
			.WithProperty("PackageReleaseNotes", plainTextNotes),
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

const string organization = "WormieCorp";
const string repository = "CCVARN";
var token = EnvironmentVariable("GITHUB_TOKEN");

Task("Create-StableTag")
	.Does<BuildVersion>((version) =>
{
	var plainTextNotes = System.IO.File.ReadAllLines(plainTextReleaseNotes.ToString(), System.Text.Encoding.UTF8).Skip(2);

        StartProcess("git", new ProcessSettings().WithArguments(args =>
                args.Append("tag")
                        .Append(version.MajorMinorPatch)
                        .Append("-a")
                        .AppendSwitchQuoted("-m", string.Join("\n", plainTextNotes).Replace("\"", string.Empty).Trim())
        ));
});

Task("Draft-ReleaseNotes")
	.WithCriteria(() => HasEnvironmentVariable("GITHUB_TOKEN"))
	.WithCriteria(() => !string.IsNullOrEmpty(EnvironmentVariable("GITHUB_TOKEN")))
	.Does<BuildVersion>((version) =>
{
	GitReleaseManagerCreate(token, organization, repository, new GitReleaseManagerCreateSettings
	{
		Name            = version.SemVer,
		InputFilePath   = markdownReleaseNotes,
		TargetCommitish = "master",
		Prerelease      = version.MajorMinorPatch != version.SemVer,
	});
});

Task("Publish-Release")
	.IsDependentOn("Default")
	.WithCriteria(() => HasEnvironmentVariable("GITHUB_TOKEN"))
	.WithCriteria(() => !string.IsNullOrEmpty(EnvironmentVariable("GITHUB_TOKEN")))
	.Does<BuildVersion>((version) =>
{
	if (version.MajorMinorPatch == version.SemVer)
	{
		GitReleaseManagerClose(token, organization, repository, version.MajorMinorPatch);
	}

	GitReleaseManagerPublish(token, organization, repository, version.SemVer);
});

Task("Default")
	.IsDependentOn("Pack")
	.IsDependentOn("Generate-LocalReport")
	.IsDependentOn("Push-NuGetPackages");

RunTarget(target);
