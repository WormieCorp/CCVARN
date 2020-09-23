namespace CCVARN.Core.Models
{
	public class CommitInfo
	{
		public CommitInfo(string sha, string rawText)
		{
			Sha = sha;
			RawText = rawText;
		}

		public bool IsTag { get; set; }
		public string RawText { get; set; }
		public string? Ref { get; set; }
		public string Sha { get; set; }
	}
}
