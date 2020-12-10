using System.Collections.Generic;

namespace OWML.Common
{
    public interface IModBehaviour
    {
        IModHelper ModHelper { get; }

        object Api { get; }

        void Configure(IModConfig config);

        IList<IModBehaviour> GetDependants();

        IList<IModBehaviour> GetDependencies();

        object GetApi();

        void Init(IModHelper helper);
    }
}
