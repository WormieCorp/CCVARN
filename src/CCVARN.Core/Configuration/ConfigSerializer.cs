namespace CCVARN.Core.Configuration
{
	using System;
	using System.IO;
	using System.Text;
	using CCVARN.Core.Configuration.Comments;
	using YamlDotNet.Serialization;
	using YamlDotNet.Serialization.NamingConventions;

	public static class ConfigSerializer
	{
		private static readonly Lazy<DeserializerBuilder> deseriazireBuilder = new Lazy<DeserializerBuilder>(() =>
			new DeserializerBuilder()
			.WithNamingConvention(HyphenatedNamingConvention.Instance));

		private static readonly Encoding encoding = new UTF8Encoding(false);

		private static readonly Lazy<SerializerBuilder> serializerBuilder = new Lazy<SerializerBuilder>(() =>
					new SerializerBuilder()
			.WithNamingConvention(HyphenatedNamingConvention.Instance)
			.WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
			.WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor)));

		public static Config LoadConfiguration(string rootDirectory)
		{
			var expectedFile = Path.Combine(rootDirectory, "CCVARN.yml");

			if (!File.Exists(expectedFile))
				return new Config();

			var deserializer = deseriazireBuilder.Value.Build();
			var text = File.ReadAllText(expectedFile, encoding);

			var config = deserializer.Deserialize<Config>(text);

			return config;
		}

		public static void SaveConfiguration(string rootDirectory, Config configuration)
		{
			var file = Path.Combine(rootDirectory, "CCVARN.yml");
			if (File.Exists(file))
				File.Delete(file);

			var serializer = serializerBuilder.Value.Build();

			var text = serializer.Serialize(configuration);

			File.WriteAllText(file, text, encoding);
		}
	}
}
