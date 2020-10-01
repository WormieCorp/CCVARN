namespace CCVARN.Core.Parser
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.WebSockets;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.IO;
	using CCVARN.Core.Models;

	internal sealed class ReleaseNoteParser
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
			var typeScope = GetMatchingTypeScop(commit, this.types);

			Description? title;
			if (typeScope != null && (typeScope.IncludeInChangelog || commit.IsBreakingChange))
			{
				title = typeScope.Description;
			}
			else
			{
				this.writer.AddIndent();
				this.writer.WriteInfoLine("[dim]:white_exclamation_mark: Not a commit to include in the changelog.[/]");
				this.writer.RemoveIndent();
				return;
			}

			var note = new NoteData(commit.CommitType, commit.Message, commit.CommitScope ?? string.Empty);

			note.Issues.AddRange(commit.IssueRefs);

			if (commit.IsBreakingChange)
			{
				if (!string.IsNullOrEmpty(commit.BreakingChangeNote))
					releaseNotes.BreakingChanges.Add(commit.BreakingChangeNote);
				else
					title = new Description("BREAKING CHANGE", "BREAKING CHANGES");
			}

			var currentNotes = UpdateOrAddNote(releaseNotes, title);

			currentNotes.Add(note);

			this.writer.AddIndent();

			this.writer.WriteInfoLine(":check_mark_button: [teal]A new [fuchsia on black]{0}[/] was found. Adding to changelog![/]", title.Singular);

			this.writer.RemoveIndent();
		}

		private static List<NoteData> UpdateOrAddNote(ReleaseNotesData releaseNotes, Description title)
		{
			List<NoteData> currentNotes;
			if (releaseNotes.Notes.ContainsKey(title.Singular))
			{
				currentNotes = releaseNotes.Notes[title.Singular];
				if (title.Plural != null)
				{
					releaseNotes.Notes.Remove(title.Singular);
					releaseNotes.Notes.Add(title.Plural, currentNotes);
				}
			}
			else if (title.Plural != null && releaseNotes.Notes.ContainsKey(title.Plural))
			{
				currentNotes = releaseNotes.Notes[title.Plural];
			}
			else
			{
				currentNotes = new List<NoteData>();
				releaseNotes.Notes.Add(title.Singular, currentNotes);
			}

			return currentNotes;
		}

		private static TypeScope? GetMatchingTypeScop(ConventionalCommitInfo commit, HashSet<TypeScope> types)
		{
			var typeConfig = types.FirstOrDefault(t => string.Equals(t.Type, commit.CommitType, StringComparison.OrdinalIgnoreCase));
			var typeScopeConfig = types.FirstOrDefault(t => string.Equals(t.Type, commit.CommitType, StringComparison.OrdinalIgnoreCase) &&
				string.Equals(t.Scope, commit.CommitScope, StringComparison.OrdinalIgnoreCase));

			return typeScopeConfig ?? typeConfig;
		}
	}
}
