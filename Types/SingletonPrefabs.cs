using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SolidUtilities.Collections;
using TypeReferences;
using System.Runtime.InteropServices;

namespace EinheitsKiste
{
    [CreateAssetMenu(fileName = "SingletonPrefabs", menuName = "EinheitsKiste/Singleton Prefabs")]
    public class SingletonPrefabs : ScriptableObjectSingleton<SingletonPrefabs>
    {
        [Serializable]
        public struct SingletonPrefab
        {
            [field: SerializeField, Inherits(typeof(MonoBehaviour))] public TypeReference Type { get; private set; }
            [field: SerializeField] public GameObject Prefab { get; private set; }

            public override readonly bool Equals(object obj)
            {
                if (obj is SingletonPrefab other) return Type.Type == other.Type.Type && Prefab == other.Prefab;
                return false;
            }

            public override readonly int GetHashCode() => HashCode.Combine(Type.Type, Prefab);
        }

        [SerializeField] private SingletonPrefab[] _prefabSingletons;

        private Dictionary<Type, GameObject> _prefabSingletonsCache;
        public IReadOnlyDictionary<Type, GameObject> PrefabSingletons
        {
            get
            {
                if (_prefabSingletonsCache == null)
                {
                    _prefabSingletonsCache = _prefabSingletons.ToDictionary(
                        prefab => prefab.Type.Type,
                        prefab => prefab.Prefab);
                }
                return _prefabSingletonsCache;
            }
        }

        private void OnValidate()
        {
            var duplicateKeys = _prefabSingletons
                .GroupBy(prefab => prefab.Type.Type)
                .Where(group => group.Count() > 1)
                .ToList();

            foreach (var group in duplicateKeys)
            {
                Debug.LogError($"Duplicate singleton prefab type found: {group.Key}", this);
            }

            var duplicateValues = _prefabSingletons
                .GroupBy(prefab => prefab.Prefab)
                .Where(group => group.Count() > 1)
                .ToList();

            foreach (var group in duplicateValues)
            {
                Debug.LogError($"Duplicate singleton prefab found: {group.Key}", this);
            }
        }
    }
}
