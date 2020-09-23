namespace CCVARN
{
	using System;
	using System.Reflection;
	using System.Threading.Tasks;
	using CCVARN.Commands;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.IO;
	using CCVARN.Core.Parser;
	using CCVARN.DependencyInject;
	using CCVARN.IO;
	using CCVARN.Options;
	using DryIoc;
	using LibGit2Sharp;
	using Spectre.Cli;

	internal static class Program
	{
		public static Task<int> Main(string[] args)
		{
			var container = CreateContainer();
			var console = container.Resolve<IConsoleWriter>();

			const string text = @"[lightyellow3 on black]
  ____ ______     ___    ____  _   _
 / ___/ ___\ \   / / \  |  _ \| \ | |
| |  | |    \ \ / / _ \ | |_) |  \| |
| |__| |___  \ V / ___ \|  _ <| |\  |
 \____\____|  \_/_/   \_|_| \_|_| \_|
[/][aqua on black]{0,37}[/]
";

			var version = Assembly
				.GetExecutingAssembly()
				.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

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
						.WithExample(new[] { "parse", "ccvarn.json", "--output", "ReleaseNotes.md", "--output", "ReleaseNotes.txt" })
						.WithExample(new[] { "parse", "--output", "ReleaseNotes.md", "--output", "ReleaseNotes.txt" });
				});
				//app.SetDefaultCommand<ParseCommand>();

				return app.RunAsync(args);
			}
			finally
			{
				container.Dispose();
			}
		}

		private static IContainer CreateContainer()
		{
			var container = new Container();
			container.Register<IConsoleWriter, ConsoleWriter>(Reuse.Singleton);
			container.RegisterDelegate(ResolveConfiguration, Reuse.Singleton);
			container.RegisterDelegate(RegisterRepository, Reuse.ScopedOrSingleton);
			container.Register<CommitParser>(Reuse.ScopedOrSingleton);

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
