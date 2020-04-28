using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModBehaviour
    {
        IModHelper ModHelper { get; }
        void Configure(IModConfig config);
        IList<IModBehaviour> GetDependants();
        IList<IModBehaviour> GetDependencies();
    }
}
