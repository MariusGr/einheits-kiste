using System.Collections;
using UnityEngine;

namespace EinheitsKiste
{
    public static class MonoBehaviours
    {
        private static IEnumerator Empty() { yield break; }
        public static Coroutine EmptyCoroutine(this MonoBehaviour monoBehaviour) => monoBehaviour.StartCoroutine(Empty());
    }
}
