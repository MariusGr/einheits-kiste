using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MyBox;

namespace EinheitsKiste
{
    public abstract class ScriptableObjectWithID : ScriptableObject, ISerializationCallbackReceiver
    {
        [field: SerializeField, ReadOnly] public string Id { get; private set; }

#if UNITY_EDITOR
        private void AssignNewUID()
        {
            string path = AssetDatabase.GetAssetPath(this);
            var id = AssetDatabase.AssetPathToGUID(path);
            if (Id != id)
            {
                Id = id;
                RegisterInstance();
                EditorUtility.SetDirty(this);
            }
        }
#endif

        protected abstract void RegisterInstance();

        protected virtual void OnEnable()
        {
            if (Id.IsNullOrEmpty())
                Debug.LogError($"{GetType()} has no ID: {name}");
            else
            {
                RegisterInstance();
            }
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            AssignNewUID();
            RegisterInstance();
#endif
        }

        public void OnAfterDeserialize() { }
    }

    public class ScriptableObjectWithID<T> : ScriptableObjectWithID where T : ScriptableObjectWithID<T>
    {
        private static readonly Dictionary<string, ScriptableObjectWithID<T>> instances = new();
        public static T Get(string id) => (T)instances[id];
        protected override void RegisterInstance() => instances[Id] = this as T;
    }
}
