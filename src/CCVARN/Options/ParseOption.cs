namespace CCVARN.Options
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using Spectre.Cli;

	[Description("Parses the current repository from the current HEAD to the first found tag commit.")]
	public class ParseOption : BaseSettings
	{
		[CommandOption("--output <OUTPUT_PATH>")]
		[Description("Additional outputs for outputting changelogs. Output type is based on the file extension (Markdown and plain text currently supported).")]
		public string[] AdditionalOutputs { get; set; } = new string[0];

		[CommandArgument(0, "[JSON_OUTPUT]")]
		[DefaultValue("CCVARN.json")]
		[Description("The main output file to store the asserted information (defaults to CCVARN.json)")]
		public string? Output { get; set; }
	}
}
