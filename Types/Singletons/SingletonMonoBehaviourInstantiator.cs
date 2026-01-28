using System.Linq;
using UnityEngine;

namespace EinheitsKiste
{
    public class SingletonMonoBehaviourInstantiator
    {
        private static bool CheckIfSingletonPrefabsExists()
        {
            if (!SingletonPrefabs.InstanceExists())
            {
                Debug.LogError($"{nameof(SingletonPrefabs)} instance does not exist. Cannot instantiate singleton prefabs." +
                            " Please create one in the project with Create -> EinheitsKiste -> Singleton Prefabs.", null);
                return false;
            }
            return true;
        }

        protected static T Instantiate<T>() where T : MonoBehaviour
        {
            if (typeof(T) is ISingletonMonoBehaviour)
            {
                var instance = UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
                if (instance != default) return instance;
            }

            var singletonObject = new GameObject(typeof(T).Name);
            var singleton = singletonObject.AddComponent<T>();
            UnityEngine.Object.DontDestroyOnLoad(singletonObject);
            return singleton;
        }

        protected static T InstantiatePrefab<T>() where T : MonoBehaviour
        {
            if (typeof(T) is ISingletonMonoBehaviour)
            {
                var instance = UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
                if (instance != default) return instance;
            }

            if (!CheckIfSingletonPrefabsExists()) return null;

            var prefab = SingletonPrefabs.Instance.PrefabSingletons[typeof(T)];
            var g = UnityEngine.Object.Instantiate(prefab);
            UnityEngine.Object.DontDestroyOnLoad(g);

            if (!g.TryGetComponent<T>(out var singleton))
            {
                Debug.LogError($"Prefab {prefab.name} does not contain component of type {typeof(T)}", prefab);
            }

            return singleton;
        }

        protected static void InstantiateAllSingletonPrefabs()
        {
            if (!CheckIfSingletonPrefabsExists()) return;

            foreach (var prefab in SingletonPrefabs.Instance.Prefabs)
            {
                var g = UnityEngine.Object.Instantiate(prefab);
                UnityEngine.Object.DontDestroyOnLoad(g);
            }
        }
    }
}
