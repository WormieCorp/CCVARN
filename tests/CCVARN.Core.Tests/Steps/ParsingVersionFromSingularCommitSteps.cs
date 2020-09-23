namespace CCVARN.Core.Tests.Steps
{
	using CCVARN.Core.Models;
	using CCVARN.Core.Parser;
	using CCVARN.Core.Tests.Models;
	using TechTalk.SpecFlow;
	using TechTalk.SpecFlow.Assist;

	[Binding]
	public class ParsingVersionFromSingularCommitSteps
	{
		private readonly ScenarioContext scenarioContext;
		private CommitInfo commit;
		private VersionData previousVersion;

		public ParsingVersionFromSingularCommitSteps(ScenarioContext scenarioContext)
		{
			this.scenarioContext = scenarioContext;
		}

		[Given(@"a(?: tag)? commit with the data")]
		public void GivenATagCommitWithTheData(Table table)
		{
			this.commit = table.CreateInstance<CommitInfoWrapper>();
		}

		[Given(@"current version is (.*)")]
		public void GivenCurrentVersionIs(string version)
		{
			this.previousVersion = version is null ? null : VersionData.Parse(version);
		}

		[Given(@"current version set to (major|minor|patch|none|weight) bump")]
		public void GivenCurrentVersionSetToBump(VersionBump bump)
		{
			this.previousVersion.SetNextBump(bump);
		}

		[Given(@"no current version")]
		public void GivenNoCurrentVersion()
		{
			this.previousVersion = null;
		}

		[When(@"commits the returned version")]
		public void WhenCommitsTheReturnedVersion()
		{
			var nextVersion = this.scenarioContext["NEXT_VERSION"] as VersionData;
			nextVersion.CommitNextBump();
		}

		[When(@"the user is parsing the next commit")]
		public void WhenTheUserIsParsingTheNextCommit()
		{
			var parser = new VersionParser();
			this.scenarioContext["NEXT_VERSION"] = parser.ParseVersion(this.previousVersion, this.commit);
		}
	}
}
