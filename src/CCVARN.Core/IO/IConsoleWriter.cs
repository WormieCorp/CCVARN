namespace CCVARN.Core.IO
{
	using System.IO;

	public interface IConsoleWriter
	{
		TextWriter StandardOut { get; }
		TextWriter StandardError { get; }

		void DisableNormalOutput();
		void DisableErrorOutput();

		void AddIndent();

		void RemoveIndent();

		void WriteErrorLine(string format, params object[] parameters);

		void WriteInfo(string format, params object[] parameters);

		void WriteInfoSafe(string format, params string[] parameters);

		void WriteInfoLine(string format, params object[] parameters);

		void WriteInfoLineSafe(string format, params string[] parameters);
	}
}
