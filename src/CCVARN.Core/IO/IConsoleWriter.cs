namespace CCVARN.Core.IO
{
	public interface IConsoleWriter
	{
		void AddIndent();

		void RemoveIndent();

		void WriteErrorLine(string format, params object[] parameters);

		void WriteInfo(string format, params object[] parameters);

		void WriteInfoLine(string format, params object[] parameters);
	}
}
