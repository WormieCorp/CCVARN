namespace CCVARN.Tests.Steps
{
	using System.Linq;
	using CCVARN.Tests.Models;
	using Shouldly;
	using TechTalk.SpecFlow;

	[Binding]
	public class CommitValidationSteps
	{
		private readonly ParsingModel data;

		public CommitValidationSteps(ParsingModel data)
		{
			this.data = data;
		}

		[Then(@"the resulting version should be (.*)")]
		public void ThenTheResultingVersionShouldBe(string expectedVersion)
		{
			var actual = this.data.Results;

			actual.ShouldNotBeNull();
			actual.Version.ShouldNotBeNull();
			actual.Version.ToString(false).ShouldBe(expectedVersion);
		}

		[Then(@"sha is set to tag commit")]
		public void ThenShaIsSetToTagCommit()
		{
			var expectedSha = this.data.Commits.First(c => c.IsTag).Sha;

			this.data.Results.Version.Metadata.ShouldBe(expectedSha);
		}

		[Then(@"with the commit count (\d+)")]
		public void ThenWithCommitCount(int count)
		{
			this.data.Results.Version.Commits.ShouldBe(count);
		}
	}
}
