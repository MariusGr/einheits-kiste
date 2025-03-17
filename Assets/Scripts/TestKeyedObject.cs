using UnityEngine;
using EinheitsKiste;
using System.Collections.Generic;
using System.Linq;
using MyBox;

[ExecuteInEditMode]
public class TestKeyedObject : KeyedMonobehaviour<TestKeyedObject>
{
    [SerializeField] private string _key;
    public override string Key => _key;
}

