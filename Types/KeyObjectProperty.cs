using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EinheitsKiste
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class KeyObjectEnumAttribute : Attribute
    {
#if UNITY_EDITOR
        private static readonly HashSet<Type> enums = new();
        public static IReadOnlyCollection<Type> Enums => enums;
        public KeyObjectEnumAttribute(Type enumType) => enums.Add(enumType);
#endif
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class KeyObjectReferenceAttribute : PropertyAttribute
    {
        public readonly Type enumType;

        public KeyObjectReferenceAttribute(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException($"Provided Type '{enumType}' is not an enum. Please provide an enum for {GetType()}.");

            this.enumType = enumType;
        }
    }
}

#if UNITY_EDITOR
namespace EinheitsKiste.Internal
{
    [CustomPropertyDrawer(typeof(KeyObjectReferenceAttribute))]
    public class KeyObjectPropertyDrawer : PropertyDrawer
    {
        private const float lineHeight = 17f;

        private int[] values;
        private string[] labels;
        private bool initialized;

        private void Initialize(SerializedProperty targetProperty, KeyObjectReferenceAttribute defaultValuesAttribute)
        {
            if (initialized) return;
            initialized = true;

            var enumType = defaultValuesAttribute.enumType;
            values = Enum.GetValues(enumType).Cast<int>().ToArray();
            labels = Enum.GetNames(enumType);

            if (values.Count() == 0 || labels.Count() == 0)
                throw new ArgumentException($"The provided enum {enumType} is empty.");

            if (labels.First().ToLower() != "none" || values.First() != 0)
                Debug.LogWarning($"It appears that the provided enum '{enumType}' does not have a 'None' default value as first option. " +
                "Please make sure that the provided enum starts with a 'None' entry at index 0.");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + lineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyObjectReference = (KeyObjectReferenceAttribute)attribute;
            Initialize(property, keyObjectReference);

            GUI.enabled = false;
            EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight, position.width, position.height), property, label, true);
            GUI.enabled = true;

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginProperty(position, label, property);
            var newIndex = EditorGUI.Popup(position, label.text, GetSelectedIndex(), labels);
            EditorGUI.EndProperty();
            if (EditorGUI.EndChangeCheck()) ApplyNewValue(newIndex, keyObjectReference.enumType);

            int GetSelectedIndex()
            {
                var transform = (Transform)property.objectReferenceValue;
                if (transform == null) return 0;
                var keyObject = transform.GetComponent<KeyObject>();

                if (keyObject.EnumType.Type != keyObjectReference.enumType)
                {
                    property.objectReferenceValue = null;
                    return 0;
                }

                for (var i = 0; i < values.Length; i++)
                {
                    if (keyObject.Key == i) return i;
                }

                return 0;
            }

            void ApplyNewValue(int newValueIndex, Type enumType)
            {
                var newValue = values[newValueIndex];
                if (newValue == 0)
                {
                    property.objectReferenceValue = null;
                    return;
                }

                try
                {
                    property.objectReferenceValue = KeyObject.GetTransform(((MonoBehaviour)property.serializedObject.targetObject).transform, newValue, enumType);
                }
                catch (Exception e)
                {
                    if (e is KeyObject.MoreThanOneKeyObjectsFoundException || e is KeyObject.NoKeyObjectFoundException)
                    {
                        Debug.LogWarning($"Had to reset {property.name} of {property.serializedObject.targetObject} because of Exception: {e}");
                        property.objectReferenceValue = null;
                    }
                    else
                        throw;
                }
                EditorUtility.SetDirty(property.serializedObject.targetObject);

                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
