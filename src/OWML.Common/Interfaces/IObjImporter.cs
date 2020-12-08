using UnityEngine;

namespace OWML.Common.Interfaces
{
    public interface IObjImporter
    {
        Mesh ImportFile(string objectPath);
    }
}