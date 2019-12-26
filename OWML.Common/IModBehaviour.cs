using System.Collections;
using UnityEngine;

namespace OWML.Common
{
    public interface IModBehaviour
    {
        IModManifest ModManifest { get; set; }
        Coroutine StartCoroutine(IEnumerator routine);
    }
}
