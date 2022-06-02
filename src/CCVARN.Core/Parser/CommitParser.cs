namespace CCVARN.Core.Parser
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.IO;
	using CCVARN.Core.Models;
	using LibGit2Sharp;

	public sealed class CommitParser
	{
		private readonly Config config;
		private readonly ReleaseNoteParser _releaseNoteParser;
		private readonly IRepository _repository;
		private readonly VersionParser _versionParser;
		private readonly IConsoleWriter _writer;
		private readonly System.Version _nextVersion;
		private readonly bool _allowMajorBump = true;

		public CommitParser(Config config, IRepository repository, IConsoleWriter writer)
		{
			this.config = config ?? throw new ArgumentNullException(nameof(config));
			this._repository = repository;
			this._writer = writer;
			this._versionParser = new VersionParser(this.config.TypeScopes);
			this._releaseNoteParser = new ReleaseNoteParser(this.config.TypeScopes, writer);

			this._nextVersion = System.Version.Parse(config.NextVersion);

			if (this._nextVersion < System.Version.Parse("1.0.0") && this._nextVersion > System.Version.Parse("0.0.0"))
				this._allowMajorBump = false;
		}

		public IEnumerable<CommitInfo> GetAllCommits()
		{
			var head = this._repository!.Head;

			foreach (var commit in head.Commits)
			{
				CommitInfo? commitInfo = null;
				try
				{
					var foundTag = this._repository.Tags?.FirstOrDefault(t => t.Target.Sha == commit.Sha);
					var branch = this._repository.Branches.FirstOrDefault(b => b.Tip.Sha == commit.Sha);
					commitInfo = new CommitInfo(commit.Sha, commit.Message)
					{
						Ref = foundTag?.CanonicalName ?? branch?.CanonicalName,
						IsTag = foundTag != null,
					};
				}
				catch (Exception ex)
				{
					this._writer.WriteWarningLine("Unable to parse commit '{0}'. Ignoring...", commit.Sha);
					this._writer.AddIndent();
					this._writer.WriteWarningLine(ex.Message);
					this._writer.RemoveIndent();
				}

				if (commitInfo is not null)
				{
					yield return commitInfo;
				}
			}
		}

		public ParsedData ParseVersionFromCommits(IEnumerable<CommitInfo> commits)
		{
			if (commits is null)
				throw new ArgumentNullException(nameof(commits));

			VersionData? version = null;
			var releaseNotes = new ReleaseNotesData();
			var firstCommit = true;
			var firstCommitIsTag = false;
			var metadata = string.Empty;
			var commitCount = 0;

			foreach (var commit in commits)
			{
				this._writer.AddIndent();
				var parsedCommit = ParseConventionalCommit(commit, firstCommit || !commit.IsTag);
				if (parsedCommit != null || commit.IsTag)
					version = this._versionParser.ParseVersion(version, parsedCommit ?? commit);

				if (parsedCommit != null && (firstCommit || !commit.IsTag))
					this._releaseNoteParser.ParseReleaseNotes(releaseNotes, parsedCommit);

				this._writer.RemoveIndent();

				if (firstCommit)
				{
					if (!commit.IsTag && commit.RawText.StartsWith("Merge ", StringComparison.OrdinalIgnoreCase))
						continue;
					firstCommitIsTag = commit.IsTag;
					metadata = commit.Sha;
				}

				if (!firstCommit && commit.IsTag)
					break;

				if (firstCommit || !commit.IsTag)
					commitCount++;

				firstCommit = false;
			}

			if (version?.IsEmpty() != false)
			{
				if (!string.IsNullOrEmpty(this.config.Tag))
				{
					version = VersionData.Parse(this.config.NextVersion + "-" + this.config.Tag);
					if (firstCommitIsTag)
						version.Weight = 1;
				}
				else
				{
					version = VersionData.Parse(this.config.NextVersion);
				}
			}

			var versionBumped = false;
			if (!firstCommitIsTag)
			{
				versionBumped = ForceNextVersion(version);
			}
			else if (version.MajorMinorPatch == "0.0.0")
			{
				version.SetNextBump(VersionBump.None, true);
				versionBumped = ForceNextVersion(version);
			}

			version.Metadata = metadata;
			version.Commits = commitCount;
			if (!firstCommitIsTag && versionBumped &&
				!string.IsNullOrEmpty(this.config.Tag) &&
				!string.Equals(this.config.Tag, version.PreReleaseLabel, StringComparison.OrdinalIgnoreCase) &&
				string.Compare(this.config.Tag, version.PreReleaseLabel, StringComparison.OrdinalIgnoreCase) > 0)
			{
				version.PreReleaseLabel = this.config.Tag;
				version.Weight = Math.Max(version.Weight ?? 1, 1);
			}

			return new ParsedData(version, releaseNotes);
		}

		private bool ForceNextVersion(VersionData version)
		{
			bool versionIsBumped;
			if (version.Major < this._nextVersion.Major)
			{
				version.SetNextBump(VersionBump.Major);
				versionIsBumped = version.CommitNextBump(this._allowMajorBump);
			}
			else if (version.Minor < this._nextVersion.Minor)
			{
				version.SetNextBump(VersionBump.Minor);
				versionIsBumped = version.CommitNextBump(this._allowMajorBump);
			}
			else if (version.Patch < this._nextVersion.Build)
			{
				version.SetNextBump(VersionBump.Patch);
				versionIsBumped = version.CommitNextBump(this._allowMajorBump);
			}
			else
			{
				versionIsBumped = version.CommitNextBump(this._allowMajorBump);

				if (version.MajorMinorPatch == "0.0.0" && this._nextVersion.ToString(3) == "0.0.0")
				{
					version.SetNextBump(VersionBump.Major);
					versionIsBumped = version.CommitNextBump(this._allowMajorBump);
				}
			}

			return versionIsBumped;
		}

		private static string AddResolvedIssues(ConventionalCommitInfo newCommit, string line)
		{
			var closeKeywords = new[]
							{
					"close #",
					"closes #",
					"closed #",
					"fix #",
					"fixes #",
					"fixed #",
					"resolve #",
					"resolves #",
					"resolved #"
				};

			var subs = line;
			string? found;
			while ((found = Array.Find(closeKeywords, c => subs.StartsWith(c, StringComparison.OrdinalIgnoreCase))) != null)
			{
				var tempSub = subs[found.Length..];
				var numS = string.Empty;
				for (var i = 0; i < tempSub.Length && char.IsDigit(tempSub[i]); i++)
					numS += tempSub[i];

				if (!string.IsNullOrEmpty(numS))
					newCommit.IssueRefs.Add(int.Parse(numS, CultureInfo.InvariantCulture));

				subs = tempSub[numS.Length..].Trim(new[] { ' ', '.', ',' });
			}

			return subs;
		}

		private ConventionalCommitInfo? ParseConventionalCommit(CommitInfo commit, bool reportCommit)
		{
			if (reportCommit)
			{
				if (commit.IsTag && commit.Ref != null)
					this._writer.WriteInfoLine("Parsing tag '[invert]{0}[/]'", commit.Ref);
				else
					this._writer.WriteInfoLine("Parsing commit '[invert]{0}[/]'", commit.Sha);
				var text = commit.RawText.Split('\n').First(text => !string.IsNullOrEmpty(text)).Trim();
				text = Regex.Replace(text, @"(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?", "[link]$0[/]", RegexOptions.Compiled);
				this._writer.AddIndent();
				this._writer.WriteInfoLineSafe("{0}", text);
			}
			else
			{
				this._writer.AddIndent();
			}

			var regex = new Regex(@"^(?<type>[a-z]+)(?:\((?<scope>[^\)]+)\))?(?<breaking>!)?\:\s*(?<message>[^\r\n]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var m = regex.Match(commit.RawText);
			if (m.Success)
				return UpdateCommitInfo(commit, m);
			else if (reportCommit && commit.RawText.TrimStart().StartsWith("Merge", StringComparison.OrdinalIgnoreCase))
				this._writer.WriteInfoLine("[yellow]:double_exclamation_mark:[/] Found merge commit...");
			else if (reportCommit)
				this._writer.WriteInfoLine(":cross_mark: Not a conventional commit...");

			this._writer.RemoveIndent();

			return null;
		}

		private ConventionalCommitInfo UpdateCommitInfo(CommitInfo commit, Match match)
		{
			this._writer.RemoveIndent();
			var type = match.Groups["type"].Value;
			var scope = match.Groups["scope"].Value;
			var message = match.Groups["message"].Value;
			var breaking = match.Groups["breaking"].Success;

			var newCommit = new ConventionalCommitInfo(commit, type, scope, message)
			{
				IsBreakingChange = breaking,
			};

			var body = new StringBuilder();
			var breakingNote = new StringBuilder();

			var isBreaking = false;

			foreach (var line in commit.RawText.Split('\n').Skip(1).Select(r => r.Trim()))
			{
				var result = AddResolvedIssues(newCommit, line);
				if (line != result)
				{
					isBreaking = false;
					continue;
				}

				if (line.StartsWith("Signed-off-by:", StringComparison.OrdinalIgnoreCase)
					|| line.StartsWith("Co-authored-by:", StringComparison.OrdinalIgnoreCase))
				{
					// We expect these to be on the last lines,
					// as such we will break and stop parsing the
					// commit message.
					break;
				}
				else if (isBreaking)
				{
					breakingNote.AppendLine(line);
				}
				else if (line.StartsWith("BREAKING CHANGE:", StringComparison.Ordinal) || line.StartsWith("BREAKING-CHANGE:", StringComparison.Ordinal))
				{
					breakingNote.AppendLine(line[16..]);
					isBreaking = true;
				}
				else
				{
					body.AppendLine(line);
				}
			}

			if (breakingNote.Length > 0)
				newCommit.BreakingChangeNote = breakingNote.ToString().Trim();
			else if (body.Length > 0 && breaking)
				newCommit.BreakingChangeNote = body.ToString().Trim();

			if (body.Length > 0 && !breaking)
				newCommit.Body = body.ToString().Trim();

			newCommit.IsBreakingChange |= isBreaking;

			return newCommit;
		}
	}
}
