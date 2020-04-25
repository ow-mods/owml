using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OWML.ModLoader
{
    class ModSorter
    {
        public static IList<ModDep> SortMods(IList<IModData> mods)
        {
            List<ModDep> modList = new List<ModDep>();
            foreach (var mod in mods)
            {
                if (mod.Manifest.Dependency != "None" && mod.Manifest.Dependency != null)
                {
                    modList.Add(new ModDep(mod.Manifest.Name, mod, mod.Manifest.Dependency));
                }
                else
                {
                    modList.Add(new ModDep(mod.Manifest.Name, mod));
                }
            }

            return Sort(modList, x => x.Dependencies, x => x.Name);
        }

        private static IList<T> Sort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        private static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found.");
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        Visit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }

        private static Func<T, IEnumerable<T>> RemapDependencies<T, TKey>(IEnumerable<T> source, Func<T, IEnumerable<TKey>> getDependencies, Func<T, TKey> getKey)
        {
            var map = source.ToDictionary(getKey);
            return item =>
            {
                var dependencies = getDependencies(item);
                return dependencies != null ? dependencies.Select(key => map[key]) : null;
            };
        }

        private static IList<T> Sort<T, TKey>(IEnumerable<T> source, Func<T, IEnumerable<TKey>> getDependencies, Func<T, TKey> getKey)
        {
            return Sort<T>(source, RemapDependencies(source, getDependencies, getKey));
        }
    }

    public class ModDep
    {
        public string Name { get; private set; }
        public string[] Dependencies { get; private set; }

        public IModData Data { get; private set; }

        public ModDep(string name, IModData data, params string[] dependencies)
        {
            Name = name;
            Data = data;
            Dependencies = dependencies;
        }
    }
}
