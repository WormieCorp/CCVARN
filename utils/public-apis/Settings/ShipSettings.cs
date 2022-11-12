namespace public_apis.Settings
{
	using Spectre.Console;
	using Spectre.Console.Cli;

	internal class ShipSettings : CommandSettings
	{
		[CommandArgument(0, "[DIRECTORY_PATH]")]
		public DirectoryInfo? Directory { get; set; }

		[CommandOption("--resort")]
		public bool Resort { get; set; }

		public override ValidationResult Validate()
		{
			if (Directory is null)
			{
				return ValidationResult.Success();
			}

			var solutionsCount = Directory.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly).Count();

			if (solutionsCount == 0)
			{
				return ValidationResult.Error($"No solution could be found in '{Directory}'!");
			}
			else if (solutionsCount > 1)
			{
				return ValidationResult.Error($"More than 1 solution was found in '{Directory}'!");
			}

			return ValidationResult.Success();
		}
	}
}
