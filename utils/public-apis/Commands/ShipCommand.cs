namespace public_apis.Commands
{
	using System.IO;
	using System.Threading.Tasks;
	using public_apis.Settings;
	using Spectre.Console;
	using Spectre.Console.Cli;

	internal sealed class ShipCommand : AsyncCommand<ShipSettings>
	{
		public override async Task<int> ExecuteAsync(CommandContext context, ShipSettings settings)
		{
			ArgumentNullException.ThrowIfNull(context);
			ArgumentNullException.ThrowIfNull(settings);

			if (settings.Directory is null)
			{
				settings.Directory = FindSolutionDirectory();
			}

			return await CopyUnshippedApiAsync(settings.Directory, settings.Resort);
		}

		private static DirectoryInfo FindSolutionDirectory()
		{
			var directory = new DirectoryInfo(Environment.CurrentDirectory);

			while (directory is not null)
			{
				var solutionsCount = directory.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly).Count();

				if (solutionsCount > 1)
				{
					throw new ApplicationException($"More than 1 solution was found in '{directory}'!");
				}
				else if (solutionsCount == 1)
				{
					return directory;
				}

				directory = directory.Parent;
			}

			if (directory?.Parent is null || directory == directory.Parent)
			{
				throw new ApplicationException($"Unable to find parent directory for solution file!");
			}

			return directory;
		}

		private async Task<int> CopyUnshippedApiAsync(DirectoryInfo path, bool resort)
		{
			ArgumentNullException.ThrowIfNull(path);

			if (!path.Exists)
			{
				AnsiConsole.MarkupLineInterpolated($"[red]ERR: Path '{path.FullName.EscapeMarkup()}' does not exist![/]");
				return 2;
			}

			var foundAtLeastOne = false;

			var options = new EnumerationOptions
			{
				MatchCasing = MatchCasing.CaseInsensitive,
				RecurseSubdirectories = true,
			};

			foreach (var unshippedTxtPath in path.EnumerateFiles("PublicAPI.Unshipped.txt", options))
			{
				foundAtLeastOne = true;
				if (unshippedTxtPath.Length == 0 && !resort)
				{
					AnsiConsole.MarkupLineInterpolated($"[yellow]{unshippedTxtPath.FullName.EscapeMarkup()}: Up to date[/]");
					continue;
				}

				var shippedTxtPath = new FileInfo(Path.Combine(unshippedTxtPath.DirectoryName ?? path.FullName, "PublicAPI.Shipped.txt"));

				if (!shippedTxtPath.Exists)
				{
					AnsiConsole.MarkupLineInterpolated($"[yellow]{shippedTxtPath.FullName.EscapeMarkup()}: Created[/]");
				}

				var shippedLines = new List<string>();
				var unshippedLines = new List<string>();
				var removeLines = new List<string>();
				var unshippedApiCount = 0;
				var removedApiCount = 0;

				using (var stream = unshippedTxtPath.OpenText())
				{
					string? line;
					while ((line = await stream.ReadLineAsync()) is not null)
					{
						if (!string.IsNullOrWhiteSpace(line))
						{
							if (line.StartsWith("#"))
							{
								unshippedLines.Add(line);
							}
							else if (line.StartsWith("*REMOVED*"))
							{
								removeLines.Add(line[9..].TrimStart());
								removedApiCount++;
							}
							else
							{
								shippedLines.Add(line);
								unshippedApiCount++;
							}
						}
					}
				}

				using (var stream = shippedTxtPath.OpenText())
				{
					string? line;
					while ((line = await stream.ReadLineAsync()) is not null)
					{
						if (!string.IsNullOrWhiteSpace(line) && !shippedLines.Contains(line, StringComparer.OrdinalIgnoreCase) && !removeLines.Contains(line, StringComparer.OrdinalIgnoreCase))
						{
							shippedLines.Add(line);
						}
					}
				}

				shippedLines.Sort(StringComparer.OrdinalIgnoreCase);

				await File.WriteAllLinesAsync(shippedTxtPath.FullName, shippedLines);
				await File.WriteAllLinesAsync(unshippedTxtPath.FullName, unshippedLines);

				AnsiConsole.MarkupLineInterpolated($"{unshippedTxtPath.FullName.EscapeMarkup()}: Shipped {unshippedApiCount} APIs, Removed {removedApiCount} APIs.");
			}

			if (!foundAtLeastOne)
			{
				AnsiConsole.MarkupLineInterpolated($"Did not find any PublicAPI.Unshipped.txt files under {path.FullName.EscapeMarkup()}");
			}

			return 0;
		}
	}
}
