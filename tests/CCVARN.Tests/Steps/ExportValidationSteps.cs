namespace CCVARN.Tests.Steps
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Text;
	using Shouldly;
	using TechTalk.SpecFlow;

	[Binding]
	public class ExportValidationSteps
	{
		private readonly ScenarioContext context;

		public ExportValidationSteps(ScenarioContext context)
		{
			this.context = context;
		}

		[Then("the exported (?:plain )?release notes should be")]
		public void ThenTheExportedPlainReleaseNotesShouldBe(Table table)
		{
			var expected = new StringBuilder();
			expected.AppendLine(table.Header.First().Replace("<TODAY>", DateTime.Today.ToString("yyyy-MM-dd")));

			foreach (var line in table.Rows)
			{
				expected.AppendLine(line[0]);
			}

			var destination = (string)this.context["EXPORTED_FILE"];
			var actual = File.ReadAllText(destination, Encoding.UTF8);
			File.Delete(destination);

			actual.ShouldBe(expected.ToString());
		}

		[Then("the result should be (true|false)")]
		public void ThenTheResultShouldBe(bool expected)
		{
			var actual = (bool)this.context["CAN_PARSE_RESULT"];

			actual.ShouldBe(expected);
		}
	}
}
