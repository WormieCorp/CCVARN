namespace CCVARN.Options
{
	using System;
	using System.ComponentModel;
	using System.IO;
	using Spectre.Cli;

	public abstract class BaseSettings : CommandSettings
	{
		[CommandOption("-r|--root")]
		[Description("The path to the repository root to parse the commits for.")]
		public string? RepositoryRoot { get; set; }

		public override ValidationResult Validate()
		{
			if (!string.IsNullOrEmpty(RepositoryRoot))
			{
				string directory;
				if (Path.IsPathFullyQualified(RepositoryRoot))
					directory = RepositoryRoot;
				else
					directory = Path.Combine(Environment.CurrentDirectory, RepositoryRoot);

				if (!Directory.Exists(directory))
					return ValidationResult.Error($"The path '{RepositoryRoot}' is not an existing directory!");
			}

			return base.Validate();
		}
	}
}
