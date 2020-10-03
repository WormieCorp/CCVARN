namespace CCVARN.Core.Exporters
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
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
			if (data is null)
				throw new ArgumentNullException(nameof(data));

			using var writer = new StreamWriter(outputPath, false, new UTF8Encoding(false));

			writer.WriteLine("# {0} (**{1}**) #", data.Version.SemVer, DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

			var breakingNotes = data.ReleaseNotes.Notes.Where(n => n.Key == "BREAKING CHANGE" || n.Key == "BREAKING CHANGES");

			if (data.ReleaseNotes.BreakingChanges.Any())
			{
				writer.WriteLine();
				writer.WriteLine("## BREAKING CHANGES ##");
				writer.WriteLine();

				foreach (var line in data.ReleaseNotes.BreakingChanges)
				{
					writer.WriteLine(line);
				}

				if (breakingNotes.Any())
					writer.WriteLine();
			}

			foreach (var notePair in breakingNotes)
			{
				AddNoteLines(writer, notePair, data.ReleaseNotes.BreakingChanges.Count == 0, 2);
			}

			foreach (var notePair in data.ReleaseNotes.Notes.Except(breakingNotes))
			{
				AddNoteLines(writer, notePair, true);
			}

			writer.Flush();

			this.console.WriteInfoLine(":check_mark: Exported [teal]Markdown Release Notes[/] to '[teal]{0}[/]'", outputPath);
		}

		private static void AddNoteLines(StreamWriter writer, KeyValuePair<string, List<NoteData>> notePair, bool includeHeader, int headerIndent = 3)
		{
			if (includeHeader)
			{
				writer.WriteLine();
				for (var i = 0; i < headerIndent; i++)
					writer.Write('#');
				writer.Write(" {0} ", notePair.Key);
				for (var i = 0; i < headerIndent; i++)
					writer.Write('#');
				writer.WriteLine();
				writer.WriteLine();
			}

			foreach (var note in notePair.Value)
			{
				writer.Write("- ");
				if (!string.IsNullOrEmpty(note.Scope))
					writer.Write("**{0}**: ", note.Scope);

				writer.Write(note.Summary);

				foreach (var issue in note.Issues)
					writer.Write(", closes #{0}", issue);

				writer.WriteLine();
			}
		}
	}
}
