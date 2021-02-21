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

	public class CommitParserTests
	{
		private readonly Config defaultConfig = new Config();
		private IRepository repository;
		private IConsoleWriter writer;

		[SetUp]
		public void SetupDependencies()
		{
			this.repository = new Mock<IRepository>().Object;
			this.writer = new Mock<IConsoleWriter>().Object;
		}

		[Test]
		public void ParseSingleFeatureTagCommit()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "1.1.0", "feat: some kind of new feature")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
		}

		[Test]
		public void ParseSingleFixTagCommit()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "1.0.1", "fix: some kind of bugfix"),
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
		}

		[Test]
		public void ParseNonChangelogCommit()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "1.0.1", "chore: some update"),
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
		}

		[Test]
		public void ParsingMultipeCommitTypes()
		{
			var commits = new[]
			{
				new CommitInfoWrapper("feat: some kind of new feature"),
				new CommitInfoWrapper("fix(scoping): some kind of bug fix"),
				new CommitInfoWrapper("refactor!: renamed a method in the public API")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
		}

		[Test]
		public void ParsingBreakingChangeAndBody()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "2.0.0", "chore: some update"),
				new CommitInfoWrapper(
@"refactor!: renamed a method in the public API

AweSomeName was renamed to AwesomeNameAsync"
				)
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
		}

		[Test]
		public void ParsingBreakingChangeInBodyWithFeatureTag()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "2.0.0", "feat: some update"),
				new CommitInfoWrapper(
@"feat(scopy): some new breaking feature

This is a normal body

BREAKING CHANGE: This is the breaking message
So be it"
				)
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
		}

		[Test]
		[TestCase("close")]
		[TestCase("closes")]
		[TestCase("closed")]
		[TestCase("fix")]
		[TestCase("fixes")]
		[TestCase("fixed")]
		[TestCase("resolve")]
		[TestCase("resolves")]
		[TestCase("resolved")]
		public void ParsingCommitsWithIssueReferences(string closeType)
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "1.0.0", "docs: update changelog"),
				new CommitInfoWrapper($"feat: some feature\n\n{closeType} #54\n{closeType} #43"),
				new CommitInfoWrapper($"chore: some chore\n\n{closeType} #22")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
		}

		[Test]
		public void ParsingMergeCommits()
		{
			var commits = new[]
			{
				new CommitInfoWrapper(true, "1.0.0", "docs: update changelog"),
				new CommitInfoWrapper("Merge develop into master"),
				new CommitInfoWrapper("feat: some feature"),
				new CommitInfoWrapper("(build) not considered")
			};

			var parser = new CommitParser(this.defaultConfig, this.repository, this.writer);

			var result = parser.ParseVersionFromCommits(commits);

			Approvals.Verify(result);
		}
	}
}
