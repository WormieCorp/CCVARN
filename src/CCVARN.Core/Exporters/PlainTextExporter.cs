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

	public sealed class PlainTextExporter : IExporter
	{
		private readonly IConsoleWriter console;

		public PlainTextExporter(IConsoleWriter console)
		{
			this.console = console;
		}

		public bool CanExportToFile(string filePath)
		{
			var extension = Path.GetExtension(filePath);

			return string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase);
		}

		public void ExportParsedData(ParsedData data, string outputPath)
		{
			if (data is null)
				throw new ArgumentNullException(nameof(data));

			using var writer = new StreamWriter(outputPath, false, new UTF8Encoding(false));

			var text = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", data.Version.SemVer, DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
			writer.WriteLine(text);
			WriteUnderline(writer, text.Length, '=');

			var breakingNotes = data.ReleaseNotes.Notes.Where(n => n.Key == "BREAKING CHANGE" || n.Key == "BREAKING CHANGES");

			if (data.ReleaseNotes.BreakingChanges.Count > 0)
			{
				writer.WriteLine();
				const string header = "BREAKING CHANGES";
				writer.WriteLine(header);
				WriteUnderline(writer, header.Length);

				foreach (var breakingChange in data.ReleaseNotes.BreakingChanges)
				{
					writer.WriteLine(breakingChange);
				}

				if (breakingNotes.Any())
					writer.WriteLine();
			}

			foreach (var notePain in breakingNotes)
			{
				AddNoteLines(writer, notePain, data.ReleaseNotes.BreakingChanges.Count == 0);
			}

			foreach (var notePair in data.ReleaseNotes.Notes.Except(breakingNotes))
			{
				AddNoteLines(writer, notePair, true);
			}

			writer.Flush();

			this.console.WriteInfoLine(":check_mark: Exported [teal]Plain Text Release Notes[/] to '[teal]{0}[/]'", outputPath);
		}

		private static void AddNoteLines(StreamWriter writer, KeyValuePair<string, List<NoteData>> notePair, bool includeHeader)
		{
			if (includeHeader)
			{
				writer.WriteLine();
				writer.WriteLine(notePair.Key);
				WriteUnderline(writer, notePair.Key.Length);
			}

			foreach (var note in notePair.Value)
			{
				writer.Write("- ");
				if (!string.IsNullOrEmpty(note.Scope))
					writer.Write("({0}): ", note.Scope);
				writer.WriteLine(note.Summary);
			}
		}

		private static void WriteUnderline(StreamWriter writer, int len, char ch = '-')
		{
			for (var i = 0; i < len; i++)
			{
				writer.Write(ch);
			}
			writer.WriteLine();
		}
	}
}
