namespace CCVARN.Core.Models
{
	using System.Collections.Generic;
	using CCVARN.Core.Configuration;

	public sealed class ReleaseNotesData
	{
		public List<string> BreakingChanges { get; } = new List<string>();
		public IDictionary<Description, List<NoteData>> ReleaseNotes { get; } = new Dictionary<Description, List<NoteData>>();
	}
}
