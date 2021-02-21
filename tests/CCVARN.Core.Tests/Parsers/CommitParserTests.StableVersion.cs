namespace CCVARN.Core.Tests.Parsers
{
	using System.Security.Cryptography.Pkcs;
	using ApprovalTests;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.IO;
	using CCVARN.Core.Parser;
	using CCVARN.Tests.Models;
	using LibGit2Sharp;
	using Moq;
	using NUnit.Framework;
	using Shouldly;

	public partial class CommitParserTests
	{
		[Test]
		public void ParsingTagWithPastFeatureCommits()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "1.0.0", "docs: update changelog"),
				new CommitInfoWrapper("feat: some kind of awesome new feature"),
				new CommitInfoWrapper("chore: just some non-source maintainance")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(3);
		}

		[Test]
		public void ParsingTagWithoutPreviousCommits()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "1.5.2", "feat: some feature")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingTagWithLowerVersionThanPreviousTag()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "2.1.0", "fix: major breaking change"),
				new CommitInfoWrapper(true, "3.0.0", "feat!: previous breaking")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingCommitWhenConfigTagSuffixIsEmpty()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat: some kind of feature"),
				new CommitInfoWrapper(true, "1.5.2", "fix: something")
			};

			var config = new Config
			{
				Tag = string.Empty,
			};
			var parser = new CommitParser(config, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingCommitWithEmptyTagSuffixAndNoPreviousTag()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat: some kind of feature")
			};

			var config = new Config
			{
				Tag = string.Empty
			};
			var parser = new CommitParser(config, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingNonBumpingCommitsSinceTag()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("chore: something"),
				new CommitInfoWrapper("build: build related only"),
				new CommitInfoWrapper(true, "1.7.0", "feat: awesome")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(2);
		}

		[Test]
		public void ParsingTagWithVPrefix()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "v.5.2.2", "fix: something")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingMergeCommitAfterTag()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("Merge master into develop"),
				new CommitInfoWrapper(true, "1.3.5", "feat: awesome")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(1);
		}

		[Test]
		public void ParsingTaggedMergeCommit()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "0.3.0", "Merge branch 'release/0.3.0' into master"),
				new CommitInfoWrapper("feat: include embedded pdb files to fully enable deterministic builds for addin"),
				new CommitInfoWrapper(true, "0.2.0", "Merge branch 'release/0.2.0' into master")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
			result.Version.Commits.ShouldBe(2);
		}
	}
}
