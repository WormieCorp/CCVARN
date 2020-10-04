namespace CCVARN.Core.Exporters
{
	using CCVARN.Core.Models;

	public interface IExporter
	{
		bool CanExportToFile(string filePath);
		void ExportParsedData(ParsedData data, string outputPath, bool excludeHeader);
	}
}
