namespace CCVARN.Core.Parser
{
	using System.Collections.Generic;
	using System.Linq;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.IO;
	using CCVARN.Core.Models;

	public sealed class ReleaseNoteParser
	{
		private readonly HashSet<TypeScope> types;
		private readonly IConsoleWriter writer;

		public ReleaseNoteParser(HashSet<TypeScope> types, IConsoleWriter writer)
		{
			this.types = types;
			this.writer = writer;
		}

		public void ParseReleaseNotes(ReleaseNotesData releaseNotes, ConventionalCommitInfo commit)
		{
			var typeConfig = this.types.FirstOrDefault(t => string.Equals(t.Type, commit.CommitType));
			var typeScopeConfig = this.types.FirstOrDefault(t => string.Equals(t.Type, commit.CommitType) &&
				string.Equals(t.Scope, commit.CommitScope));
			Description? title = null;

			if (typeScopeConfig != null && (typeScopeConfig.IncludeInChangelog || commit.IsBreakingChange))
				title = typeScopeConfig.Description;
			else if (typeConfig != null && (typeConfig.IncludeInChangelog || commit.IsBreakingChange))
				title = typeConfig.Description;
			else
				return;

			var note = new NoteData(commit.CommitType, commit.Message)
			{
				Scope = commit.CommitScope,
			};

			note.Issues.AddRange(commit.IssueRefs);

			if (commit.IsBreakingChange && !string.IsNullOrEmpty(commit.BreakingChangeNote))
			{
				releaseNotes.BreakingChanges.Add(commit.BreakingChangeNote);
			}

			if (!releaseNotes.ReleaseNotes.ContainsKey(title))
				releaseNotes.ReleaseNotes.Add(title, new List<NoteData>());
			releaseNotes.ReleaseNotes[title].Add(note);

			this.writer.AddIndent();

			this.writer.WriteInfoLine("A new [fuchsia on black]{0}[/] was found. Adding to changelog!", title);
		}
	}
}
