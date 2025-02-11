using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEventBus;

public class EventRegister : MonoBehaviour
{
    IEventBinding testEventBinding;
    TestEvent testEvent = new TestEvent() { health = 100 };

    [ButtonMethod]
    public void RaiseTestEvent()
    {
        EventBusUtil.RaiseEvent(new TestEvent() { health = 100 });
    }

    [ButtonMethod]
    public void RegisterTestEvent()
    {
        testEventBinding = EventBusUtil.RegisterEvent(testEvent.GetType(), OnTestEvent);
    }

    [ButtonMethod]
    public void DeregisterTestEvent()
    {
        EventBusUtil.DeregisterEvent(testEvent.GetType(), testEventBinding);
    }

    private void OnTestEvent(IEvent testEvent)
    {
        Debug.Log("TestEvent: " + ((TestEvent)testEvent).health);
    }
}
