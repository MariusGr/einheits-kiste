using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEditor;
using EinheitsKiste;

public class DefinedValueTest : MonoBehaviour
{
    public bool showThat;
    [ConditionalField(nameof(showThat))] public string that = "That";

    [DefinedValues(nameof(GetValues))] public string test;
    private string[] GetValues() => names;
    public Entry[] entries;
    public string[] names;

    void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(transform.position, Vector3.forward);
    }
}

[System.Serializable]
public class Entry
{
    public bool showThat;
    [ConditionalField(nameof(showThat))] public string that = "That";

    public string[] names;
    [DefinedValues(nameof(GetValues))] public string selection;
    private string[] GetValues() => names;
}
