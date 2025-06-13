using UnityEngine;
using MyBox;

public class DefinedValueForEnum : MonoBehaviour
{
    public enum TestEnum
    {
        First,
        Second,
        Third
    }

    [DefinedValues(nameof(GetValuesEnum))] public TestEnum enumValue;
    private TestEnum[] GetValuesEnum()
    {
        return (TestEnum[])System.Enum.GetValues(typeof(TestEnum));
    }
}
