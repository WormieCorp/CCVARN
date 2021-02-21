namespace CCVARN.Core.Tests.Exporters
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using ApprovalTests;
	using CCVARN.Core.Exporters;
	using CCVARN.Core.Models;
	using NUnit.Framework;

	public abstract class BaseExporterTests
	{
		protected abstract IExporter CreateExporter();
		protected abstract string Extension { get; }

		protected void VerifyExportedData(
			string version,
			bool excludeHeader,
			(string, (string, string)) releaseNotes
		)
		{
			VerifyExportedData(
				version,
				string.Empty,
				excludeHeader,
				(releaseNotes.Item1, releaseNotes.Item2)
			);
		}

		protected void VerifyExportedData(
			string version,
			bool excludeHeader,
			(string, (string, string, string)) releaseNotes
		)
		{
			VerifyExportedData(
				version,
				string.Empty,
				excludeHeader,
				(releaseNotes.Item1, releaseNotes.Item2)
			);
		}

		protected void VerifyExportedData(
			string version,
			bool excludeHeader,
			(string, (string, string)[]) releaseNotes
		)
		{
			VerifyExportedData(
				version,
				string.Empty,
				excludeHeader,
				(releaseNotes.Item1, releaseNotes.Item2)
			);
		}

		protected void VerifyExportedData(
			string version,
			bool excludeHeader,
			(string, (string, string, string)[]) releaseNotes
		)
		{
			VerifyExportedData(
				version,
				string.Empty,
				excludeHeader,
				(releaseNotes.Item1, releaseNotes.Item2)
			);
		}

		protected void VerifyExportedData(
			string version,
			string breakingBody,
			bool excludeHeader,
			(string, (string, string)) releaseNotes
		)
		{
			VerifyExportedData(
				version,
				breakingBody,
				excludeHeader,
				(releaseNotes.Item1, new[] { releaseNotes.Item2 })
			);
		}

		protected void VerifyExportedData(
			string version,
			string breakingBody,
			bool excludeHeader,
			(string, (string, string, string)) releaseNotes
		)
		{
			VerifyExportedData(
				version,
				breakingBody,
				excludeHeader,
				(releaseNotes.Item1, new[] { releaseNotes.Item2 })
			);
		}

		protected void VerifyExportedData(
			string version,
			string breakingBody,
			bool excludeHeader,
			(string, (string, string)[]) releaseNotes
		)
		{
			var releaseNotesData = new ReleaseNotesData();
			var items = releaseNotes.Item2.Select((item) => new NoteData(item.Item1, item.Item2)).ToList();
			releaseNotesData.Notes.Add(releaseNotes.Item1, items);
			if (!string.IsNullOrEmpty(breakingBody))
			{
				releaseNotesData.BreakingChanges.AddRange(
					breakingBody.Replace("\r\n", "\n").Split('\n')
				);
			}

			VerifyExportedData(
				VersionData.Parse(version),
				excludeHeader,
				releaseNotesData
			);
		}

		protected void VerifyExportedData(
			string version,
			string breakingBody,
			bool excludeHeader,
			(string, (string, string, string)[]) releaseNotes
		)
		{
			var releaseNotesData = new ReleaseNotesData();
			var items = releaseNotes.Item2.Select((item) => new NoteData(item.Item1, item.Item2, item.Item3)).ToList();
			releaseNotesData.Notes.Add(releaseNotes.Item1, items);
			if (!string.IsNullOrEmpty(breakingBody))
			{
				releaseNotesData.BreakingChanges.AddRange(
					breakingBody.Replace("\r\n", "\n").Split('\n')
				);
			}

			VerifyExportedData(
				VersionData.Parse(version),
				excludeHeader,
				releaseNotesData
			);
		}

		protected void VerifyExportedData(VersionData version, bool excludeHeader, ReleaseNotesData releaseNotes)
		{
			var data = new ParsedData(version, releaseNotes);
			var outputPath = Path.GetRandomFileName() + "." + Extension;
			var exporter = CreateExporter();

			exporter.ExportParsedData(data, outputPath, excludeHeader);

			string text;
			try
			{
				text = File.ReadAllText(outputPath);
			}
			finally
			{
				File.Delete(outputPath);
			}

			Approvals.VerifyWithExtension(text, "." + Extension, ReplaceCurrentDate);
		}

		private static string ReplaceCurrentDate(string oldData)
		{
			var replacement = DateTime.Today.ToString("yyyy-MM-dd");

			return oldData.Replace(replacement, "2021-01-01");
		}
	}
}
