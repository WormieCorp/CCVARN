namespace CCVARN.Core.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class NoteData
	{
		public NoteData(string type, string summary)
			: this(type, summary, string.Empty)
		{
		}

		public NoteData(string type, string summary, string scope)
		{
			Type = type;
			Summary = summary;
			Scope = scope ?? string.Empty;
		}

		public List<int> Issues { get; } = new List<int>();
		public string Scope { get; set; }
		public string Summary { get; set; }
		public string Type { get; set; }

		public static bool operator ==(NoteData a, NoteData b)
			=> Equals(a, b);

		public static bool operator !=(NoteData a, NoteData b)
			=> !(a == b);

		public override bool Equals(object? obj)
		{
			if (!(obj is NoteData data))
				return false;
			var result = Type == data.Type;
			result = result && Scope == data.Scope;
			result = result && Summary == data.Summary;
			result = result && (
				Issues.Count == data.Issues.Count
				&& (Issues.Count == 0 || !Issues.Except(data.Issues).Any()));

			return result;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(
				Type,
				Scope,
				Summary,
				Issues.AsEnumerable());
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Scope))
				return $"{Type}({Scope}): {Summary}";
			else
				return $"{Type}: {Summary}";
		}
	}
}
