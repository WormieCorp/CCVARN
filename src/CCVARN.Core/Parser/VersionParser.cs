namespace CCVARN.Core.Parser
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using CCVARN.Core.Configuration;
	using CCVARN.Core.Models;

	internal sealed class VersionParser
	{
		private readonly HashSet<TypeScope> types;

		public VersionParser()
			: this(TypeScope.Defaults)
		{
		}

		public VersionParser(HashSet<TypeScope> types)
		{
			this.types = types;
		}

		public VersionData ParseVersion(VersionData? version, CommitInfo commit)
		{
			if (commit is null)
				throw new ArgumentNullException(nameof(commit));

			VersionData newVersion;
			if (version is null || (commit.IsTag && version.IsEmpty()))
				newVersion = ParseVersionFromTag(commit, version) ?? version ?? new VersionData();
			else
				newVersion = version ?? new VersionData();

			if (commit.IsTag)
				return newVersion; // We should not parse the commit for version increment on tagged releases

			if (commit is not ConventionalCommitInfo conventionalCommit)
				return newVersion;

			if (conventionalCommit.IsBreakingChange)
			{
				newVersion.SetNextBump(VersionBump.Major);
			}
			else
			{
				var typeConfig = this.types.FirstOrDefault(t => string.Equals(t.Type, conventionalCommit.CommitType, StringComparison.OrdinalIgnoreCase));
				var typeScopeConfig = this.types.FirstOrDefault(t => string.Equals(t.Type, conventionalCommit.CommitType, StringComparison.OrdinalIgnoreCase) &&
					string.Equals(t.Scope, conventionalCommit.CommitScope, StringComparison.OrdinalIgnoreCase));

				if (typeScopeConfig != null)
					newVersion.SetNextBump(typeScopeConfig.VersionBump);
				else if (typeConfig != null)
					newVersion.SetNextBump(typeConfig.VersionBump);
			}

			return newVersion;
		}

		private static VersionData? ParseVersionFromTag(CommitInfo commit, VersionData? oldVersion)
		{
			if (!commit.IsTag || commit.Ref is null)
				return null;

			var data = VersionData.Parse(commit.Ref, oldVersion);

			return data;
		}
	}
}
