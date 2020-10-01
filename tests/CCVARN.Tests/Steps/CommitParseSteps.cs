namespace CCVARN.Tests.Steps
{
	using CCVARN.Core.IO;
	using CCVARN.Core.Parser;
	using CCVARN.Tests.Models;
	using LibGit2Sharp;
	using Moq;
	using TechTalk.SpecFlow;

	[Binding]
	public class CommitParseSteps
	{
		private readonly ParsingModel data;

		public CommitParseSteps(ParsingModel data)
		{
			this.data = data;
		}

		[When("the user parses the commits")]
		public void WhenTheUserParsesTheCommits()
		{
			var parser = new CommitParser(this.data.Configuration, new Mock<IRepository>().Object, new Mock<IConsoleWriter>().Object);

			var result = parser.ParseVersionFromCommits(this.data.Commits);

			this.data.Results = result;
		}
	}
}
