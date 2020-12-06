using Autofac;

namespace OWML.Utils
{
    public class Container
    {
        private readonly ContainerBuilder _builder;
        private IContainer _container;

        public Container()
        {
            _builder = new ContainerBuilder();
        }

        public Container Register<TInterface>(TInterface instance) where TInterface : class
        {
            _builder.RegisterInstance(instance).As<TInterface>();
            return this;
        }

        public Container Register<TInterface, TImplementation>()
        {
            _builder.RegisterType<TImplementation>().As<TInterface>();
            return this;
        }

        public Container Register<TImplementation>()
        {
            _builder.RegisterType<TImplementation>();
            return this;
        }

        public Container Build()
        {
            _container = _builder.Build();
            return this;
        }

        public T Resolve<T>()
        {
            if (_container == null)
            {
                Build();
            }

            return _container.Resolve<T>();
        }
    }
}
