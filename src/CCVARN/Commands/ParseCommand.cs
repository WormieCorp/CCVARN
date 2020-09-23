namespace CCVARN.Commands
{
	using System.ComponentModel;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.Parser;
	using CCVARN.Options;
	using DryIoc;
	using Spectre.Cli;

	[Description("Parses the current repository from the current HEAD to the first found tag commit.")]
	public sealed class ParseCommand : BaseCommand<ParseOption>
	{
		protected override int ExecuteCore(CommandContext context, ParseOption settings)
		{
			var commitParser = Container.Resolve<CommitParser>();

			var commits = commitParser.GetAllCommits();
			var result = commitParser.ParseVersionFromCommits(commits);

			Console!.WriteInfoLine("");
			Console.WriteInfoLine("New version is [aqua on black]{0}[/]", result);

			return 0;
		}
	}
}
