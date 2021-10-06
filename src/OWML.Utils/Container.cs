using Autofac;

namespace OWML.Utils
{
	public class Container
	{
		private readonly ContainerBuilder _builder = new();

		public Container Add<TInterface>(TInterface instance)
			where TInterface : class
		{
			_builder.RegisterInstance(instance).SingleInstance();
			return this;
		}

		public Container Add<TInterface, TImplementation>() where TImplementation : TInterface
		{
			_builder.RegisterType<TImplementation>().As<TInterface>().SingleInstance();
			return this;
		}

		public Container Add<TImplementation>()
		{
			_builder.RegisterType<TImplementation>().SingleInstance();
			return this;
		}

		public T Resolve<T>() =>
			_builder.Build().Resolve<T>();
	}
}
