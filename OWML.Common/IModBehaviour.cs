using System.Collections;
using UnityEngine;

namespace OWML.Common
{
    public interface IModBehaviour
    {
        Coroutine StartCoroutine(IEnumerator routine);
    }
}
