namespace CCVARN.Core.Configuration.Comments
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using YamlDotNet.Core;
	using YamlDotNet.Serialization;
	using YamlDotNet.Serialization.TypeInspectors;

	internal sealed class CommentGatheringTypeInspector : TypeInspectorSkeleton
	{
		private readonly ITypeInspector innerTypeDescriptor;

		public CommentGatheringTypeInspector(ITypeInspector innerTypeDescriptor)
		{
			this.innerTypeDescriptor = innerTypeDescriptor ?? throw new ArgumentNullException(nameof(innerTypeDescriptor));
		}

		public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
		{
			return this.innerTypeDescriptor
				.GetProperties(type, container)
				.Select(d => new CommentsPropertyDescriptor(d));
		}

		private sealed class CommentsPropertyDescriptor : IPropertyDescriptor
		{
			private readonly IPropertyDescriptor baseDescriptor;

			public CommentsPropertyDescriptor(IPropertyDescriptor baseDescriptor)
			{
				this.baseDescriptor = baseDescriptor;
				Name = baseDescriptor.Name;
			}

			public bool CanWrite => this.baseDescriptor.CanWrite;
			public string Name { get; set; }
			public int Order { get; set; }

			public ScalarStyle ScalarStyle
			{
				get => this.baseDescriptor.ScalarStyle;
				set => this.baseDescriptor.ScalarStyle = value;
			}

			public Type Type => this.baseDescriptor.Type;

			public Type? TypeOverride
			{
				get => this.baseDescriptor.TypeOverride;
				set => this.baseDescriptor.TypeOverride = value;
			}

			public T GetCustomAttribute<T>()
				where T : Attribute
				=> this.baseDescriptor.GetCustomAttribute<T>();

			public IObjectDescriptor Read(object target)
			{
				var description = GetCustomAttribute<DescriptionAttribute>();
				return (description is null) ?
					this.baseDescriptor.Read(target) :
					new CommentsObjectDescriptor(this.baseDescriptor.Read(target), description.Description);
			}

			public void Write(object target, object? value)
										=> this.baseDescriptor.Write(target, value);
		}
	}
}
