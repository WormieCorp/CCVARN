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
	}
}
