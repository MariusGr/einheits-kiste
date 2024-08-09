using UnityEngine;
using MyBox;
using System;

public class DefinedValueTupleTest : MonoBehaviour
{
    [DefinedValues(nameof(GetTuples))] public Tuple<string, string> tupleTest;
    private LabelValuePair[] GetTuples()
        => new LabelValuePair[]
        {
            new LabelValuePair("One->Two", new Tuple<string, string>("1", "2")),
            new LabelValuePair("Two->Three", new Tuple<string, string>("2", "3")),
            new LabelValuePair("Three->Four", new Tuple<string, string>("3", "4")),
            new LabelValuePair("Four->One", new Tuple<string, string>("4", "1")),
        };

    [ButtonMethod]
    private void PrintTuple() => print(tupleTest);
}
