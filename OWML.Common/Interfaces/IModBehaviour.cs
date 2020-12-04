using System.Collections.Generic;

namespace OWML.Common.Interfaces
{
    public interface IModBehaviour
    {
        IModHelper ModHelper { get; }
        object Api { get; }
        void Configure(IModConfig config);
        IList<IModBehaviour> GetDependants();
        IList<IModBehaviour> GetDependencies();
        object GetApi();
    }
}
