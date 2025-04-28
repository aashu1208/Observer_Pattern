using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubject
{
    void Subscribe(ISubscriber subscriber);
    void Unsubscribe(ISubscriber subscriber);
    void Notify(string eventType, object param = null);

}
