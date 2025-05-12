using UnityEngine;
using EinheitsKiste;

[ExecuteInEditMode]
public class TestEnumKeyedObject : KeyedMonoBehaviour<TestEnumKeyedObject, TestEnumKeyedObject.KeyEnum>
{
    public enum KeyEnum
    {
        Eins,
        Zwei,
        Drei
    }

    [SerializeField] private KeyEnum _key;
    public override KeyEnum Key => _key;
}