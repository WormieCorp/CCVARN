namespace CCVARN.Tests.Steps
{
	using CCVARN.Core.Models;
	using Shouldly;
	using TechTalk.SpecFlow;

	[Binding]
	public class VersionDataSteps
	{
		private readonly ScenarioContext scenarioContext;
		private string versionOrReference;

		public VersionDataSteps(ScenarioContext scenarioContext)
		{
			this.scenarioContext = scenarioContext;
		}

		[Given(@"a tag (?:reference)? of (?:refs\/tags\/)?(.*)")]
		public void GivenATagPrefixOf(string reference)
		{
			this.versionOrReference = "refs/tags/" + reference;
		}

		[Given("a version of (.*)")]
		public void GivenAVersionOf(string version)
		{
			this.versionOrReference = version;
		}

		[Then("the version output should be (.*)")]
		public void ThenTheVersionOutputShouldBe(string expected)
		{
			this.scenarioContext["NEXT_VERSION"].ShouldNotBeNull();
			var vd = this.scenarioContext["NEXT_VERSION"].ShouldBeOfType<VersionData>();
			if (this.scenarioContext.ContainsKey("EXPECTED_HASH"))
				vd.ToString().ShouldBe(expected + "+" + this.scenarioContext["EXPECTED_HASH"]);
			else
				vd.ToString().ShouldBe(expected);
		}

		[When("the user (?:tries to )?bumps? the (major|minor|patch|weight) part")]
		public void WhenBumpsTheTypePart(VersionBump versionBump)
		{
			var version = this.scenarioContext["NEXT_VERSION"] as VersionData;
			version.SetNextBump(versionBump);
			version.CommitNextBump();
		}

		[When("the user parses? the version")]
		[When("the user parses? the reference")]
		public void WhenTheUserParsesTheReference()
		{
			var version = VersionData.Parse(this.versionOrReference);
			this.scenarioContext["NEXT_VERSION"] = version;
		}

		[When("the user tries to bump the version when none is used")]
		public void WhenTheUserTriesToBumpTheVersionWhenNoneIsUsed()
		{
			var version = this.scenarioContext["NEXT_VERSION"] as VersionData;
			version.SetNextBump(VersionBump.None);
			version.CommitNextBump();
		}
	}
}
