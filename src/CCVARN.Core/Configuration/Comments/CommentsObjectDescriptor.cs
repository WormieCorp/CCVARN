namespace CCVARN.Core.Configuration.Comments
{
	using System;
	using YamlDotNet.Core;
	using YamlDotNet.Serialization;

	internal sealed class CommentsObjectDescriptor : IObjectDescriptor
	{
		private readonly IObjectDescriptor innerDescriptor;

		public CommentsObjectDescriptor(IObjectDescriptor innerDescriptor, string comment)
		{
			this.innerDescriptor = innerDescriptor;
			Comment = comment;
		}

		public string Comment { get; }

		public ScalarStyle ScalarStyle => this.innerDescriptor.ScalarStyle;
		public Type StaticType => this.innerDescriptor.StaticType;
		public Type Type => this.innerDescriptor.Type;
		public object? Value => this.innerDescriptor.Value;
	}
}
