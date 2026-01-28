using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EinheitsKiste
{
    public class SingletonMonoBehaviourInstantiator
    {
        protected static T Instantiate<T>() where T : SingletonMonoBehaviour<T>
        {
            var singletonObject = new GameObject(typeof(T).Name);
            var singleton = singletonObject.AddComponent<T>();
            UnityEngine.Object.DontDestroyOnLoad(singletonObject);
            SingletonMonoBehaviour<T>._instance = singleton;
            return singleton;
        }

        protected static T InstantiatePrefab<T>() where T : SingletonMonoBehaviour<T>
        {
            if (SingletonMonoBehaviour<T>.InstanceExists()) return SingletonMonoBehaviour<T>.Instance;

            if (!SingletonPrefabs.InstanceExists())
            {
                Debug.LogError($"{nameof(SingletonPrefabs)} instance does not exist. Cannot instantiate singleton prefab." +
                               " Please create one in the project with Create -> EinheitsKiste -> Singleton Prefabs.", null);
                return null;
            }

            var prefab = SingletonPrefabs.Instance.PrefabSingletons[typeof(T)];
            var g = UnityEngine.Object.Instantiate(prefab);
            UnityEngine.Object.DontDestroyOnLoad(g);

            if (!g.TryGetComponent<T>(out var singleton))
            {
                Debug.LogError($"Prefab {prefab.name} does not contain component of type {typeof(T)}", prefab);
            }

            SingletonMonoBehaviour<T>._instance = singleton;
            return singleton;
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

        private static T FindSingleton(T[] instances)
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
