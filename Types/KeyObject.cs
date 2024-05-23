using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using TypeReferences;

namespace EinheitsKiste
{
    public class KeyObject : MonoBehaviour
    {
        public event EventHandler EnumTypeChanged;
        private const string KEYOBJECTNAMESPACE = "KeyObjects.";

        [field: SerializeField, DefinedValues(nameof(GetEnumTypeNames), valueChangedMethod: nameof(OnEnumTypeChanged))]
        public TypeReference EnumType { get; private set; }

        [field: SerializeField, DefinedValues(nameof(GetKeyNames), initializeEvent: nameof(EnumTypeChanged))]
        public int Key { get; private set; }

#if UNITY_EDITOR
        private void OnEnumTypeChanged() => EnumTypeChanged?.Invoke(this, null);

        private LabelValuePair[] GetEnumTypeNames()
        {
            List<string> labels = new() { "None" };
            List<object> values = new() { null };
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes().Where(t => t.IsEnum && t.FullName.Contains(KEYOBJECTNAMESPACE)))
                {
                    labels.Add(t.FullName.Split(KEYOBJECTNAMESPACE)[1]);
                    values.Add(t);
                }
            }

            return labels.Zip(values, (label, value) => new LabelValuePair(label, value)).ToArray();
        }

        private LabelValuePair[] GetKeyNames()
        {
            var labels = Enum.GetNames(EnumType.Type);
            var values = Enum.GetValues(EnumType.Type).Cast<int>();
            return labels.Zip(values, (label, value) => new LabelValuePair(label, value)).ToArray();
        }
#endif

        public class NoKeyObjectFoundException : Exception
        {
            public NoKeyObjectFoundException(string message) : base(message) { }
        }

        public class MoreThanOneKeyObjectsFoundException : Exception
        {
            public MoreThanOneKeyObjectsFoundException(string message) : base(message) { }
        }

        public static KeyObject GetKeyObject(Transform root, int value, Type enumType)
        {
            var objects = root.GetComponentsInChildren<KeyObject>().Where(x => x.Key == value && x.EnumType.Type == enumType);

            if (objects.Count() == 0)
                throw new NoKeyObjectFoundException($"No {nameof(KeyObject)} found under '{root.gameObject.name}' with key '{value} from enum {enumType}'. " +
                    $"Please add a {nameof(KeyObject)} with that key from that enum.");
            if (objects.Count() > 1)
                throw new MoreThanOneKeyObjectsFoundException(
                    $"Found more than one ({objects.Count()}) '{root.gameObject.name}' with key '{value}'. Please remove duplicates.");

            return objects.First();
        }

        public static Transform GetTransform(Transform root, int value, Type enumType) => GetKeyObject(root, value, enumType).transform;
        public static GameObject GetGameObject(Transform root, int value, Type enumType) => GetKeyObject(root, value, enumType).gameObject;
    }
}
