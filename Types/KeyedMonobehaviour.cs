using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EinheitsKiste
{
    // TODO Get working with non-ExecuteInEditMode MonoBehaviours
    public abstract class KeyedMonoBehaviour<T, KT> : MonoBehaviour where T : KeyedMonoBehaviour<T, KT>
    {
        public abstract KT Key { get; }

        private static readonly Dictionary<KT, T> _instances = new();
        public static T[] GetInstances() => FindObjectsByType<T>(FindObjectsSortMode.None);

        public static T Get(KT key)
        {
            if (_instances.TryGetValue(key, out T instance))
                return instance;

            instance = GetInstances().Where(i => i.Key.Equals(key)).FirstOrDefault();

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
