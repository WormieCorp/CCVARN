namespace CCVARN.Tests.Models
{
	using System.Collections.Generic;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.Models;

	public class ParsingModel
	{
		public Config Configuration { get; } = new Config();

		public List<CommitInfo> Commits { get; } = new List<CommitInfo>();

		public ParsedData Results { get; set; } = null;
	}
}
