using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing_Prefabs : MonoBehaviour
{
    public bool isScale = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.G))
        {
            ScaleToggele();
        }
    }

    public void ScaleToggele()
    {
        isScale = !isScale;
        if(isScale)
        {
            transform.localScale = Vector3.one * 2f;
        }
        else
        {
            transform.localScale = Vector3.one * 1f;
        }
    }
}
