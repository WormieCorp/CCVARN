namespace CCVARN.Commands
{
	using System.ComponentModel;
	using CCVARN.Core.Configuration;
	using CCVARN.Options;
	using Spectre.Cli;

	[Description("Initializes the current repository with a new default configuration file\n([yellow]WARNING: This will overwrite any existing configuration file[/])")]
	public sealed class InitCommand : BaseCommand<InitOptions>
	{
		protected override int ExecuteCore(CommandContext context, InitOptions settings)
		{
			Console!.WriteInfoLine("Exporting standard configuration file");
			ConfigSerializer.SaveConfiguration(settings.RepositoryRoot!, new Config() { NextVersion = "1.0.0" });
			Console.WriteInfo(":check_mark: Configuration file successfully exported!");

			return 0;
		}
	}
}
