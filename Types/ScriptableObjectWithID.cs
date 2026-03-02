using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MyBox;

namespace EinheitsKiste
{
    public abstract class ScriptableObjectWithID : ScriptableObject, ISerializationCallbackReceiver
    {
        [field: SerializeField, ReadOnly] public string ID { get; private set; }

#if UNITY_EDITOR
        private void AssignNewGuid()
        {
            string path = AssetDatabase.GetAssetPath(this);
            var id = AssetDatabase.AssetPathToGUID(path);
            if (ID != id)
            {
                ID = id;
                RegisterInstance();
                EditorUtility.SetDirty(this);
            }
        }
#endif

        protected abstract void RegisterInstance();

        protected virtual void OnEnable()
        {
            if (ID.IsNullOrEmpty())
                Debug.LogError($"{GetType()} has no ID: {name}");
            else
            {
                RegisterInstance();
            }
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            AssignNewGuid();
            RegisterInstance();
#endif
        }

        public void OnAfterDeserialize() { }
    }

    public class ScriptableObjectWithID<T> : ScriptableObjectWithID where T : ScriptableObjectWithID<T>
    {
        private static readonly Dictionary<string, ScriptableObjectWithID<T>> instances = new();
        public static T Get(string id) => (T)instances[id];
        public static bool TryGet(string id, out T instance)
        {
            if (instances.TryGetValue(id, out ScriptableObjectWithID<T> foundInstance))
            {
                instance = (T)foundInstance;
                return true;
            }
            instance = null;
            return false;
        }

        protected override void RegisterInstance() => instances[ID] = this as T;
    }
}
