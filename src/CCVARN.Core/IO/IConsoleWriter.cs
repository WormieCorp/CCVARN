namespace CCVARN.Core.IO
{
	public interface IConsoleWriter
	{
		void AddIndent();

		void RemoveIndent();

		void WriteInfo(string format, params object[] parameters);

		void WriteInfoLine(string format, params object[] parameters);
	}
}
