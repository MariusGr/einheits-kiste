using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EinheitsKiste;

public class TestSingleton : SingletonMonoBehaviour<TestSingleton>
{
    public string helloMessage = "";
    public void Hello() => print(helloMessage);
}
