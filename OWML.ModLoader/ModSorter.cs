using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OWML.ModLoader
{
    public class ModSorter
    {
        private readonly IModConsole _console;

        public ModSorter(IModConsole console)
        {
            _console = console;
        }

        public IList<IModData> SortMods(IList<IModData> mods)
        {
            var modDict = new Dictionary<string, IModData>();
            var modList = new List<string>();
            var set = new HashSet<Edge>();

            foreach (var mod in mods)
            {
                modList.Add(mod.Manifest.UniqueName);
            }

            foreach (var mod in mods)
            {
                modDict.Add(mod.Manifest.UniqueName, mod);

                foreach (var dependency in mod.Manifest.Dependencies)
                {
                    if (mod.Manifest.PriorityLoad && !modList.Contains(dependency))
                    {
                        _console.WriteLine($"Error! {mod.Manifest.UniqueName} (priority load) depends on a normal mod! Removing from load...");
                        modDict.Remove(mod.Manifest.UniqueName);
                        modList.Remove(mod.Manifest.UniqueName);
                    }
                    else
                    {
                        set.Add(new Edge(mod.Manifest.UniqueName, dependency));
                    }
                }
            }


            var sortedList = TopologicalSort(
                new HashSet<string>(modList),
                new HashSet<Edge>(set)
            );

            if (sortedList == null)
            {
                // Sorting has failed, return the original mod list
                _console.WriteLine("Error - Cyclic dependency found. Returning original load order...");
                return mods;
            }

            sortedList.Reverse();

            return sortedList.Select(mod => modDict[mod]).ToList();
        }

        // Thanks to https://gist.github.com/Sup3rc4l1fr4g1l1571c3xp14l1d0c10u5/3341dba6a53d7171fe3397d13d00ee3f

        static List<string> TopologicalSort(HashSet<string> nodes, HashSet<Edge> edges)
        {
            var sortedList = new List<string>();

            var nodesWithNoEdges = new HashSet<string>(nodes.Where(node => edges.All(edge => edge.Second.Equals(node) == false)));

            while (nodesWithNoEdges.Any())
            {
                var firstNode = nodesWithNoEdges.First();
                nodesWithNoEdges.Remove(firstNode);

                sortedList.Add(firstNode);

                foreach (var edge in edges.Where(e => e.First.Equals(firstNode)).ToList())
                {
                    var secondNode = edge.Second;

                    edges.Remove(edge);

                    if (edges.All(mEdge => mEdge.Second.Equals(secondNode) == false))
                    {
                        nodesWithNoEdges.Add(secondNode);
                    }
                }
            }

            if (edges.Any())
            {
                // This will be caught and handled in the caller method
                return null;
            }
            else
            {
                return sortedList;
            }
        }
    }

    public class Edge
    {
        public string First { get; private set; }
        public string Second { get; private set; }
        internal Edge(string first, string second)
        {
            this.First = first;
            this.Second = second;
        }
    }
}
