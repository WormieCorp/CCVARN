namespace CCVARN.Core.Models
{
	using System;
	using System.Collections.Generic;

	public class ConventionalCommitInfo : CommitInfo
	{
		public ConventionalCommitInfo(CommitInfo commitInfo, string type, string? scope, string message)
			: this(
				commitInfo?.Sha ?? throw new ArgumentNullException(nameof(commitInfo)),
				commitInfo.RawText,
				type,
				scope,
				message)
		{
			IsTag = commitInfo.IsTag;
			Ref = commitInfo.Ref;
		}

		public ConventionalCommitInfo(string sha, string rawText, string type, string? scope, string message)
			: base(sha, rawText)
		{
			CommitType = type;
			CommitScope = scope;
			Message = message;
		}

		public string? Body { get; set; }
		public string? BreakingChangeNote { get; set; }
		public string? CommitScope { get; }
		public string CommitType { get; }
		public string? Footer { get; set; }
		public bool IsBreakingChange { get; set; }
		public List<int> IssueRefs { get; } = new List<int>();
		public string Message { get; }
	}
}
