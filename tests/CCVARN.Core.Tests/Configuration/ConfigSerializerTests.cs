namespace CCVARN.Core.Tests
{
	using System.Collections.Generic;
	using System.IO;
	using ApprovalTests;
	using CCVARN.Core.Configuration;
	using NUnit.Framework;

	[NonParallelizable]
	public class ConfigSerializerTests
	{
		private static readonly string testDirectory = Path.Combine(Path.GetTempPath(), "ccvarn-tests");
		private static readonly string testFile = Path.Combine(testDirectory, "CCVARN.yml");

		[SetUp]
		public void CreateTestDirectory()
		{
			if (!Directory.Exists(testDirectory))
				Directory.CreateDirectory(testDirectory);
		}

		[OneTimeTearDown]
		public void RemoveTestFolder()
		{
			Directory.Delete(testDirectory, true);
		}

		[Test]
		public void SerializingDefaultConfiguration()
		{
			var config = new Config();
			ConfigSerializer.SaveConfiguration(testDirectory, config);

			Approvals.VerifyFile(testFile);
		}

		[Test]
		public void SerializingConfigurationWithCustomSettings()
		{
			var config = new Config
			{
				NextVersion = "0.5.0",
				Tag = "rc",
				TypeScopes = new HashSet<TypeScope>
				{
					new TypeScope("yes", new Description("Singular", "Plural"), "my scope"),
					new TypeScope("feature", new Description("Feature", "Features"))
				}
			};
			ConfigSerializer.SaveConfiguration(testDirectory, config);
			Approvals.VerifyFile(testFile);
		}
	}
}
