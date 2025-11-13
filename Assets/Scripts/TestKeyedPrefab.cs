using EinheitsKiste;
using MyBox;
using UnityEngine;
using EinheitsKiste.KeyedMonoBehaviourMode;

public class TestKeyedPrefab : KeyedMonoBehaviour<TestKeyedPrefab, MonoBehaviourID, IInstancesInSceneAndAssetsMode>
{
    [SerializeField, AutoProperty] private MonoBehaviourID _key;
    public override MonoBehaviourID Key => _key;
}
