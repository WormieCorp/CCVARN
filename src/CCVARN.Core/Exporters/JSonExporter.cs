namespace CCVARN.Core.Exporters
{
	using System;
	using System.IO;
	using System.Text;
	using CCVARN.Core.IO;
	using CCVARN.Core.Models;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;

	public class JSonExporter : IExporter
	{
		protected IConsoleWriter Console { get; }

		public JSonExporter(IConsoleWriter console)
		{
			Console = console;
		}

		public virtual bool CanExportToFile(string filePath)
		{
			var extension = Path.GetExtension(filePath);

			return string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase);
		}

		public virtual void ExportParsedData(ParsedData data, string outputPath, bool excludeHeader)
		{
			using var sw = new StreamWriter(outputPath, false, new UTF8Encoding(false));
			ExportJsonInformation(sw, data);

			Console.WriteInfoLine(":check_mark: Exported [teal]JSON Data[/] to '[teal]{0}[/]'", outputPath);
		}

		protected static void ExportJsonInformation(TextWriter stream, ParsedData data)
		{
			using var writer = new JsonTextWriter(stream);
			var serializer = new JsonSerializer
			{
				Formatting = Formatting.Indented,
				ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new CamelCaseNamingStrategy()
				},
			};

			serializer.Serialize(writer, data);
		}
	}
}
