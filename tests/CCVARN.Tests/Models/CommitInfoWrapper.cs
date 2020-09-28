namespace CCVARN.Tests.Models
{
	using System.Security.Cryptography;
	using System.Text;
	using CCVARN.Core.Models;

	public class CommitInfoWrapper : CommitInfo
	{
		public CommitInfoWrapper(string rawText)
			: this(false, null, rawText)
		{
		}

		public CommitInfoWrapper(bool isTag, string @ref)
				: this(isTag, @ref, string.Empty)
		{
		}

		public CommitInfoWrapper(bool isTag, string @ref, string rawText)
			: base(
				CreateSha((string.IsNullOrEmpty(rawText)) ? @ref : rawText),
				rawText ?? string.Empty)
		{
			IsTag = isTag;
			Ref = @ref;
		}

		private static string CreateSha(string text)
		{
			using var sha = SHA1.Create();
			var sb = new StringBuilder();

			foreach (var b in sha.ComputeHash(Encoding.UTF8.GetBytes(text)))
			{
				sb.AppendFormat("{0:x2}", b);
			}

			return sb.ToString();
		}
	}
}
