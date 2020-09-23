namespace CCVARN.Core.Configuration
{
	using System;
	using YamlDotNet.Serialization;

	public sealed class Description
	{
		public Description()
			: this(string.Empty)
		{
		}

		public Description(string singular, string? plural = null)
		{
			Singular = singular;
			Plural = plural;
		}

		[YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitNull)]
		public string? Plural { get; set; }
		public string Singular { get; set; }

		public override bool Equals(object? obj)
		{
			return obj is Description description &&
				   Plural == description.Plural &&
				   Singular == description.Singular;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Plural, Singular);
		}

		public override string ToString()
		{
			return $"{Singular} ({Plural})";
		}
	}
}
