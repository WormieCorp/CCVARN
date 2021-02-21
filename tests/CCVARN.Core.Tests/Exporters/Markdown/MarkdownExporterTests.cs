namespace CCVARN.Tests.Exporters
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using ApprovalTests;
	using ApprovalTests.Namers;
	using ApprovalTests.Writers;
	using CCVARN.Core.Exporters;
	using CCVARN.Core.IO;
	using CCVARN.Core.Models;
	using CCVARN.Core.Tests.Exporters;
	using Moq;
	using NUnit.Framework;
	using Shouldly;

	public class MarkdownExporterTests : BaseExporterTests
	{
		protected override string Extension => "md";

		[Test]
		public void ExportingASingleFeature()
		{
			VerifyExportedData(
				"1.0.0",
				false,
				("Feature", ("feat", "some kind of feature"))
			);
		}

		[Test]
		public void ExportingASingleFeatureWithoutHeader()
		{
			VerifyExportedData(
				"1.0.0",
				true,
				("Feature", ("feat", "some kind of feature"))
			);
		}

		[Test]
		public void ExportingASingeBugFix()
		{
			VerifyExportedData(
				"1.0.1",
				false,
				("Bug fix", ("fix", "some kind of bug fix"))
			);
		}

		[Test]
		public void ExportingASingeBugFixWithoutHeader()
		{
			VerifyExportedData(
				"1.0.1",
				true,
				("Bug fix", ("fix", "some kind of bug fix"))
			);
		}

		[Test]
		public void ExportingMultipleFeatures()
		{
			VerifyExportedData(
				"1.1.0",
				false,
				("Features", new[] {
					("feat", "some awesome new feature"),
					("feat", "another awesome feature")
				})
			);
		}

		[Test]
		public void ExportingMultipleFeaturesWithoutHeader()
		{
			VerifyExportedData(
				"1.1.0",
				true,
				("Features", new[] {
					("feat", "some awesome new feature"),
					("feat", "another awesome feature")
				})
			);
		}

		[Test]
		public void ExportingSingleRefactorWithBreakingChange()
		{
			VerifyExportedData(
				"1.0.0",
				true,
				("BREAKING CHANGE", ("refactor", "renamed MethodA to MethodB"))
			);
		}

		[Test]
		public void ExportingSingleRefactorWithBreakingChangeAndBody()
		{
			VerifyExportedData(
				"2.0.0",
				@"In the class Something the MethodA was renamed to methodB
to allow something different",
				true,
				("Code refactoring", ("refactor", "renamed MethodA to MethodB"))
			);
		}

		[Test]
		public void ExportingSingleFeatureWithScope()
		{
			VerifyExportedData(
				"1.3.0",
				true,
				("Feature", ("feat", "some new feature", "awesome"))
			);
		}

		[Test]
		public void ExportingMixedBreakingChanges()
		{
			var releaseNotes = new ReleaseNotesData();
			releaseNotes.Notes.Add("Feature", new List<NoteData>
			{
				new NoteData("feat", "some feature")
			});
			releaseNotes.Notes.Add("BREAKING CHANGE", new List<NoteData>
			{
				new NoteData("feat", "some breaking change")
			});
			releaseNotes.Notes.Add("BREAKING CHANGES", new List<NoteData>
			{
				new NoteData("fix", "some breaking fix")
			});
			releaseNotes.BreakingChanges.Add("This is a very big breaking change");
			VerifyExportedData(
				VersionData.Parse("3.0.0"),
				true,
				releaseNotes
			);
		}

		[Test]
		public void CanExportFilesWithFileExtension([Values(".md", ".MD", ".Md", ".mD")] string extensions)
		{
			var randomFile = Path.GetRandomFileName() + extensions;
			var exporter = new MarkdownExporter(new Mock<IConsoleWriter>().Object);

			exporter.CanExportToFile(randomFile).ShouldBeTrue();
		}

		[Test]
		public void CanNOTExportFilesWithFileExtension([Values(".txt", ".xml", ".bin", "")] string extension)
		{
			var randomFile = Path.GetRandomFileName() + extension;
			var exporter = new MarkdownExporter(new Mock<IConsoleWriter>().Object);

			exporter.CanExportToFile(randomFile).ShouldBeFalse();
		}

		protected override IExporter CreateExporter()
			=> new MarkdownExporter(new Mock<IConsoleWriter>().Object);
	}
}
