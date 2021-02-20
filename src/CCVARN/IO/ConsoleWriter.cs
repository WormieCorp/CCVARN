namespace CCVARN.IO
{
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using CCVARN.Core.IO;
	using Spectre.Console;

	public class ConsoleWriter : IConsoleWriter
	{
		private readonly IAnsiConsole console;
		private readonly IAnsiConsole errorConsole;
		private string currentIndent = string.Empty;
		private bool standardOutputDisabled;
		private bool errorOutputDisabled;

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
			if (this.errorOutputDisabled)
				return;

			const string prefix = "[teal][[[red]ERR[/]]][/] ";
			this.errorConsole.Write(this.currentIndent, Style.Plain);
			this.errorConsole.Markup(prefix);
			this.errorConsole.MarkupLine(format, parameters);
		}

		public void WriteInfo(string format, params object[] parameters)
		{
			if (this.standardOutputDisabled)
				return;

			this.console.MarkupLine(CultureInfo.CurrentCulture, format, parameters);
		}

		public void WriteInfoSafe(string format, params string[] parameters)
		{
			if (this.standardOutputDisabled)
				return;

			var safeParameters = parameters.Select(p => p.EscapeMarkup()).ToArray();

			WriteInfo(format, safeParameters);
		}

		public void WriteInfoLine(string format, params object[] parameters)
		{
			if (this.standardOutputDisabled)
				return;

			this.console.Write(this.currentIndent, Style.Plain);
			this.console.MarkupLine(CultureInfo.CurrentCulture, format, parameters);
		}

		public void WriteInfoLineSafe(string format, params string[] parameters)
		{
			if (this.standardOutputDisabled)
				return;

			var safeParameters = parameters.Select(p => p.EscapeMarkup()).ToArray();

			WriteInfoLine(format, safeParameters);
		}

		public void DisableNormalOutput()
		{
			this.standardOutputDisabled = true;
		}

		public void DisableErrorOutput()
		{
			this.errorOutputDisabled = true;
		}
	}
}
