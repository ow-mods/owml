using Autofac;

namespace OWML.Utils
{
    public class Container
    {
        private readonly ContainerBuilder _builder = new ContainerBuilder();

        public Container Add<TInterface>(TInterface instance)
            where TInterface : class
        {
            _builder.RegisterInstance(instance); // todo singletons?
            return this;
        }

        public Container Add<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _builder.RegisterType<TImplementation>().As<TInterface>();
            return this;
        }

        public Container Add<TImplementation>()
        {
            _builder.RegisterType<TImplementation>();
            return this;
        }

        public T Resolve<T>() =>
            _builder.Build().Resolve<T>();
    }
}
