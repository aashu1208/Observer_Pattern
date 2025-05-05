using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingDelegates : MonoBehaviour
{
    public delegate void DelegatesTesting(string msg);
    DelegatesTesting del = Message => Debug.Log(Message);
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
