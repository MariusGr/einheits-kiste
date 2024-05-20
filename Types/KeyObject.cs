using UnityEngine;
using System;
using System.Linq;
using MyBox;
using TypeReferences;

namespace EinheitsKiste
{
    public class KeyObject : MonoBehaviour
    {
        [SerializeField, Inherits(typeof(Enum), DropdownHeight = 300)] private TypeReference enumType = typeof(TZG.Runtime.CharacterKeyObjectEnum);
        [field: SerializeField, DefinedValues(nameof(GetKeyNames))] public int Key { get; private set; }

        public class NoKeyObjectFoundException : Exception
        {
            public NoKeyObjectFoundException(string message) : base(message) { }
        }

        public class MoreThanOneKeyObjectsFoundException : Exception
        {
            public MoreThanOneKeyObjectsFoundException(string message) : base(message) { }
        }

        private LabelValuePair[] GetKeyNames()
        {
            var labels = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType).Cast<int>();
            return labels.Zip(values, (label, value) => new LabelValuePair(label, value)).ToArray();
        }

        public static KeyObject GetKeyObject(Transform root, int value, Type enumType)
        {
            var objects = root.GetComponentsInChildren<KeyObject>().Where(x => x.Key == value && x.enumType.Type == enumType);

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
