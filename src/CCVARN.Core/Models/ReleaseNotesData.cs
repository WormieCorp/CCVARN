namespace CCVARN.Core.Models
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public sealed class ReleaseNotesData
	{
		public List<string> BreakingChanges { get; } = new List<string>();

		public IDictionary<string, List<NoteData>> Notes { get; } = new SortedDictionary<string, List<NoteData>>();

		public override string ToString()
		{
			var sb = new StringBuilder();

			if (BreakingChanges.Any())
			{
				sb.AppendLine("BREAKING NOTES");
				foreach (var line in BreakingChanges)
				{
					sb.AppendLine(line);
				}
				sb.AppendLine();
			}

			foreach (var note in Notes)
			{
				sb.Append(note.Key)
					.AppendLine(": ");
				foreach (var noteData in note.Value)
				{
					sb.Append('\t').Append(noteData).AppendLine();
				}
			}

			return sb.ToString();
		}
	}
}
