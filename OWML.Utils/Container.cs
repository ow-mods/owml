using Unity;
using Unity.Lifetime;

namespace OWML.Utils
{
	public class Container
	{
		private readonly IUnityContainer _container = new UnityContainer();

		public Container Add<TInterface>(TInterface instance)
		{
			_container.RegisterInstance(instance, new ContainerControlledLifetimeManager());
			return this;
		}

		public Container Add<TInterface, TImplementation>() where TImplementation : TInterface
		{
			_container.RegisterType<TInterface, TImplementation>(new ContainerControlledLifetimeManager());
			return this;
		}

		public Container Add<TImplementation>()
		{
			_container.RegisterType<TImplementation>(new ContainerControlledLifetimeManager());
			return this;
		}

		public T Resolve<T>() =>
			_container.Resolve<T>();
	}
}