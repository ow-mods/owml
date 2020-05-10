using OWML.Common;

namespace OWML.ModLoader
{
    public class ModDependency
    {
        public string Name { get; private set; }
        public string[] Dependencies { get; private set; }

        public IModData Data { get; private set; }

        public ModDependency(string name, IModData data, params string[] dependencies)
        {
            Name = name;
            Data = data;
            Dependencies = dependencies;
        }
    }
}
