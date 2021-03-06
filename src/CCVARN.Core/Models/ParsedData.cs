namespace CCVARN.Core.Models
{
	using System;
	using System.Collections.Generic;

	public class ParsedData
	{
		public ParsedData(VersionData version, ReleaseNotesData releaseNotes)
		{
			Version = version;
			ReleaseNotes = releaseNotes;
		}

		public VersionData Version { get; set; }

		public ReleaseNotesData ReleaseNotes { get; }

		public override string ToString()
		{
			return $"ParsedData\n{Version}\n{ReleaseNotes}";
		}
	}
}
