namespace CCVARN.Core.IO
{
	public interface IConsoleWriter
	{
		void AddIndent();

		void RemoveIndent();

		void WriteErrorLine(string format, params object[] parameters);

		void WriteInfo(string format, params object[] parameters);

		void WriteInfoSafe(string format, params string[] parameters);

		void WriteInfoLine(string format, params object[] parameters);

		void WriteInfoLineSafe(string format, params string[] parameters);
	}
}
