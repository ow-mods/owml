using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OWML.ModLoader
{
    class ModSorter
    {
        public IList<ModDep> SortMods(IList<IModData> mods)
        {
            var modList = new List<ModDep>();
            foreach (var mod in mods)
            {
                 modList.Add(new ModDep(mod.Manifest.Name, mod, mod.Manifest.Dependencies));
            }

            return Sort(modList, x => x.Dependencies, x => x.Name);
        }

        private IList<T> Sort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        private void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            var alreadyVisited = visited.TryGetValue(item, out bool inProcess);

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

        private Func<T, IEnumerable<T>> RemapDependencies<T, TKey>(IEnumerable<T> source, Func<T, IEnumerable<TKey>> getDependencies, Func<T, TKey> getKey)
        {
            var map = source.ToDictionary(getKey);
            return item =>
            {
                var dependencies = getDependencies(item);
                return dependencies?.Select(key => map[key]);
            };
        }

        private IList<T> Sort<T, TKey>(IEnumerable<T> source, Func<T, IEnumerable<TKey>> getDependencies, Func<T, TKey> getKey)
        {
            return Sort<T>(source, RemapDependencies(source, getDependencies, getKey));
        }
    }
}
