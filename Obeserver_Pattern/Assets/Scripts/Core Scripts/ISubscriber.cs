using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubscriber
{
    public void OnNotify(string eventType, object param = null);
    
}
