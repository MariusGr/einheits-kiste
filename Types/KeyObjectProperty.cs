using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SolidUtilities.Editor;

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
        public readonly int value;

        public KeyObjectReferenceAttribute(object enumValue)
        {
            Type type = enumValue.GetType();

            if (!type.IsEnum)
                throw new ArgumentException($"Provided Type '{type}' is not an enum. Please provide an enum for {GetType()}.");

            if (!Enum.IsDefined(type, enumValue))
                throw new ArgumentException($"Provided value '{enumValue}' is not defined in enum '{type}'. Please provide a valid value.");

            enumType = type;
            value = (int)enumValue;
        }
    }
}

#if UNITY_EDITOR
namespace EinheitsKiste.Internal
{
    [CustomPropertyDrawer(typeof(KeyObjectReferenceAttribute))]
    public class KeyObjectPropertyDrawer : PropertyDrawer
    {
        private bool initialized;

        private void Initialize(SerializedProperty property, KeyObjectReferenceAttribute keyObjectReference)
        {
            if (initialized) return;
            initialized = true;

            var enumType = keyObjectReference.enumType;
            var values = Enum.GetValues(enumType).Cast<int>().ToArray();
            var labels = Enum.GetNames(enumType);

            if (values.Count() == 0 || labels.Count() == 0)
                throw new ArgumentException($"The provided enum {enumType} is empty.");

            if (labels.First().ToLower() != "none" || values.First() != 0)
                Debug.LogWarning($"It appears that the provided enum '{enumType}' does not have a 'None' default value as first option. " +
                "Please make sure that the provided enum starts with a 'None' entry at index 0.");

            var before = property.objectReferenceValue;

            var newValue = values[keyObjectReference.value];
            if (newValue == 0)
                property.objectReferenceValue = null;
            else
                try
                {
                    var transform = ((MonoBehaviour)property.serializedObject.targetObject).transform;

                    Type type = property.GetObjectType();

                    if (type == typeof(Transform))
                        property.objectReferenceValue = KeyObject.GetTransform(transform,
                                                                                newValue,
                                                                                enumType);
                    else if (type == typeof(GameObject))
                        property.objectReferenceValue = KeyObject.GetGameObject(transform,
                                                                                newValue,
                                                                                enumType);
                    else if (type.IsSubclassOf(typeof(Component)))
                        property.objectReferenceValue = KeyObject.GetComponent(transform,
                                                                                newValue,
                                                                                enumType,
                                                                                type);
                    else
                        throw new ArgumentException($"The provided Type '{type}' is not supported. " +
                            $"You need something derived of {nameof(Component)}.");
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
                
            if (before == property.objectReferenceValue) return;
            EditorUtility.SetDirty(property.serializedObject.targetObject);
            property.serializedObject.ApplyModifiedProperties();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keyObjectReference = (KeyObjectReferenceAttribute)attribute;
            Initialize(property, keyObjectReference);

            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
#endif
