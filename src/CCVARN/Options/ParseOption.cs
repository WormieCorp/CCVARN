namespace CCVARN.Options
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using Spectre.Cli;

	[Description("Parses the current repository from the current HEAD to the first found tag commit.")]
	public sealed class ParseOption : BaseSettings
	{
		[CommandOption("-o|--output <ADDITIONAL_OUTPUTS>")]
		[Description("Additional outputs for outputting changelogs. Output type is based on the file extension (Markdown and plain text currently supported).")]
		public IEnumerable<string>? AdditionalOutputs { get; set; }

		[CommandArgument(0, "[JSON_OUTPUT]")]
		[DefaultValue("CCVARN.json")]
		[Description("The main output file to store the asserted information (defaults to CCVARN.json)")]
		public string? Output { get; set; }
	}
}
