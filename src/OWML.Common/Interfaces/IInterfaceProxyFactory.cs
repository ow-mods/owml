namespace OWML.Common.Interfaces
{
    public interface IInterfaceProxyFactory
    {
        TInterface CreateProxy<TInterface>(object api, string manifestUniqueName, string uniqueName) where TInterface : class;
    }
}