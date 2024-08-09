using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EinheitsKiste;

[CreateAssetMenu(fileName = "Singleton", menuName = "EinheitsKiste/Singleton")]
public class TestScriptableObjectSingleton : ScriptableObjectSingleton<TestScriptableObjectSingleton>
{
    public string message = "None";
}
