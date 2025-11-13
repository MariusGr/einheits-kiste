using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EinheitsKiste.KeyedMonoBehaviourMode
{
    public interface IMode { }
    public interface IInstancesInAssetsOnlyMode : IMode { }
    public interface IInstancesInSceneOnlyMode : IMode { }
    public interface IInstancesInSceneThenAssetsMode : IMode { }
    public interface IInstancesInSceneAndAssetsMode : IMode { }
}

namespace EinheitsKiste
{
    public abstract class KeyedMonoBehaviour<T, TKey> : KeyedMonoBehaviour<T, TKey, KeyedMonoBehaviourMode.IInstancesInSceneOnlyMode>
        where T : KeyedMonoBehaviour<T, TKey>
    { }

    public abstract class KeyedMonoBehaviour<T, TKey, TMode> : MonoBehaviour
        where T : KeyedMonoBehaviour<T, TKey, TMode>
        where TMode : KeyedMonoBehaviourMode.IMode
    {
        private static T[] GetInstancesFromScene() => FindObjectsByType<T>(FindObjectsSortMode.None);
        private static T[] GetInstancesFromAssets() => Resources.FindObjectsOfTypeAll<T>();
        private static T[] GetInstancesFromSceneThenAssets()
        {
            var instancesInScene = GetInstancesFromScene();
            if (instancesInScene.Length > 0) return instancesInScene;
            return GetInstancesFromAssets();
        }

        private static T[] GetInstancesFromSceneAndAssets()
        {
            var instancesInScene = GetInstancesFromScene();
            var instancesInAssets = GetInstancesFromAssets();
            return instancesInScene.Concat(instancesInAssets).Distinct().ToArray();
        }

        private static readonly Dictionary<TKey, T> _instances = new();
        private static readonly Func<T[]> _getInstances;
        public static T[] GetInstances() => _getInstances();

        public abstract TKey Key { get; }

        static KeyedMonoBehaviour()
        {
            if (typeof(TMode) == typeof(KeyedMonoBehaviourMode.IInstancesInAssetsOnlyMode)) _getInstances = GetInstancesFromAssets;
            else if (typeof(TMode) == typeof(KeyedMonoBehaviourMode.IInstancesInSceneOnlyMode)) _getInstances = GetInstancesFromScene;
            else if (typeof(TMode) == typeof(KeyedMonoBehaviourMode.IInstancesInSceneThenAssetsMode)) _getInstances = GetInstancesFromSceneThenAssets;
            else if (typeof(TMode) == typeof(KeyedMonoBehaviourMode.IInstancesInSceneAndAssetsMode)) _getInstances = GetInstancesFromSceneAndAssets;
            else throw new ArgumentException($"Invalid mode type {typeof(TMode).Name} for {nameof(KeyedMonoBehaviour<T, TKey, TMode>)}");
        }

        public static T Get(TKey key)
        {
            if (TryGet(key, out T instance)) return instance;
            Debug.LogWarning($"No instance of {typeof(T).Name} found with key {key}");
            return null;
        }

        public static bool TryGet(TKey key, out T instance)
        {
            if (_instances.TryGetValue(key, out instance))
                return true;

            instance = GetInstances().Where(i => i.Key.Equals(key)).FirstOrDefault();

            if (instance != null)
            {
                _instances.Add(key, instance);
                return true;
            }

            return false;
        }


        protected virtual void OnValidate()
        {
            if (Key == null) return;
            if (_instances.ContainsKey(Key)) return;

            _instances.Add(Key, (T)this);
        }

        virtual protected void OnDestroy()
        {
            if (Key == null) return;
            if (!_instances.ContainsKey(Key)) return;

            _instances.Remove(Key);
        }
    }
}
