namespace CCVARN.Core.Exporters
{
	using System;
	using CCVARN.Core.IO;
	using CCVARN.Core.Models;

	public sealed class ConsoleExporter : JSonExporter
	{
		public ConsoleExporter(IConsoleWriter writer)
			: base(writer)
		{
		}

		public override bool CanExportToFile(string filePath)
		{
			return string.Equals(filePath, "stdout", StringComparison.OrdinalIgnoreCase) ||
				string.Equals(filePath, "stderr", StringComparison.OrdinalIgnoreCase);
		}

		public override void ExportParsedData(ParsedData data, string outputPath, bool excludeHeader)
		{
			string outputType;
			if (string.Equals(outputPath, "stdout", StringComparison.OrdinalIgnoreCase))
			{
				ExportJsonInformation(System.Console.Out, data);
				outputType = "standard output";
			}
			else if (string.Equals(outputPath, "stderr", StringComparison.OrdinalIgnoreCase))
			{
				ExportJsonInformation(System.Console.Error, data);
				outputType = "standard error output";
			}
			else
			{
				throw new NotSupportedException($"The specified stream '{outputPath}' is not supported by the console exporter!");
			}

			// This should only output when we standard output is not disabled.
			// We should still output in case of future usage when logging
			// is enabled.
			this.Console.WriteInfoLine(":check_mark: Exported [teal]JSON Data[/] to '[teal]{0}[/]'", outputType);
		}
	}
}
