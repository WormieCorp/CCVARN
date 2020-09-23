namespace CCVARN.Core.Exporters
{
	using System;
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
			using var writer = new StreamWriter(outputPath, false, new UTF8Encoding(false));

			var text = string.Format("{0} ({1})", data.Version.SemVer, DateTime.Now.ToString("yyyy-MM-dd"));
			writer.WriteLine(text);
			WriteUnderline(writer, text.Length, '=');

			writer.WriteLine();

			if (data.ReleaseNotes.BreakingChanges.Any())
			{
				const string header = "BREAKING CHANGES";
				writer.WriteLine(header);
				WriteUnderline(writer, header.Length);

				foreach (var breakingChange in data.ReleaseNotes.BreakingChanges)
				{
					writer.WriteLine(breakingChange);
				}

				writer.WriteLine();
			}

			foreach (var notePair in data.ReleaseNotes.Notes)
			{
				writer.WriteLine(notePair.Key);
				WriteUnderline(writer, notePair.Key.Length);
				writer.WriteLine();

				foreach (var note in notePair.Value)
				{
					writer.Write("- ");
					if (!string.IsNullOrEmpty(note.Scope))
						writer.Write("({0}): ", note.Scope);
					writer.WriteLine(note.Summary); // TODO: Include the sha for the commit as well
				}

				writer.WriteLine();
			}

			writer.Flush();

			this.console.WriteInfoLine(":check_mark: Exported [teal]Plain Text Release Notes[/] to '[teal]{0}[/]'", outputPath);
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
