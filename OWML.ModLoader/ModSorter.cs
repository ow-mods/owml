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
                // Add to list of uniqueNames
                modList.Add(mod.Manifest.UniqueName);
            }

            foreach (var mod in mods)
            {
                // Add to dict of (uniqueName, IModData)
                modDict.Add(mod.Manifest.UniqueName, mod);

                // Add to hashset of tuples (Dependant : Dependency)
                foreach (var dependency in mod.Manifest.Dependencies)
                {
                    if (mod.Manifest.PriorityLoad && !modList.Contains(dependency))
                    {
                        _console.WriteLine($"Error! {mod.Manifest.UniqueName} (priority load) depends on a normal mod! Removing from load...");
                        mods.Remove(mod);
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
                _console.WriteLine("Error - Cyclic dependency found.");
                return mods;
            }

            sortedList.Reverse();

            // Get the IModData back by looking up uniqueName in dict
            var returnList = new List<IModData>();
            sortedList.ForEach(mod => returnList.Add(modDict[mod]));

            return returnList;
        }

        // Thanks to https://gist.github.com/Sup3rc4l1fr4g1l1571c3xp14l1d0c10u5/3341dba6a53d7171fe3397d13d00ee3f

        static List<string> TopologicalSort(HashSet<string> nodes, HashSet<Edge> edges)
        {
            // Empty list that will contain the sorted elements
            var sortedList = new List<string>();

            // Set of all nodes with no incoming edges
            var nodesWithNoEdges = new HashSet<string>(nodes.Where(node => edges.All(edge => edge.Second.Equals(node) == false)));

            // while nodesWithNoEdges is non-empty do
            while (nodesWithNoEdges.Any())
            {
                //  remove a node from nodesWithNoEdges
                var firstNode = nodesWithNoEdges.First();
                nodesWithNoEdges.Remove(firstNode);

                // add node to tail of sortedList
                sortedList.Add(firstNode);

                // for each node secondNode with an edge from firstNode to secondNode do
                foreach (var edge in edges.Where(e => e.First.Equals(firstNode)).ToList())
                {
                    var secondNode = edge.Second;

                    // remove edge e from the graph
                    edges.Remove(edge);

                    // if secondNode has no other incoming edges then
                    if (edges.All(mEdge => mEdge.Second.Equals(secondNode) == false))
                    {
                        // insert secondNode into nodesWithNoEdges
                        nodesWithNoEdges.Add(secondNode);
                    }
                }
            }

            // if graph has edges then
            if (edges.Any())
            {
                // This will be caught and handled in the caller method
                return null;
            }
            else
            {
                // return sortedList (a topologically sorted order)
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
