namespace CCVARN.Commands
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Linq;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.Parser;
	using CCVARN.Options;
	using DryIoc;
	using Spectre.Cli;

	[Description("Parses the current repository from the current HEAD to the first found tag commit.")]
	public sealed class ParseCommand : BaseCommand<ParseOption>
	{
		private readonly IReadOnlyCollection<IExporter> exporters;

		public ParseCommand(IReadOnlyCollection<IExporter> exporters)
		{
			this.exporters = exporters;
		}

		protected override int ExecuteCore(CommandContext context, ParseOption settings)
		{
			var commitParser = Container.Resolve<CommitParser>();

			var commits = commitParser.GetAllCommits();
			var result = commitParser.ParseVersionFromCommits(commits);

			Console!.WriteInfoLine("");
			Console.WriteInfoLine("New version is [aqua on black]{0}[/]", result);

			var outputs = new List<string>();
			if (!string.IsNullOrEmpty(settings.Output))
				outputs.Add(settings.Output);
			if (settings.AdditionalOutputs.Any())
				outputs.AddRange(settings.AdditionalOutputs!);

			foreach (var output in outputs)
			{
				var success = false;
				foreach (var exporter in this.exporters)
				{
					if (exporter.CanExportToFile(output))
					{
						exporter.ExportParsedData(result, output);
						success = true;
					}
				}

				if (!success)
				{
					Console.WriteErrorLine("Unable to find an exporter for file type '[grey]{0}[/]'! Exiting...",
						Path.GetExtension(output));
				}
			}

			return 0;
		}
	}
}
