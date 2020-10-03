namespace CCVARN.Tests.Steps
{
	using System;
	using System.IO;
	using CCVARN.Core.Exporters;
	using CCVARN.Core.IO;
	using CCVARN.Core.Models;
	using Moq;
	using TechTalk.SpecFlow;

	[Binding]
	public class ExportingSteps
	{
		private readonly ParsedData data;
		private readonly ScenarioContext context;

		public ExportingSteps(ParsedData data, ScenarioContext context)
		{
			this.context = context;
			this.data = data;

		}

		[When("the user is exporting plain text")]
		public void WhenTheUserIsExportingPlainText()
		{
			var destination = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid() + ".txt");

			var exporter = new PlainTextExporter(new Mock<IConsoleWriter>().Object);

			exporter.ExportParsedData(this.data, destination);

			this.context["EXPORTED_FILE"] = destination;
		}

		[When("the user is exporting markdown text")]
		public void WhenTheUserIsExportingMarkdownText()
		{
			var destination = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid() + ".md");

			var exporter = new MarkdownExporter(new Mock<IConsoleWriter>().Object);

			exporter.ExportParsedData(this.data, destination);

			this.context["EXPORTED_FILE"] = destination;
		}

		[Scope(Tag = "plain-text")]
		[When("the user checks if (?:the )?path can be parsed")]
		public void WhenTheUserChecksIfThePlainTextPathCanBeParsed()
		{
			var parser = new PlainTextExporter(new Mock<IConsoleWriter>().Object);
			var path = (string)this.context["TEST_PATH"];

			this.context["CAN_PARSE_RESULT"] = parser.CanExportToFile(path);
		}

		[Scope(Tag = "markdown-text")]
		[When("the user checks if (?:the )?path can be parsed")]
		public void WhenTheUserChecksIfTheMarkdownPathCanBeParsed()
		{
			var parser = new MarkdownExporter(new Mock<IConsoleWriter>().Object);
			var path = (string)this.context["TEST_PATH"];

			this.context["CAN_PARSE_RESULT"] = parser.CanExportToFile(path);
		}
	}
}
