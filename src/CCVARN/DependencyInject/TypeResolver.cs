namespace CCVARN.DependencyInject
{
	using System;
	using System.Linq;
	using CCVARN.Commands;
	using CCVARN.Options;
	using DryIoc;
	using Spectre.Console.Cli;

	public sealed class TypeResolver : ITypeResolver
	{
		private readonly IContainer container;

		public TypeResolver(IContainer container)
		{
			this.container = container;
		}

		public object? Resolve(Type? type)
		{
			var result = this.container.Resolve(type);

			if (type.GetImplementedTypes().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BaseCommand<>)))
			{
				this.container.InjectPropertiesAndFields(result);
			}

			return result;
		}
	}
}
