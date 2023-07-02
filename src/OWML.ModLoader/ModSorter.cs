using System.Collections.Generic;
using System.Linq;
using OWML.Common;

namespace OWML.ModLoader
{
	public class ModSorter : IModSorter
	{
		private readonly IModConsole _console;

		public ModSorter(IModConsole console) => 
			_console = console;

		public IList<IModData> SortMods(IList<IModData> mods)
		{
			// #541 When detecting a cyclic mod dependency we give up on sorting mods at all
			// When this happens because of a disabled mod it will potentially break the rest of the mods for no reason
			var enabledMods = mods.Where(x => x.Enabled).ToList();

			var modDict = new Dictionary<string, IModData>();
			var set = new HashSet<Edge>();
			var modList = enabledMods.Select(mod => mod.Manifest.UniqueName).ToList();

			foreach (var mod in enabledMods)
			{
				if (modDict.ContainsKey(mod.Manifest.UniqueName))
				{
					_console.WriteLine($"Error - {mod.Manifest.UniqueName} already exists in the mod sorter - ignoring duplicate.", MessageType.Error);
					continue;
				}
				modDict.Add(mod.Manifest.UniqueName, mod);

				foreach (var dependency in mod.Manifest.Dependencies)
				{
					if (mod.Manifest.PriorityLoad && !modList.Contains(dependency))
					{
						_console.WriteLine($"Error - {mod.Manifest.UniqueName} (priority load) depends on a normal mod! Removing from load...", MessageType.Error);
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
				_console.WriteLine("Error - Cyclic dependency found. Returning original load order...", MessageType.Error);
				return mods;
			}

			sortedList.Reverse();
			var sortedModData = sortedList.Where(modDict.ContainsKey).Select(mod => modDict[mod]).ToList();

			// Include the disabled mods at the end of the list
			return sortedModData.Union(mods.Where(x => !x.Enabled)).ToList();
		}

		// Thanks to https://gist.github.com/Sup3rc4l1fr4g1l1571c3xp14l1d0c10u5/3341dba6a53d7171fe3397d13d00ee3f
		private static List<string> TopologicalSort(HashSet<string> nodes, HashSet<Edge> edges)
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

			return edges.Any() ? null : sortedList;
		}
	}

	internal class Edge
	{
		public string First { get; }

		public string Second { get; }

		internal Edge(string first, string second)
		{
			First = first;
			Second = second;
		}
	}
}
