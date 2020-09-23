namespace CCVARN.Core.Configuration
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using CCVARN.Core.Models;
	using YamlDotNet.Serialization;

	public sealed class TypeScope
	{
		public TypeScope()
			: this(string.Empty, new Description())
		{
		}
		public TypeScope(string type, Description description, string? scope = null)
		{
			Description = description;
			Scope = scope;
			Type = type;
		}

		public static HashSet<TypeScope> Defaults { get; } = new HashSet<TypeScope>
			{
				new TypeScope("feat", new Description("Feature", "Features"))
				{
					VersionBump = VersionBump.Minor,
				},
				new TypeScope("fix", new Description("Bug fix", "Bug Fixes"))
				{
					VersionBump = VersionBump.Patch,
				},
				new TypeScope("perf", new Description("Performance Improvements"))
				{
					VersionBump = VersionBump.Minor,
				},
				new TypeScope("improvement", new Description("Improvement", "Improvements"))
				{
					VersionBump = VersionBump.Minor,
				},
				new TypeScope("revert", new Description("Reverts")),
				new TypeScope("docs", new Description("Documentation"))
				{
					IncludeInChangelog = false,
				},
				new TypeScope("style", new Description("Style", "Styles"))
				{
					IncludeInChangelog = false,
				},
				new TypeScope("refactor", new Description("Code Refactoring"))
				{
					IncludeInChangelog = false,
				},
				new TypeScope("test", new Description("Test", "Tests"))
				{
					IncludeInChangelog = false,
				},
				new TypeScope("chore", new Description("Chore"))
				{
					IncludeInChangelog = false
				},
				new TypeScope("build", new Description("Build", "Builds"))
				{
					IncludeInChangelog = false,
				},
				new TypeScope("ci", new Description("Continuous Integration"))
				{
					IncludeInChangelog = false,
				},
			};

		public Description Description { get; set; }

		[DefaultValue(true)]
		[YamlMember(Alias = "changelog", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
		public bool IncludeInChangelog { get; set; } = true;

		public string? Scope { get; set; }
		public string Type { get; set; }

		[DefaultValue(VersionBump.None)]
		[YamlMember(Alias = "bump", DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
		public VersionBump VersionBump { get; set; } = VersionBump.None;
	}
}
