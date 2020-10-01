namespace CCVARN.Tests.Steps
{
	using CCVARN.Tests.Models;
	using TechTalk.SpecFlow;
	using TechTalk.SpecFlow.Assist;

	[Binding]
	public class CommitAddSteps
	{
		private readonly ParsingModel data;

		public CommitAddSteps(ParsingModel data)
		{
			this.data = data;
		}

		[Given("no (?:commits?|tags?)")]
		public void GivenNoCommitsOrTags()
		{
			this.data.Commits.Clear(); // Just to be sure we do not have any commits
		}

		[Given("(?:the )?configured tag suffix (?:is )?(.*)")]
		public void GivenTheConfiguredTagSuffix(string suffix)
		{
			if (suffix == "empty")
				this.data.Configuration.Tag = string.Empty;
			else
				this.data.Configuration.Tag = suffix;
		}

		[Given("the next version is set to (.*)")]
		public void GivenTheNextVersionIsSetTo(string version)
		{
			this.data.Configuration.NextVersion = version;
		}

		[Given("the (?:previous|following) tag (.*) with the message (.*)")]
		[Given(@"the(?: previous)? tag with (?:the )?version (.*) with the message (.*)")]
		public void GivenTheTagWithVersionAndMessage(string version, string message)
		{
			var newCommit = new CommitInfoWrapper(true, $"refs/tags/{version}", message);
			this.data.Commits.Add(newCommit);
		}

		[Given("the following commits")]
		[Given("have the previous(?: raw)? commits")]
		public void GivenThePreviousCommits(Table table)
		{
			var newCommits = table.CreateSet<CommitInfoWrapper>();

			this.data.Commits.AddRange(newCommits);
		}

		[Given("the following commit (.*)")]
		public void GivenTheFollowingCommit(string commitText)
		{
			var newCommit = new CommitInfoWrapper(commitText.Replace("\\n", "\n"));

			this.data.Commits.Add(newCommit);
		}
	}
}
