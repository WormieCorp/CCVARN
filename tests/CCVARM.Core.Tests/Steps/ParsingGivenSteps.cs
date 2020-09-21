using LibGit2Sharp;
using Moq;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TechTalk.SpecFlow;
using Version = System.Version;

namespace CCVARM.Core.Tests.Steps
{
    [Binding]
    public class ParsingGivenSteps : IDisposable
    {
        private readonly ScenarioContext scenarioContext;
        private IRepository repository;
        private Mock<TagCollection> tags;
        private Mock<IQueryableCommitLog> commits;

        public ParsingGivenSteps(ScenarioContext context)
        {
            this.scenarioContext = context;
            repository = new Repository(@"E:\Docs\git\organizations\WormieCorp\generator-cake-addin");
        }

        [Given(@"a repository without any commits?")]
        [Given(@"without any commits?")]
        public void GivenARepositoryWithoutAnyCommits()
        {
        }

        [Given(@"a repository without any tags?")]
        [Given(@"without any tags?")]
        public void GivenARepositoryWithoutAnyTags()
        {
        }

        [Given(@"a repository with (\d+) commits?")]
        [Given(@"a repository with (\d+) commits? since last tag")]
        public void GivenAGitRepositoryWithCommits(int count, Table table)
        {
        }

        [Given(@"a repository with the tag v?([\d\.]+)")]
        public void GivenARepositoryWithTheTag(Version version)
        {
        }

        [When(@"the user execute version parsing")]
        public void WhenTheUserExecuteVersionParsing()
        {
            Version tagVersion = null;
            string bumpType = "none";
            var tags = this.repository.Tags.ToList();
            int skip = 0;
            var current = this.repository.Head.Tip;
            var tag = tags.FirstOrDefault(tag => current.Sha == tag.Target.Sha);

            if (tag is object)
            {
                skip = 1;
            }

            foreach (var commit in this.repository.Head.Commits.Skip(skip))
            {
                var newTag = tags.FirstOrDefault(t => t.Target.Sha == commit.Sha);
                if (newTag is object)
                {
                    if (tag is null)
                    {
                        tag = newTag;
                    }
                    break;
                }
                else if (tag is null)
                {
                    if (commit.Message.StartsWith("!:"))
                        bumpType = "major";
                    else if (commit.Message.StartsWith("feat:"))
                        bumpType = "minor";
                    else if (commit.Message.StartsWith("fix:"))
                        bumpType = "patch";
                }
            }

            if (tag is object)
            {
                var @ref = tag.CanonicalName;
                var ver = @ref.Substring("refs/tags".Length + 1);
                tagVersion = Version.Parse(ver.TrimStart('v', '.'));

                if (bumpType == "patch")
                {
                    tagVersion = new Version(tagVersion.Major, tagVersion.Minor, tagVersion.Build + 1);
                }
                else if (bumpType == "minor" || (bumpType == "major" && tagVersion.Major == 0))
                {
                    tagVersion = new Version(tagVersion.Major, tagVersion.Minor + 1, tagVersion.Build);
                }
                else if (bumpType == "major")
                {
                    tagVersion = new Version(tagVersion.Major, tagVersion.Major, tagVersion.Build);
                }
            }
            else
            {
                tagVersion = new Version("1.0.0");
            }

            this.scenarioContext["Parsed_Version"] = tagVersion;
        }

        private static string CreateSha(string text)
        {
            using var algo = SHA1.Create();
            var sb = new StringBuilder();

            foreach (var b in algo.ComputeHash(Encoding.UTF8.GetBytes(text)))
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }

        public void Dispose()
        {
            repository?.Dispose();
            repository = null;
        }
    }
}