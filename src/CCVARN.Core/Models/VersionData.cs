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
		public int Major { get; private set; }
		public string? Metadata { get; set; }
		public int Minor { get; private set; }
		public int Patch { get; private set; }
		public string Tag { get; set; } = string.Empty;
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
				data.Tag = versionOrReference.Substring(preIndex + 1, metaIndex - preIndex - 1);
				var weightNum = new StringBuilder();
				for (var i = data.Tag.Length - 1; i >= 0; i--)
				{
					if (!char.IsDigit(data.Tag[i]) && !" .".Any(c => data.Tag[i] == c))
					{
						break;
					}

					if (char.IsDigit(data.Tag[i]))
					{
						weightNum.Insert(0, data.Tag[i]);
					}

					data.Tag = data.Tag[..^1];
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
			if (this._isEmpty)
				this._nextVersionBump = VersionBump.Major;

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
				Tag == other.Tag;
		}

		public override int GetHashCode()
			=> HashCode.Combine(Major, Minor, Patch, Tag);

		public bool IsEmpty()
			=> this._isEmpty;

		public void SetNextBump(VersionBump versionBump)
		{
			if (!_allowMajorBump && versionBump == VersionBump.Major)
				versionBump = VersionBump.Minor;

			if (versionBump > this._nextVersionBump)
				this._nextVersionBump = versionBump;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("{0}.{1}.{2}", Major, Minor, Patch);

			if (!string.IsNullOrEmpty(Tag))
			{
				sb.AppendFormat("-{0}", Tag);
				if (Weight > 0 && Commits > 0)
					sb.AppendFormat(".{0}.{1}", Weight, Commits);
				else if (Weight > 0)
					sb.AppendFormat(".{0}", Weight);
			}

			if (!string.IsNullOrWhiteSpace(Metadata))
				sb.AppendFormat("+{0}", Metadata.Trim());

			return sb.ToString();
		}
	}
}
