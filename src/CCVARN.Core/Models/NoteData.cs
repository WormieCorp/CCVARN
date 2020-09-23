namespace CCVARN.Core.Models
{
	using System;
	using System.Collections.Generic;

	public sealed class NoteData
	{
		public NoteData(string type, string summary)
		{
			Type = type;
			Summary = summary;
		}

		public List<int> Issues { get; set; } = new List<int>();
		public string? Scope { get; set; }
		public string Summary { get; set; }
		public string Type { get; set; }

		public override bool Equals(object? obj)
		{
			return obj is NoteData data &&
				   Type == data.Type &&
				   Scope == data.Scope &&
				   Summary == data.Summary &&
				   EqualityComparer<List<int>>.Default.Equals(Issues, data.Issues);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(
				Type,
				Scope,
				Summary,
				Issues);
		}
	}
}
