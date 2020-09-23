namespace CCVARN.Commands
{
	using System;
	using System.IO;
	using CCVARN.Core.IO;
	using CCVARN.Options;
	using DryIoc;
	using Spectre.Cli;

	public abstract class BaseCommand<TSettings> : Command<TSettings>
		where TSettings : BaseSettings
	{
		public IConsoleWriter? Console { get; set; }

		public IContainer? Container { get; set; }

		public sealed override int Execute(CommandContext context, TSettings settings)
		{
			Container.RegisterMapping<BaseSettings, TSettings>();

			return ExecuteCore(context, settings);
		}

		public override ValidationResult Validate(CommandContext context, TSettings settings)
		{
			var repoRoot = GetRootPath(settings);

			if (repoRoot is null)
			{
				return ValidationResult.Error($"The path '{Environment.CurrentDirectory}' is not a git repository!");
			}
			else if (!Directory.Exists(Path.Combine(repoRoot, ".git")))
			{
				return ValidationResult.Error($"The path '{repoRoot}' is not the root of a git repository");
			}

			Console!.WriteInfoLine("Using repository root: [grey]{0}[/]", repoRoot);

			settings.RepositoryRoot = repoRoot;

			return base.Validate(context, settings);
		}

		protected abstract int ExecuteCore(CommandContext context, TSettings settings);

		protected string? GetRootPath(TSettings settings)
		{
			if (!string.IsNullOrEmpty(settings.RepositoryRoot))
			{
				if (Path.IsPathFullyQualified(settings.RepositoryRoot))
					return settings.RepositoryRoot;
				else
					return Path.GetFullPath(settings.RepositoryRoot);
			}

			var directory = Environment.CurrentDirectory;

			while (directory != null)
			{
				var gitDir = Path.Combine(directory, ".git");
				if (Directory.Exists(gitDir))
				{
					break;
				}
				else if (directory == Path.GetPathRoot(directory))
				{
					directory = null;
					break;
				}
				else
				{
					directory = Path.GetDirectoryName(directory);
				}
			}

			return directory;
		}
	}
}
