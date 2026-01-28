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
                    _prefabSingletonsCache = _prefabSingletons.ToDictionary(
                        prefab =>
                        {
                            if (prefab.TryGetComponent<ISingletonMonoBehaviour>(out var component)) return component.GetType();
                            if (prefab.TryGetComponent<MonoBehaviour>(out var monoBehaviour)) return monoBehaviour.GetType();
                            Debug.LogError($"Prefab {prefab.name} does not have a MonoBehaviour component", prefab);
                            return null;
                        },
                        prefab => prefab);
                }
                return _prefabSingletonsCache;
            }
        }

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

            foreach (var prefab in _prefabSingletons)
            {
                if (!prefab.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
                {
                    Debug.LogError($"Prefab {prefab.name} does not have a MonoBehaviour component", prefab);
                }
            }
        }
    }
}
