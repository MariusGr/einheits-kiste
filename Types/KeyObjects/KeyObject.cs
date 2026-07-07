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
        private const string KEY_OBJECT_NAMESPACE = "KeyObjects.";
        private static LabelValuePair[] _cachedEnumTypeNames;
        private static int _cachedEnumTypeAssemblyCount = -1;
        private static readonly Dictionary<Type, LabelValuePair[]> _cachedKeyNames = new();

        [field: SerializeField, DefinedValues(nameof(GetEnumTypeNames), valueChangedMethod: nameof(OnEnumTypeChanged))]
        public TypeReference EnumType { get; private set; }

        [field: SerializeField, ConditionalField(true, nameof(EnumTypeIsValid)), DefinedValues(nameof(GetKeyNames), initializeEvent: nameof(EnumTypeChanged))]
        public int Key { get; private set; }

        private bool EnumTypeIsValid() => EnumType.Type != null;
        private void OnEnumTypeChanged() => EnumTypeChanged?.Invoke(this, null);

        private LabelValuePair[] GetEnumTypeNames()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (_cachedEnumTypeNames != null && _cachedEnumTypeAssemblyCount == assemblies.Length)
            {
                return _cachedEnumTypeNames;
            }

            var entries = new List<LabelValuePair> { new("None", null) };
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type t in GetAssemblyTypes(assembly))
                {
                    if (!t.IsEnum || t.FullName == null) continue;

                    int namespaceStart = t.FullName.IndexOf(KEY_OBJECT_NAMESPACE, StringComparison.Ordinal);
                    if (namespaceStart < 0) continue;

                    string label = t.FullName[(namespaceStart + KEY_OBJECT_NAMESPACE.Length)..];
                    entries.Add(new LabelValuePair(label, t));
                }
            }

            _cachedEnumTypeNames = entries.ToArray();
            _cachedEnumTypeAssemblyCount = assemblies.Length;
            return _cachedEnumTypeNames;
        }

        private LabelValuePair[] GetKeyNames()
        {
            var enumType = EnumType?.Type;
            if (enumType == null || !enumType.IsEnum) return Array.Empty<LabelValuePair>();
            if (_cachedKeyNames.TryGetValue(enumType, out var cachedEntries)) return cachedEntries;

            var labels = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType).Cast<int>();
            cachedEntries = labels.Zip(values, (label, value) => new LabelValuePair(label, value)).ToArray();
            _cachedKeyNames[enumType] = cachedEntries;
            return cachedEntries;
        }

        private static IEnumerable<Type> GetAssemblyTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }

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
        public static Component GetComponent(Transform root, int value, Type enumType, Type type) => GetKeyObject(root, value, enumType).GetComponent(type);
        public static Component GetComponentOnSelf(Transform self, Type type) => self.GetComponent(type);
    }
}
