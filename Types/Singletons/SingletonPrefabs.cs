using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace EinheitsKiste
{
    [CreateAssetMenu(fileName = "SingletonPrefabs", menuName = "EinheitsKiste/Singleton Prefabs")]
    public class SingletonPrefabs : ScriptableObjectSingleton<SingletonPrefabs>
    {
        [SerializeField] private GameObject[] _prefabSingletons;

        private Dictionary<Type, GameObject> _prefabSingletonsCache;
        public IReadOnlyDictionary<Type, GameObject> PrefabSingletons
        {
            get
            {
                if (_prefabSingletonsCache == null)
                {
                    _prefabSingletonsCache = new();
                    foreach (var prefab in _prefabSingletons)
                    {
                        Type type = null;
                        if (prefab.TryGetComponent<ISingletonMonoBehaviour>(out var component)) type = component.GetType();
                        else if (prefab.TryGetComponent<MonoBehaviour>(out var monoBehaviour)) type = monoBehaviour.GetType();

                        if (type != null) _prefabSingletonsCache[type] = prefab;
                    }
                        
                }
                return _prefabSingletonsCache;
            }
        }
        public IEnumerable<GameObject> Prefabs => PrefabSingletons.Values;

        private void OnValidate()
        {
            var duplicateValues = _prefabSingletons
                .GroupBy(prefab => prefab)
                .Where(group => group.Count() > 1)
                .ToList();

            foreach (var group in duplicateValues)
            {
                Debug.LogError($"Duplicate singleton prefab found: {group.Key.name}", this);
            }
        }
    }
}
