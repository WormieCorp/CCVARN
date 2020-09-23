namespace CCVARN.Core.Configuration
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using YamlDotNet.Serialization;

	public sealed class Config
	{
		[Description("The major-minor-patch version that should be used when parsing the commits.\n" +
			"This option will only be taken into account when the current HEAD is not a tagged\n" +
			"build, and the value is higher than the last tagged release.")]
		public string? NextVersion { get; set; }

		[Description("The pre-release tag to use as a default for non-tagged releases (tagged releases will override this option).")]
		public string Tag { get; set; } = "alpha";

		[Description("The types and scope that should be consider valid, as well as which part of the version to bump when the type+scope matches.\n" +
			"Additionally the headers in the changelog can be configured,\n" +
			"and wether the types+scope should be included in the changelog by default\n" +
			"(breaking changes will always be included in the changelog).")]
		[YamlMember(Alias = "types")]
		public HashSet<TypeScope> TypeScopes { get; set; } = TypeScope.Defaults;
	}
}
