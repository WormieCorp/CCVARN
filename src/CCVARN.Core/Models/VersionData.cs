namespace CCVARN.Core.Models
{
	using System;
	using System.Globalization;
	using System.Linq;
	using System.Text;

	public class VersionData : IEquatable<VersionData>
	{
		private const string TAG_REFERENCE_PREFIX = "refs/tags/";
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

		public static VersionData Parse(string versionOrReference, VersionData? oldVersion = null)
		{
			if (versionOrReference is null)
			{
				return new VersionData()
				{
					_isEmpty = true,
					_nextVersionBump = oldVersion?._nextVersionBump ?? VersionBump.None,
				};
			}

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

			var preIndex = versionOrReference.IndexOf('-', StringComparison.Ordinal);
			var metaIndex = versionOrReference.IndexOf('+', StringComparison.Ordinal);
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

					if (i > 0 && data.PreReleaseLabel[i] == '.' && char.IsDigit(data.PreReleaseLabel[i - 1]))
						weightNum.Clear();
					else if (char.IsDigit(data.PreReleaseLabel[i]))
						weightNum.Insert(0, data.PreReleaseLabel[i]);

					data.PreReleaseLabel = data.PreReleaseLabel[..^1];
				}

				if (weightNum.Length > 0)
				{
					data.Weight = int.Parse(weightNum.ToString(), CultureInfo.InvariantCulture);
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

		public bool CommitNextBump(bool allowMajorBump = true)
		{
			if (this._nextVersionBump == VersionBump.None)
				return false;

			if (allowMajorBump && this._isEmpty)
				this._nextVersionBump = VersionBump.Major;
			else if (!allowMajorBump && this._nextVersionBump == VersionBump.Major)
				this._nextVersionBump = VersionBump.Minor;

			var isBumped = false;

			switch (this._nextVersionBump)
			{
				case VersionBump.Patch:
					Patch++;
					isBumped = true;
					break;

				case VersionBump.Minor:
					Patch = 0;
					Minor++;
					isBumped = true;
					break;

				case VersionBump.Major:
					Patch = 0;
					Minor = 0;
					Major++;
					isBumped = true;
					break;

				case VersionBump.Weight:
					Weight = (Weight is null) ? 1 : Weight + 1;
					break;
			}

			this._isEmpty = false;
			this._nextVersionBump = VersionBump.None;

			return isBumped;
		}

		public override bool Equals(object? obj)
			=> obj is VersionData data && Equals(data);

		public bool Equals(VersionData? other)
		{
			if (other is null)
				return false;

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

		public void SetNextBump(VersionBump versionBump, bool forceBump = false)
		{
			if (versionBump > this._nextVersionBump || forceBump)
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
				sb.AppendFormat(CultureInfo.InvariantCulture, "-{0}", PreReleaseTag);
			}

			if (includeMetadata && !string.IsNullOrWhiteSpace(Metadata))
				sb.AppendFormat(CultureInfo.InvariantCulture, "+{0}", Metadata.Trim());

			return sb.ToString();
		}

		private string BuildPreReleaseTag()
		{
			var sb = new StringBuilder();
			if (!string.IsNullOrEmpty(PreReleaseLabel))
			{
				sb.AppendFormat(CultureInfo.InvariantCulture, "{0}", PreReleaseLabel);
				if (Weight > 0 && Commits > 0)
					sb.AppendFormat(CultureInfo.InvariantCulture, ".{0}.{1}", Weight, Commits);
				else if (Weight > 0)
					sb.AppendFormat(CultureInfo.InvariantCulture, ".{0}", Weight);
			}

			return sb.ToString();
		}
	}
}
