namespace OWML.Common
{
    public interface IModBehaviour
    {
        IModHelper ModHelper { get; }
        void Configure(IModConfig config);
    }
}
