#if UNITY_EDITOR
using UnityEditor;

namespace EinheitsKiste
{
    public static class SerializedObjects
    {
        public static SerializedProperty FindPropertyByAutoName(this SerializedObject obj, string propertyName)
            => obj.FindProperty(Property.GetBackingFieldName(propertyName));
    }
}
#endif
