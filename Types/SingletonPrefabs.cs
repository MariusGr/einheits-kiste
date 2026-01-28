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
                        prefab => prefab.GetComponent<ISingletonMonoBehaviour>().GetType(),
                        prefab => prefab);
                }
                return _prefabSingletonsCache;
            }
        }

        private void OnValidate()
        {
            var missingSingletons = _prefabSingletons
                .Where(prefab => !prefab.TryGetComponent<ISingletonMonoBehaviour>(out _))
                .ToList();
            
            foreach (var prefab in missingSingletons)
            {
                Debug.LogError($"Prefab {prefab.name} does not contain a component implementing ISingletonMonoBehaviour", this);
            }

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
