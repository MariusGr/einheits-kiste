using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace EinheitsKiste
{
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (!InstanceExists())
                    instance = FindSingleton();
                return instance;
            }
        }

        private static T FindSingleton()
        {
            var typeName = typeof(T).Name;
            var guid = AssetDatabase.FindAssets($"t:{typeName}").FirstOrDefault() ?? throw new SingletonDoesNotExistException();
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var singleton = AssetDatabase.LoadAssetAtPath<T>(path);
            return singleton;
        }

        public static bool InstanceExists() => instance != null;

        virtual protected void Awake()
        {
            bool instanceExists = InstanceExists();
            // Instance already set, but it is this object: skip initialisation
            if (instanceExists && instance.GetInstanceID() == GetInstanceID())
                return;

            var instances = FindObjectsByType<T>(FindObjectsSortMode.None);
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
                => $"Could not find instance of {typeof(T).Name} in Assets folder.";
            public SingletonDoesNotExistException() : base(DefaultMessage) { }
        }

        // Source: https://codereview.stackexchange.com/questions/276679/creating-a-generic-base-class-for-singletons-in-unity
        public class MultipleSingletonInSceneException : Exception
        {
            private static string message(IEnumerable<T> singletons)
                => $"{typeof(ScriptableObjectSingleton<T>)} is a singleton, but multiple copies exist in the scene {SceneManager.GetActiveScene().name}: "
                   + string.Join(", ", singletons.Select(s => s.name));

            public MultipleSingletonInSceneException(params T[] singletons) : base(message(singletons)) { }
        }
    }
}
