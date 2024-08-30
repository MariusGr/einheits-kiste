using UnityEngine;

public class ImplementingSomething : MonoBehaviour, ISomeInterface
{
    public void SomeMethod()
    {
        Debug.Log("SomeMethod called");
    }
}
