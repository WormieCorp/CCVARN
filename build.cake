#addin nuget:?package=Cake.Codecov&version=0.9.1

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var artifactsDir = Argument<DirectoryPath>("artifacts", "./.artifacts");
var solution = "./CCVARN.sln";
var dotnetExec = Context.Tools.Resolve("dotnet") ?? Context.Tools.Resolve("dotnet.exe");

public class BuildVersion
{
	public string MajorMinorPatch { get; set; }
	public string SemVer { get; set; }
	public string SemVerFull { get; set; }
	public string Tag { get; set; }
	public string BuildMetadata { get; set; }
}

Setup((context) =>
{
	var majorMinorPatch = "1.0.0";
	var tag = "alpha.1.1";
	var buildMeta = tag;
	return new BuildVersion
	{
		MajorMinorPatch = majorMinorPatch,
		SemVer = $"{majorMinorPatch}-{tag}",
		SemVerFull = $"{majorMinorPatch}-{buildMeta}",
		Tag = tag,
		BuildMetadata = buildMeta,
	};
});

Task("Clean")
	.Does(() =>
{
	CleanDirectory(artifactsDir);
});

Task("Build")
	.IsDependentOn("Clean")
	.Does<BuildVersion>((version) =>
{
	DotNetCoreBuild(solution, new DotNetCoreBuildSettings
	{
		Configuration = configuration,
		NoIncremental = HasArgument("no-incremental"),
		MSBuildSettings = new DotNetCoreMSBuildSettings()
			.SetVersionPrefix(version.MajorMinorPatch),
		VersionSuffix = version.BuildMetadata,
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
			.AppendSwitchQuoted("DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format", "=", "opencover,cobertura"),
		Configuration = configuration,
		NoBuild = true,
		ResultsDirectory = artifactsDir.Combine("coverage/tests"),
	});
});

Task("Pack")
	.IsDependentOn("Test")
	.Does<BuildVersion>((version) =>
{
	DotNetCorePack(solution, new DotNetCorePackSettings
	{
		Configuration = configuration,
		NoBuild = true,
		OutputDirectory = artifactsDir.Combine("packages"),
		MSBuildSettings = new DotNetCoreMSBuildSettings()
			.SetVersionPrefix(version.MajorMinorPatch),
		VersionSuffix = version.BuildMetadata,
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

Task("Default")
	.IsDependentOn("Pack")
	.IsDependentOn("Generate-LocalReport")
	.IsDependentOn("Upload-CoverageToCodecov");

RunTarget(target);
