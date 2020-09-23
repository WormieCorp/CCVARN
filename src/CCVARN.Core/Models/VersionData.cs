namespace CCVARN.Core.Models
{
	using System;
	using System.Linq;
	using System.Text;

	public class VersionData : IEquatable<VersionData>
	{
		private const string TAG_REFERENCE_PREFIX = "refs/tags/";
		private static bool _allowMajorBump = true;
		private bool _isEmpty = true;
		private VersionBump _nextVersionBump;

		public int Commits { get; set; }
		public string FullSemVer => ToString();
		public int Major { get; private set; }
		public string MajorMinorPatch => $"{Major}.{Minor}.{Patch}";
		public string? Metadata { get; set; }
		public int Minor { get; private set; }
		public int Patch { get; private set; }
		public string PreReleaseLabel { get; set; } = string.Empty;
		public string PreReleaseTag => BuildPreReleaseTag();
		public string SemVer => ToString(false);
		public int? Weight { get; set; }

		public static void DisableMajorBump()
			=> _allowMajorBump = false;

		public static VersionData Parse(string versionOrReference, VersionData? oldVersion = null)
		{
			var newVersionSb = new StringBuilder(versionOrReference);
			if (versionOrReference.StartsWith(TAG_REFERENCE_PREFIX, StringComparison.OrdinalIgnoreCase))
			{
				newVersionSb.Remove(0, TAG_REFERENCE_PREFIX.Length);
			}

			while (newVersionSb.Length > 0 && !char.IsDigit(newVersionSb[0]))
				newVersionSb.Remove(0, 1);

			if (newVersionSb.Length == 0)
			{
				return new VersionData()
				{
					_isEmpty = true,
					_nextVersionBump = oldVersion?._nextVersionBump ?? VersionBump.None,
				};
			}

			var data = new VersionData
			{
				_nextVersionBump = oldVersion?._nextVersionBump ?? VersionBump.None,
			};

			var preIndex = versionOrReference.IndexOf('-');
			var metaIndex = versionOrReference.IndexOf('+');
			if (metaIndex == -1)
				metaIndex = versionOrReference.Length;
			if (preIndex > 0 && preIndex < metaIndex)
			{
				data.PreReleaseLabel = versionOrReference.Substring(preIndex + 1, metaIndex - preIndex - 1);
				var weightNum = new StringBuilder();
				for (var i = data.PreReleaseLabel.Length - 1; i >= 0; i--)
				{
					if (!char.IsDigit(data.PreReleaseLabel[i]) && !" .".Any(c => data.PreReleaseLabel[i] == c))
					{
						break;
					}

					if (char.IsDigit(data.PreReleaseLabel[i]))
					{
						weightNum.Insert(0, data.PreReleaseLabel[i]);
					}

					data.PreReleaseLabel = data.PreReleaseLabel[..^1];
				}

				if (weightNum.Length > 0)
				{
					data.Weight = int.Parse(weightNum.ToString());
				}
				else
				{
					data.Weight = 0;
				}

				var startIndex = preIndex - (versionOrReference.Length - newVersionSb.Length);
				var endIndex = newVersionSb.Length - startIndex;
				newVersionSb.Remove(startIndex, endIndex);
				data._nextVersionBump = VersionBump.Weight;
			}
			else if (metaIndex < versionOrReference.Length)
			{
				var startIndex = metaIndex - (versionOrReference.Length - newVersionSb.Length);
				var endIndex = newVersionSb.Length - startIndex;
				newVersionSb.Remove(startIndex, endIndex);
			}

			if (Version.TryParse(newVersionSb.ToString(), out var version))
			{
				data.Major = Math.Max(version.Major, 0);
				data.Minor = Math.Max(version.Minor, 0);
				data.Patch = Math.Max(version.Build, 0);
				data._isEmpty = false;
			}

			return data;
		}

		public void CommitNextBump()
		{
			if (_allowMajorBump && this._isEmpty)
				this._nextVersionBump = VersionBump.Major;
			else if (!_allowMajorBump && this._nextVersionBump == VersionBump.Major)
				this._nextVersionBump = VersionBump.Minor;

			switch (this._nextVersionBump)
			{
				case VersionBump.Patch:
					Patch++;
					break;

				case VersionBump.Minor:
					Patch = 0;
					Minor++;
					break;

				case VersionBump.Major:
					Patch = 0;
					Minor = 0;
					Major++;
					break;

				case VersionBump.Weight:
					Weight = (Weight is null) ? 1 : Weight + 1;
					break;
			}

			this._isEmpty = false;
			this._nextVersionBump = VersionBump.None;
		}

		public override bool Equals(object? obj)
			=> obj is VersionData data && Equals(data);

		public bool Equals(VersionData other)
		{
			return
				Major == other.Major &&
				Minor == other.Minor &&
				Patch == other.Patch &&
				PreReleaseLabel == other.PreReleaseLabel;
		}

		public override int GetHashCode()
			=> HashCode.Combine(Major, Minor, Patch, PreReleaseLabel);

		public bool IsEmpty()
			=> this._isEmpty;

		public void SetNextBump(VersionBump versionBump)
		{
			if (versionBump > this._nextVersionBump)
				this._nextVersionBump = versionBump;
		}

		public override string ToString()
			=> ToString(true);

		public string ToString(bool includeMetadata)
		{
			var sb = new StringBuilder();
			sb.Append(MajorMinorPatch);

			if (!string.IsNullOrEmpty(PreReleaseLabel))
			{
				sb.AppendFormat("-{0}", PreReleaseTag);
			}

			if (includeMetadata && !string.IsNullOrWhiteSpace(Metadata))
				sb.AppendFormat("+{0}", Metadata.Trim());

			return sb.ToString();
		}

		private string BuildPreReleaseTag()
		{
			var sb = new StringBuilder();
			if (!string.IsNullOrEmpty(PreReleaseLabel))
			{
				sb.AppendFormat("{0}", PreReleaseLabel);
				if (Weight > 0 && Commits > 0)
					sb.AppendFormat(".{0}.{1}", Weight, Commits);
				else if (Weight > 0)
					sb.AppendFormat(".{0}", Weight);
			}

			return sb.ToString();
		}
	}
}
