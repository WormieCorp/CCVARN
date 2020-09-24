namespace CCVARN.Tests.Models
{
	using System.Security.Cryptography;
	using System.Text;
	using CCVARN.Core.Models;

	public class ConventionalCommitInfoWrapper : ConventionalCommitInfo
	{
		public ConventionalCommitInfoWrapper(string type, string scope, string message)
			: this(
				type,
				scope,
				message,
				type + (string.IsNullOrEmpty(scope) ? ": " : $"({scope}): ") + message)
		{
		}

		private ConventionalCommitInfoWrapper(string type, string scope, string message, string rawText)
			: base(CreateSha(rawText), rawText, type, scope, message)
		{
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
