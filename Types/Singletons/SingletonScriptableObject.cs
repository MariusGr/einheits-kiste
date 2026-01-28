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
#if UNITY_EDITOR
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
                    if (guids.Length == 0)
                        throw new SingletonDoesNotExistException();
                    if (guids.Length > 1)
                        throw new MultipleSingletonInSceneException(guids.Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid))).ToArray());
                    _instance = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
                }
                return _instance;
            }
            set => _instance = value;
        }
#else
        public static T Instance { get; private set; }
#endif
        public static bool InstanceExists() => Instance != null;

        protected void Awake()
        {
#if UNITY_EDITOR
            // Add the this object to the build
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            if (!preloadedAssets.Contains(this))
            {
                preloadedAssets.Add(this);
                PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            }
#endif
        }

        virtual protected void OnEnable()
        {
            // Check if Instance is already set to this object
            if (InstanceExists() && Instance == this)
                return;

            if (InstanceExists())
            {
                // If there is already an instance of this singleton in the scene, destroy this one
                throw new MultipleSingletonInSceneException(Instance, this as T);
            }

            Instance = this as T;
        }

        virtual protected void OnDisable()
        {
            if (Instance == this)
                Instance = null;
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
