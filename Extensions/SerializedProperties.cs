using UnityEditor;

namespace EinheitsKiste
{
    public static class SerializedProperties
    {
        public static SerializedProperty FindPropertyByAutoName(this SerializedProperty obj, string propertyName)
            => obj.FindPropertyRelative($"<{propertyName}>k__BackingField");
    }
}
