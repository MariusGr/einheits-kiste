using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SolidUtilities.Collections;
using TypeReferences;

namespace EinheitsKiste
{
    [CreateAssetMenu(fileName = "SingletonPrefabs", menuName = "EinheitsKiste/Singleton Prefabs")]
    public class SingletonPrefabs : ScriptableObjectSingleton<SingletonPrefabs>
    {
        [SerializeField] private SerializableDictionary<TypeReference, GameObject> _prefabSingletons = new();

        private Dictionary<Type, GameObject> _prefabSingletonsCache;
        public IReadOnlyDictionary<Type, GameObject> PrefabSingletons
        {
            get
            {
                if (_prefabSingletonsCache == null)
                {
                    _prefabSingletonsCache = _prefabSingletons.ToDictionary(
                        kvp => kvp.Key.Type,
                        kvp => kvp.Value);
                }
                return _prefabSingletonsCache;
            }
        }

        private void OnValidate()
        {
            var duplicateKeys = _prefabSingletons.Keys
                .GroupBy(k => k.Type)
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var group in duplicateKeys)
            {
                Debug.LogError($"Duplicate singleton prefab type found: {group.Key}", this);
            }
        }
    }
}
