using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace EinheitsKiste
{
    // TODO Get working with non-ExecuteInEditMode MonoBehaviours
    public abstract class KeyedMonobehaviour<T> : MonoBehaviour where T : KeyedMonobehaviour<T>
    {
        public abstract string Key { get; }

        private static readonly Dictionary<string, T> _instances = new();
        private static T[] GetInstances() => FindObjectsByType<T>(FindObjectsSortMode.None);

        public static T Get(string key)
        {
            if (_instances.TryGetValue(key, out T instance))
                return instance;

            instance = GetInstances().Where(i => i.Key == key).FirstOrDefault();

            if (instance != null)
            {
                _instances.Add(key, instance);
                return instance;
            }

            Debug.LogWarning($"No instance of {typeof(T).Name} found with key {key}");
            return null;
        }

        protected virtual void OnValidate()
        {
            if (Key.IsNullOrEmpty()) return;
            if (_instances.ContainsKey(Key)) return;

            _instances.Add(Key, (T)this);
        }

        protected virtual void OnDestroy()
        {
            if (Key.IsNullOrEmpty()) return;
            if (!_instances.ContainsKey(Key)) return;

            _instances.Remove(Key);
        }
    }
}
