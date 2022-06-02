namespace CCVARN
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;
	using CCVARN.Commands;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.Exporters;
	using CCVARN.Core.IO;
	using CCVARN.Core.Parser;
	using CCVARN.DependencyInject;
	using CCVARN.IO;
	using CCVARN.Options;
	using DryIoc;
	using LibGit2Sharp;
	using Spectre.Console.Cli;

	internal static class Program
	{
		public static async Task<int> Main(string[] args)
		{
			var container = CreateContainer();
			var console = container.Resolve<IConsoleWriter>();

			if (args.Any(a => string.Equals(a, "parse", StringComparison.OrdinalIgnoreCase)))
			{
				if (args.Any(a => string.Equals(a, "stdout", StringComparison.OrdinalIgnoreCase)))
					console.DisableNormalOutput();

				if (args.Any(a => string.Equals(a, "stderr", StringComparison.OrdinalIgnoreCase)))
					console.DisableErrorOutput();
			}

			const string text = "[lightyellow3 on black]\n" +
				"  ____ ______     ___    ____  _   _\n" +
				" / ___/ ___\\ \\   / / \\  |  _ \\| \\ | |\n" +
				"| |  | |    \\ \\ / / _ \\ | |_) |  \\| |\n" +
				"| |__| |___  \\ V / ___ \\|  _ <| |\\  |\n" +
				" \\____\\____|  \\_/_/   \\_|_| \\_|_| \\_|\n" +
				"[/][aqua on black]{0,37}[/]\n" +
				"\n";

			var version = Assembly
				.GetExecutingAssembly()
				.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

			if (version.IndexOf('+', StringComparison.Ordinal) > 0)
				version = version[..version.IndexOf('+', StringComparison.Ordinal)];

			console.WriteInfoLine(text, version);

			var registrar = new TypeRegistrar(container);
			try
			{
				var app = new CommandApp(registrar);

				app.Configure(config =>
				{
					config.SetApplicationName("ccvarn");
					config.ValidateExamples();

					config.AddCommand<InitCommand>("init")
						.WithExample(new[] { "init" })
						.WithExample(new[] { "init", "--root", Environment.CurrentDirectory });

					config.AddCommand<ParseCommand>("parse")
						.WithExample(new[] { "parse", "ccvarn.json" })
						.WithExample(new[] { "parse", "stdout" })
						.WithExample(new[] { "parse", "stderr" })
						.WithExample(new[] { "parse", "ccvarn.json", "--output", "ReleaseNotes.md", "--output", "ReleaseNotes.txt" })
						.WithExample(new[] { "parse", "--output", "ReleaseNotes.md", "--output", "ReleaseNotes.txt" });
				});
				//app.SetDefaultCommand<ParseCommand>();

				return await app.RunAsync(args);
			}
			finally
			{
				container.Dispose();
			}
		}

		private static IContainer CreateContainer()
		{
			var container = new Container(rules => rules.WithTrackingDisposableTransients());
			container.Register<IConsoleWriter, ConsoleWriter>(Reuse.Singleton);
			container.RegisterDelegate(ResolveConfiguration, Reuse.Singleton);
			container.RegisterDelegate(RegisterRepository, Reuse.ScopedOrSingleton);
			container.Register<CommitParser>(Reuse.ScopedOrSingleton);
			container.Register<IExporter, JSonExporter>();
			container.Register<IExporter, PlainTextExporter>();
			container.Register<IExporter, MarkdownExporter>();

			container.Register<InitOptions>(Reuse.Singleton);
			container.Register<ParseOption>(Reuse.Singleton);

			return container;
		}

		private static IRepository RegisterRepository(IResolverContext arg)
		{
			var option = arg.Resolve<BaseSettings>();

			return new Repository(option.RepositoryRoot);
		}

		private static Config ResolveConfiguration(IResolverContext arg)
		{
			var option = arg.Resolve<BaseSettings>();

			return ConfigSerializer.LoadConfiguration(option.RepositoryRoot!);
		}
	}
}
