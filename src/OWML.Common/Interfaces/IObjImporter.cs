using UnityEngine;

namespace OWML.Common
{
	public interface IObjImporter
	{
		Mesh ImportFile(string objectPath);
	}
}