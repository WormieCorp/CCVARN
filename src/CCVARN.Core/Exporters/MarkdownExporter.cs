namespace CCVARN.Core.Exporters
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using CCVARN.Core.IO;
	using CCVARN.Core.Models;

	public sealed class MarkdownExporter : IExporter
	{
		private readonly IConsoleWriter console;

		public MarkdownExporter(IConsoleWriter console)
		{
			this.console = console;
		}

		public bool CanExportToFile(string filePath)
		{
			var extension = Path.GetExtension(filePath);

			return string.Equals(extension, ".md", StringComparison.OrdinalIgnoreCase);
		}

		public void ExportParsedData(ParsedData data, string outputPath)
		{
			using var writer = new StreamWriter(outputPath, false, new UTF8Encoding(false));

			writer.WriteLine("# {0} (**{1}**) #", data.Version.SemVer, DateTime.Now.ToString("yyyy-MM-dd"));
			writer.WriteLine();

			if (data.ReleaseNotes.BreakingChanges.Any())
			{
				writer.WriteLine("## BREAKING CHANGES ##");
				writer.WriteLine();

				foreach (var line in data.ReleaseNotes.BreakingChanges)
				{
					writer.WriteLine(line);
				}

				writer.WriteLine();
			}

			foreach (var notePair in data.ReleaseNotes.Notes)
			{
				writer.WriteLine("### {0} ###", notePair.Key);
				writer.WriteLine();

				foreach (var note in notePair.Value)
				{
					writer.Write("- ");
					if (!string.IsNullOrEmpty(note.Scope))
						writer.Write("**{0}**: ", note.Scope);

					writer.Write(note.Summary);

					foreach (var issue in note.Issues)
					{
						writer.Write(", closes #{0}", issue);
					}

					writer.WriteLine();
				}

				writer.WriteLine();
			}

			writer.Flush();

			this.console.WriteInfoLine(":check_mark: Exported [teal]Markdown Release Notes[/] to '[teal]{0}[/]'", outputPath);
		}
	}
}
