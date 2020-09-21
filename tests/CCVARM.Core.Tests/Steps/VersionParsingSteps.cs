using Shouldly;
using System;
using TechTalk.SpecFlow;

namespace CCVARM.Core.Tests.Steps
{
    [Binding]
    public class VersionParsingSteps
    {
        private readonly ScenarioContext scenarioContext;

        public VersionParsingSteps(ScenarioContext context)
        {
            this.scenarioContext = context;
        }
        
        [Then(@"the result should be ([\d\.]+)")]
        public void ThenTheResultShouldBe(Version version)
        {
            var parsedVersion = this.scenarioContext["Parsed_Version"];
            parsedVersion.ShouldNotBeNull();
            parsedVersion.ShouldBeOfType<Version>();
            parsedVersion.ShouldBe(version);
        }
    }
}
