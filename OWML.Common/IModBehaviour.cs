namespace OWML.Common
{
    public interface IModBehaviour
    {
        IModHelper ModHelper { get; }

        void Configure(IOwoConfig config);
    }
}
