namespace CCVARN.Commands
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Linq;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.Exporters;
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
			Console.WriteInfoLine("New version is [aqua on black]{0}[/]", result.Version);

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
						var directory = Path.GetDirectoryName(output);
						if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
						{
							Directory.CreateDirectory(directory);
						}

						exporter.ExportParsedData(result, output);
						success = true;
						break;
					}
				}

				if (!success)
				{
					Console.WriteErrorLine("Unable to find an exporter for file type '[teal]{0}[/]'! Exiting...",
						Path.GetExtension(output));

					return 1;
				}
			}

			return 0;
		}

		public override ValidationResult Validate(CommandContext context, ParseOption settings)
		{
			if (settings is null)
				throw new ArgumentNullException(nameof(settings));

			foreach (var output in settings.AdditionalOutputs)
			{
				var success = false;

				foreach (var exporter in this.exporters)
				{
					success = exporter.CanExportToFile(output);

					if (success)
						break;
				}

				if (!success)
					return ValidationResult.Error($"The path '{output}' do not use a file extension we can output to!");
			}

			return base.Validate(context, settings);
		}
	}
}
