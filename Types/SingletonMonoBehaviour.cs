using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public interface ISingletonMonoBehaviour { }

    public class SingletonMonoBehaviour<T> : MonoBehaviour, ISingletonMonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        internal static T _instance;
        public static T Instance
        {
            get
            {
                if (!InstanceExists())
                    _instance = FindSingleton(FindObjectsByType<T>(FindObjectsSortMode.None));
                return _instance;
            }
        }

        public static bool InstanceExists() => _instance != null;

        virtual protected void Awake()
        {
            bool instanceExists = InstanceExists();
            // Instance already set, but it is this object: skip initialisation
            if (instanceExists && _instance.GetInstanceID() == GetInstanceID())
                return;

            var instances = gameObject.GetComponents<T>();
            // Check if another isntance is already stored in instance
            if (instanceExists)
                throw new MultipleSingletonInSceneException(new HashSet<T>(instances) { _instance }.ToArray());

            _instance = FindSingleton(instances);
        }

        public static T FindSingleton(T[] instances)
        {
            if (instances.Length == 0)
                throw new SingletonDoesNotExistException();
            if (instances.Length > 1)
                throw new MultipleSingletonInSceneException(instances);

            return instances.FirstOrDefault();
        }

        // Source: https://codereview.stackexchange.com/questions/276679/creating-a-generic-base-class-for-singletons-in-unity
        public class SingletonDoesNotExistException : Exception
        {
            private static string DefaultMessage
                => $"{typeof(SingletonMonoBehaviour<T>)} is required by a script, but does not exist in scene \"{SceneManager.GetActiveScene().name}\".";
            public SingletonDoesNotExistException() : base(DefaultMessage) { }
        }

        // Source: https://codereview.stackexchange.com/questions/276679/creating-a-generic-base-class-for-singletons-in-unity
        public class MultipleSingletonInSceneException : Exception
        {
            private static string DefaultMessage(IEnumerable<T> singletons)
                => $"{typeof(SingletonMonoBehaviour<T>)} is a singleton, but multiple copies exist in the scene {SceneManager.GetActiveScene().name}: "
                   + string.Join(", ", singletons.Select(s => $"{s.name} ({s.gameObject.name})"));

            public MultipleSingletonInSceneException(params T[] singletons) : base(DefaultMessage(singletons)) { }
        }
    }
}
