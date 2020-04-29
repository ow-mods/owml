using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OWML.ModLoader
{
    class ModSorter
    {
        public IList<IModData> SortMods(IList<IModData> mods)
        {
            // Make dict of uniqueName to IModData
            var modDict = new Dictionary<string, IModData>();
            foreach (var item in mods)
            {
                modDict.Add(item.Manifest.UniqueName, item);
            }

            // Make list of uniqueNames
            var modList = new List<string>();
            foreach (var mod in mods)
            {
                 modList.Add(mod.Manifest.UniqueName);
            }

            // Make hashset of tuples of Dependant : Dependency
            HashSet<Tuple<string, string>> set = new HashSet<Tuple<string, string>>();
            foreach (var mod in mods)
            {
                foreach (var dep in mod.Manifest.Dependencies)
                {
                    set.Add(new Tuple<string, string>(mod.Manifest.UniqueName, dep));
                }
            }

            // Sort the mods
            var ret = TopologicalSort(
                new HashSet<string>(modList),
                new HashSet<Tuple<string, string>>(set)
            );

            // Reverse the list
            ret.Reverse();

            // Get the IModData back by looking up uniqueName in dict
            var returnList = new List<IModData>();
            ret.ForEach(x => returnList.Add(modDict[x]));

            return returnList;
        }

        // Thanks to https://gist.github.com/Sup3rc4l1fr4g1l1571c3xp14l1d0c10u5/3341dba6a53d7171fe3397d13d00ee3f

        static List<T> TopologicalSort<T>(HashSet<T> nodes, HashSet<Tuple<T, T>> edges)
        {
            // Empty list that will contain the sorted elements
            var sortedList = new List<T>();

            // Set of all nodes with no incoming edges
            var nodesWithNoEdges = new HashSet<T>(nodes.Where(n => edges.All(e => e.Item2.Equals(n) == false)));

            // while nodesWithNoEdges is non-empty do
            while (nodesWithNoEdges.Any())
            {
                //  remove a node from nodesWithNoEdges
                var node = nodesWithNoEdges.First();
                nodesWithNoEdges.Remove(node);

                // add node to tail of sortedList
                sortedList.Add(node);

                // for each node m with an edge e from n to m do
                foreach (var e in edges.Where(e => e.Item1.Equals(node)).ToList())
                {
                    var m = e.Item2;

                    // remove edge e from the graph
                    edges.Remove(e);

                    // if m has no other incoming edges then
                    if (edges.All(me => me.Item2.Equals(m) == false))
                    {
                        // insert m into nodesWithNoEdges
                        nodesWithNoEdges.Add(m);
                    }
                }
            }

            // if graph has edges then
            if (edges.Any())
            {
                // return error (graph has at least one cycle)
                return null;
            }
            else
            {
                // return sortedList (a topologically sorted order)
                return sortedList;
            }
        }
    }

    public class Tuple<T1, T2>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        internal Tuple(T1 first, T2 second)
        {
            Item1 = first;
            Item2 = second;
        }
    }

    public static class Tuple
    {
        public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            var tuple = new Tuple<T1, T2>(first, second);
            return tuple;
        }
    }
}
