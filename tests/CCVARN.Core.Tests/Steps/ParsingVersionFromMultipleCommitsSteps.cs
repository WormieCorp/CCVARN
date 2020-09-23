namespace CCVARN.Core.Tests.Steps
{
	using System.Collections.Generic;
	using System.Linq;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.IO;
	using CCVARN.Core.Models;
	using CCVARN.Core.Parser;
	using CCVARN.Core.Tests.Models;
	using LibGit2Sharp;
	using Moq;
	using TechTalk.SpecFlow;
	using TechTalk.SpecFlow.Assist;

	[Binding]
	public class ParsingVersionFromMultipleCommitsSteps
	{
		private readonly List<CommitInfo> commits = new List<CommitInfo>();
		private readonly Config configuration = new Config();
		private readonly ScenarioContext scenarioContext;

		public ParsingVersionFromMultipleCommitsSteps(ScenarioContext scenarioContext)
		{
			this.scenarioContext = scenarioContext;
		}

		[Given(@"the configuration tag (?:suffix )(.*)")]
		public void GivenTheConfigurationTag(string tag)
		{
			this.configuration.Tag = tag;
		}

		[Given(@"the following commits")]
		public void GivenTheFollowingCommits(Table table)
		{
			this.commits.AddRange(table.CreateSet<CommitInfoWrapper>());
		}

		[Given(@"the current tag is ([^\s]+) with message (.*)")]
		[Given(@"with the (?:following|previous) tag (?:commit )?([^\s]+)(?: with message)? (.*)")]
		public void GivenWithTheFollowingTagCommit(string referencOrVersion, string commitText)
		{
			var newCommit = new CommitInfoWrapper(true, $"refs/tags/{referencOrVersion}", commitText)
			{
				Ref = $"refs/tags/{referencOrVersion}",
				IsTag = true,
			};
			this.commits.Add(newCommit);
		}

		[When(@"the user parses the commits")]
		public void WhenTheUserParsesTheCommits()
		{
			var parser = new CommitParser(
				this.configuration,
				new Mock<IRepository>(MockBehavior.Loose).Object,
				new Mock<IConsoleWriter>(MockBehavior.Loose).Object
			);

			this.scenarioContext["EXPECTED_HASH"] = this.commits.FirstOrDefault()?.Sha;

			var version = parser.ParseVersionFromCommits(this.commits.AsEnumerable());
			this.scenarioContext["NEXT_VERSION"] = version;
		}
	}
}
