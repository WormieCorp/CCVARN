namespace CCVARN.IO
{
	using System;
	using System.Globalization;
	using System.Text;
	using CCVARN.Core.IO;
	using Spectre.Console;

	public class ConsoleWriter : IConsoleWriter
	{
		private readonly IAnsiConsole console;
		private readonly IAnsiConsole errorConsole;
		private string currentIndent = string.Empty;

		public ConsoleWriter()
		{
			Console.OutputEncoding = Encoding.UTF8;
			this.console = AnsiConsole.Create(new AnsiConsoleSettings
			{
				Ansi = AnsiSupport.Detect,
				ColorSystem = ColorSystemSupport.Detect,
				Out = Console.Out,
			});
			this.errorConsole = AnsiConsole.Create(new AnsiConsoleSettings
			{
				Ansi = AnsiSupport.Detect,
				ColorSystem = ColorSystemSupport.Detect,
				Out = Console.Error,
			});
		}

		public void AddIndent()
		{
			this.currentIndent += "  ";
		}

		public void RemoveIndent()
		{
			const int indentLen = 2;
			if (this.currentIndent.Length < indentLen)
				return;

			this.currentIndent = this.currentIndent[..^indentLen];
		}

		public void WriteErrorLine(string format, params object[] parameters)
		{
			const string prefix = "[teal][[[red]ERR[/]]][/]";
			this.console.Write(this.currentIndent, Style.Plain);
			this.console.Markup(prefix);
			this.console.MarkupLine(format, parameters);
		}

		public void WriteInfo(string format, params object[] parameters)
		{
			this.console.MarkupLine(CultureInfo.CurrentCulture, format, parameters);
		}

		public void WriteInfoLine(string format, params object[] parameters)
		{
			this.console.Write(this.currentIndent, Style.Plain);
			this.console.MarkupLine(CultureInfo.CurrentCulture, format, parameters);
		}
	}
}
