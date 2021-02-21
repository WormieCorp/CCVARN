namespace CCVARN.Tests.Exporters
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using ApprovalTests;
	using CCVARN.Core.Exporters;
	using CCVARN.Core.IO;
	using CCVARN.Core.Models;
	using CCVARN.Core.Tests.Exporters;
	using Moq;
	using NUnit.Framework;
	using Shouldly;

	public class ConsoleExporterTests
	{
		[Test]
		public void AcceptsStandardOutputAndStandardError([Values("stdout", "stderr")] string output)
		{
			var consoleMock = new Mock<IConsoleWriter>();
			var exporter = new ConsoleExporter(consoleMock.Object);

			exporter.CanExportToFile(output).ShouldBeTrue();
		}

		[Test]
		public void DoNotAcceptOtherValues([Values("help.json", "stdwarn", "release.md")] string output)
		{
			var consoleMock = new Mock<IConsoleWriter>();
			var exporter = new ConsoleExporter(consoleMock.Object);

			exporter.CanExportToFile(output).ShouldBeFalse();
		}

		[Test]
		public void GetsStandardOutputFromConsole()
		{
			var consoleMock = new Mock<IConsoleWriter>();
			consoleMock.SetupGet(c => c.StandardOut).Returns(TextWriter.Null);
			var exporter = new ConsoleExporter(consoleMock.Object);

			exporter.ExportParsedData(new ParsedData(VersionData.Parse("1.0.0"), new ReleaseNotesData()), "stdout", false);

			consoleMock.VerifyGet(c => c.StandardOut, Times.Once);
		}

		[Test]
		public void GetsStandardErrorFromConsole()
		{
			var consoleMock = new Mock<IConsoleWriter>();
			consoleMock.SetupGet(c => c.StandardError).Returns(TextWriter.Null);
			var exporter = new ConsoleExporter(consoleMock.Object);

			exporter.ExportParsedData(new ParsedData(VersionData.Parse("1.0.0"), new ReleaseNotesData()), "stderr", false);

			consoleMock.VerifyGet(c => c.StandardError, Times.Once);
		}

		[Test]
		public void ThrowsNotSupportedExceptionOnInvalidValues([Values("help.json", "stdwarn", "release.md")] string output)
		{
			var consoleMock = new Mock<IConsoleWriter>();
			consoleMock.SetupGet(c => c.StandardError).Returns(TextWriter.Null);
			consoleMock.SetupGet(c => c.StandardOut).Returns(TextWriter.Null);
			var exporter = new ConsoleExporter(consoleMock.Object);

			Should.Throw<NotSupportedException>(() =>
				exporter.ExportParsedData(new ParsedData(VersionData.Parse("1.0.0"), new ReleaseNotesData()), output, false));

			consoleMock.VerifyGet(c => c.StandardOut, Times.Never);
			consoleMock.VerifyGet(c => c.StandardError, Times.Never);
		}
	}
}
