namespace CCVARN.Core.Tests.Parsers
{
	using System;
	using System.Collections.Generic;
	using System.Security.Cryptography.Pkcs;
	using ApprovalTests;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.IO;
	using CCVARN.Core.Models;
	using CCVARN.Core.Parser;
	using CCVARN.Tests.Models;
	using LibGit2Sharp;
	using Moq;
	using NUnit.Framework;
	using Shouldly;

	public partial class CommitParserTests
	{
		[Test]
		public void ParsingUnstableTaggedFeatureCommit()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "1.3.0-alpha.1", "feat: some awesome feature")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingMultipleUnstableFeatureCommitsWithTag()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "1.4.4-alpha.1", "feat: awesome: tag commit"),
				new CommitInfoWrapper("feat: this is just a commit")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(2);
		}

		[Test]
		public void ParsingTaggedCommitWithoutVersion()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "invalid", "feat: test commit")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingTaggedNonAlphaCommit()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "1.3.3-beta.5", "fix: yup this happened")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingEmptyGitHistory()
		{
			var commits = Array.Empty<CommitInfoWrapper>();

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(0);
		}

		[Test]
		public void ParsingSingleFeatureCommit()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat: some kind of awesome feature"),
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingSingleFeatureWithPreviousTag()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat: some kind of awesome feature"),
				new CommitInfoWrapper(true, "1.2.3", "docs: update changelog")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingMultipleFeatureCommits()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat: some kind of awesome feature 2"),
				new CommitInfoWrapper("feat: some kind of awesome feature 1")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(2);
		}

		[Test]
		public void ParsingMultipleFeatureCommitsWithPreviousTag()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat: awesomesouce"),
				new CommitInfoWrapper("feat: best new feature ever creted"),
				new CommitInfoWrapper(true, "2.4.4", "docs: update changelog")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(2);
		}

		[Test]
		public void ParsingSingleFixCommit()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("fix: correct unintentional bug"),
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingSingleFixCommitWithPreviousTag()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("fix: correct code"),
				new CommitInfoWrapper(true, "3.1.1", "docs: update changelog")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		[TestCase("alpha")]
		[TestCase("beta")]
		[TestCase("ceta")]
		[TestCase("rc")]
		[TestCase("unstable")]
		[TestCase("pre-release")]
		public void ParsingWithConfiguredTagSuffix(string tagSuffix)
		{
			var commits = new[]
			{
				new CommitInfoWrapper("fix: correct my code")
			};

			var config = new Config
			{
				Tag = tagSuffix
			};
			var parser = new CommitParser(config, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result.ToString(), s => s.Replace("-" + tagSuffix, "-tag-suffix"));
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		[TestCase("0.1.0")]
		[TestCase("0.5.0")]
		[TestCase("1.0.0")]
		[TestCase("5.0.0")]
		public void ParsingWithConfiguredNextVersion(string nextVersion)
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat: awesome feature")
			};

			var config = new Config
			{
				NextVersion = nextVersion
			};
			var parser = new CommitParser(config, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result.ToString(), s => s.Replace(nextVersion + "-", "next-version-"));
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingHigherVersionThanConfigured()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat: some test commit"),
				new CommitInfoWrapper(true, "2.4.5", "docs: update changelog")
			};
			var config = new Config
			{
				NextVersion = "2.0.0"
			};

			var parser = new CommitParser(config, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		[TestCase("alpha")]
		[TestCase("beta")]
		[TestCase("rc")]
		public void ParsingSingleFixCommitWithPreviousTagSuggfixTag(string tagSuffix)
		{
			var commits = new[]
			{
				new CommitInfoWrapper("fix: something"),
				new CommitInfoWrapper(true, $"v1.3.1-{tagSuffix}.2", "docs: update changelog")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result.ToString(), s => s.Replace("-" + tagSuffix, "-tag-suffix"));
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingBreakingFeatureCommit()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat!: this is a major breaking change"),
				new CommitInfoWrapper(true, "v1.2.0", "docs: update changelog")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingBreakingFeatureCommitCommitOnPreStableAndVersionConfigured()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat!: some major break"),
				new CommitInfoWrapper(true, "0.2.0", "docs: update changelog")
			};
			var config = new Config
			{
				NextVersion = "0.1.0"
			};

			var parser = new CommitParser(config, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingFeatureCommitConfiguredWithStableNextVersionAndPreviousNonStable()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat: some kind of new feature"),
				new CommitInfoWrapper(true, "0.2.0", "docs: update changelog")
			};
			var config = new Config
			{
				NextVersion = "1.0.0"
			};

			var parser = new CommitParser(config, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingCommitWithBreakingBody()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat: some new feature\n\nSome body\n\nBREAKING CHANGE: Important breaking change"),
				new CommitInfoWrapper(true, "1.0.0", "docs: update changelog")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingNonBumpingCommits()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("chore: some kind of update"),
				new CommitInfoWrapper("build: build changes"),
				new CommitInfoWrapper(true, "refs/tags/1.3.3", "feat: important new feature"),
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(2);
		}

		[Test]
		public void ParseIgnoresSignedAndAuthoredLines()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(
@"chore(deps)!: bump Cake.Core from 0.33.0 to 1.0.0 (#89)

Bumps [Cake.Core](cake-build/cake) from 0.33.0 to 1.0.0.
- [Release notes](cake-build/cake/releases)
- [Changelog](cake-build/cake@develop/ReleaseNotes.md)
- [Commits](cake-build/cake@v0.33.0...v1.0.0)

BREAKING CHANGE:
This release is to make Cake.Warp compatible with 1.0.0 of Cake.
This means dropping all support for previous versions of Cake.
Please see the [Upgrade instructions](cakebuild.net/docs/getting-started/upgrade#cake-0.38.x-to-cake-1.0) for how to upgrade cake.

Signed-off-by: dependabot-preview[bot] <support@dependabot.com>

Co-authored-by: dependabot-preview[bot] <27856297+dependabot-preview[bot]@users.noreply.github.com>")
			};
			var config = new Config
			{
				TypeScopes = new HashSet<TypeScope>
				{
					new TypeScope("chore", new Description("Dependency", "Dependencies"), "deps")
				}
			};

			var parser = new CommitParser(config, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result.ReleaseNotes);
			result.Version.Commits.ShouldBe(1);
		}
	}
}
