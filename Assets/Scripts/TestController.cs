using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EinheitsKiste;
using KeyObjects;

public class TestController : MonoBehaviour
{
    [KeyObjectReference(typeof(TestEnum))] public Transform ref1;
    [KeyObjectReference(typeof(TestEnum2))] public Transform ref2;
}
