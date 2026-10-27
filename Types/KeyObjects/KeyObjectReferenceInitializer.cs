#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEditor;

namespace EinheitsKiste
{
    /// <summary>
    /// Helper utility for initializing KeyObjectReferenceAttribute fields.
    /// Call this from OnValidate or OnAfterDeserialize in your MonoBehaviour to ensure references are correct after duplication.
    /// </summary>
    public static class KeyObjectReferenceInitializer
    {
        public static void InitializeAllReferences(MonoBehaviour target)
        {
            var fields = target.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (var field in fields)
            {
                var attrs = field.GetCustomAttributes(typeof(KeyObjectReferenceAttribute), false);
                if (attrs.Length == 0) continue;

                var attr = (KeyObjectReferenceAttribute)attrs[0];
                InitializeField(target, field, attr);
            }
        }

        private static void InitializeField(MonoBehaviour target, System.Reflection.FieldInfo field, KeyObjectReferenceAttribute attr)
        {
            try
            {
                if (attr.enumType != null)
                {
                    var transform = target.transform;
                    var fieldType = field.FieldType;

                    UnityEngine.Object newValue = null;
                    if (attr.value == 0)
                    {
                        newValue = null;
                    }
                    else if (fieldType == typeof(Transform))
                    {
                        newValue = KeyObject.GetTransform(transform, attr.value, attr.enumType);
                    }
                    else if (fieldType == typeof(GameObject))
                    {
                        newValue = KeyObject.GetGameObject(transform, attr.value, attr.enumType);
                    }
                    else if (fieldType.IsSubclassOf(typeof(Component)))
                    {
                        newValue = KeyObject.GetComponent(transform, attr.value, attr.enumType, fieldType);
                    }
                    else
                    {
                        Debug.LogWarning($"The type '{fieldType}' is not supported for {field.Name}");
                        return;
                    }

                    AssignIfChanged(target, field, newValue);
                }
                else if (attr.searchOnlyOnSelf)
                {
                    var transform = target.transform;
                    var fieldType = field.FieldType;
                    var newValue = KeyObject.GetComponentOnSelf(transform, fieldType);
                    AssignIfChanged(target, field, newValue);
                }
            }
            catch (Exception e)
            {
                if (e is KeyObject.MoreThanOneKeyObjectsFoundException || e is KeyObject.NoKeyObjectFoundException)
                {
                    if (!attr.allowEmpty)
                    {
                        Debug.LogWarning($"Had to reset {field.Name} of {target.name} because of Exception: {e}");
                    }
                    AssignIfChanged(target, field, null);
                }
                else
                {
                    Debug.LogError($"Error initializing {field.Name}: {e}");
                }
            }
        }

        // Only dirty the target on an actual change, otherwise the periodic re-initialization keeps the scene dirty forever.
        private static void AssignIfChanged(MonoBehaviour target, System.Reflection.FieldInfo field, UnityEngine.Object newValue)
        {
            var oldValue = field.GetValue(target) as UnityEngine.Object;
            if (oldValue == newValue) return;

            field.SetValue(target, newValue);
            EditorUtility.SetDirty(target);
        }
    }
}
#endif
