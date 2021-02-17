namespace CCVARN.DependencyInject
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using CCVARN.Options;
	using DryIoc;
	using Spectre.Console.Cli;

	public sealed class TypeRegistrar : ITypeRegistrar
	{
		private readonly IContainer container;

		public TypeRegistrar(IContainer container)
		{
			this.container = container;
		}

		public ITypeResolver Build()
		{
			return new TypeResolver(this.container);
		}

		public void Register(Type service, Type implementation)
		{
			this.container.Register(service, implementation, Reuse.Singleton);
		}

		public void RegisterInstance(Type service, object implementation)
		{
			if (service == typeof(BaseSettings))
			{
				this.container.RegisterInstance(typeof(BaseSettings), implementation);
			}
			this.container.RegisterInstance(service, implementation);
		}
	}
}
