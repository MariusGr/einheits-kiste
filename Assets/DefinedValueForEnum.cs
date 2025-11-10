using UnityEngine;
using MyBox;
using EinheitsKiste;

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
        this.EmptyCoroutine();
        return (TestEnum[])System.Enum.GetValues(typeof(TestEnum));
    }
}
