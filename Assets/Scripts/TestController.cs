using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EinheitsKiste;
using KeyObjects;
using MyBox;

public class TestController : MonoBehaviour
{
    [KeyObjectReference(typeof(TestEnum))] public Transform ref1;
    [KeyObjectReference(typeof(TestEnum2))] public Transform ref2;
    [DefinedValues(validationMethod: nameof(Validation), definedValues: new object[]{ "Eins", "Zwei", "Drei", "Vier" })] public string strings;
    [DefinedValues(definedValues: new object[]{ "Eins", "Zwei", "Drei", "Vier" })] public string strings2;

    private bool Validation(int index, object value)
    {
        if (index == 2) return false;
        return true;
    }

    void Start()
    {
        TestSingleton.Instance.Hello();
        print(TestScriptableObjectSingleton.Instance.message);
    }
}
