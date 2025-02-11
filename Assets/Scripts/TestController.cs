using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EinheitsKiste;
using KeyObjects;
using MyBox;
using UnityEventBus;

public class TestController : MonoBehaviour
{
    [KeyObjectReference(TestEnum.Etwas)] public Transform ref1;
    [KeyObjectReference(TestEnum2.GanzWasAnderes)] public GameObject ref2;
    [KeyObjectReference(TestEnum2.NochEtwas)] public AudioSource ref3;
    [KeyObjectReference(searchOnlyOnSelf: true)] public AudioSource refSelf;
    [field: SerializeField, KeyObjectReference(TestEnum2.NochEtwas)] public AudioSource Ref4 { get; private set; }
    [DefinedValues(validationMethod: nameof(Validation), definedValues: new object[] { "Eins", "Zwei", "Drei", "Vier" })] public string strings;
    [DefinedValues(definedValues: new object[] { "Eins", "Zwei", "Drei", "Vier" })] public string strings2;
    [DefinedValues(nameof(GetEventOptions))] public string eventTest;

    private bool Validation(int index, object value)
    {
        if (index == 2) return false;
        return true;
    }

    private LabelValuePair[] GetEventOptions() => EventBusUtil.GetEventOptions();

    void Start()
    {
        TestSingleton.Instance.Hello();
        print(TestScriptableObjectSingleton.Instance.message);
        EventBusUtil.RaiseEvent(eventTest, new TestEvent() { health = 100 });
    }
}
