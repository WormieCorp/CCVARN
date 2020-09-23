namespace CCVARN.Core.Models
{
	using System.Collections.Generic;
	using Newtonsoft.Json;

	public sealed class ReleaseNotesData
	{
		public List<string> BreakingChanges { get; } = new List<string>();

		public IDictionary<string, List<NoteData>> Notes { get; } = new SortedDictionary<string, List<NoteData>>();
	}
}
