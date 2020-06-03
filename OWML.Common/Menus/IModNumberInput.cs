#pragma warning disable 1591

namespace OWML.Common.Menus
{
    public interface IModNumberInput : IModInput<float>
    {
        IModNumberInput Copy();
        IModNumberInput Copy(string key);
    }
}
