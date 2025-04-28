using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
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
}
