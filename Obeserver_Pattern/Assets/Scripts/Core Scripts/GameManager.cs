using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, ISubject
{

    private List<ISubscriber> _subscribers = new List<ISubscriber>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Subscribe(ISubscriber subscriber)
    {
        if(!_subscribers.Contains(subscriber))
        {
            _subscribers.Add(subscriber);
        }
    }

    public void Unsubscribe(ISubscriber subscriber)
    {

        if(_subscribers.Contains(subscriber))
        {
            _subscribers.Remove(subscriber);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="param"></param>
    public void Notify(string eventType, object param = null)
    {
        foreach (ISubscriber subscriber in _subscribers)
        {
            subscriber.OnNotify(eventType, param);
        }
    }
}
