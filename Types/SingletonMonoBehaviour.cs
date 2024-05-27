using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EinheitsKiste
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (!InstanceExists())
                    instance = FindSingleton(FindObjectsByType<T>(FindObjectsSortMode.None));
                return instance;
            }
        }

        public static bool InstanceExists() => instance != null;

        virtual protected void Awake()
        {
            bool instanceExists = InstanceExists();
            // Instance already set, but it is this object: skip initialisation
            if (instanceExists && instance.GetInstanceID() == GetInstanceID())
                return;

            var instances = gameObject.GetComponents<T>();
            // Check if another isntance is already stored in instance
            if (instanceExists)
                throw new MultipleSingletonInSceneException(new HashSet<T>(instances) { instance }.ToArray());

            instance = FindSingleton(instances);
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
