namespace CCVARN.Core.Exporters
{
	using System.Text;
	using System.IO;
	using System;
    using CCVARN.Core.Models;
	using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
	using CCVARN.Core.IO;
	using CCVARN.Core.Configuration;

	public class JSonExporter : IExporter
    {
		private readonly IConsoleWriter console;

		public JSonExporter(IConsoleWriter console)
		{
			this.console = console;
		}

        public bool CanExportToFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);

			return string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase);
        }

        public void ExportParsedData(ParsedData data, string outputPath)
        {
			var serializer = new JsonSerializer
			{
				Formatting = Formatting.Indented,
				ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new CamelCaseNamingStrategy(),
				},
			};

			using var sw = new StreamWriter(outputPath, false, new UTF8Encoding(false));
			using var writer = new JsonTextWriter(sw);

			serializer.Serialize(writer, data);

			this.console.WriteInfoLine(":cross_mark: Exported [grey]JSON Data[/] to '[grey]{0}[/]'", outputPath);
        }
    }
}
