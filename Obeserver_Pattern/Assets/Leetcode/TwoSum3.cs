using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoSum3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Testing_Return())
        {
            Debug.Log("Found");
        }

        else
        {
            Debug.Log("Not found");
        }
    }

    private bool Testing_Return()
    {

        for (int i = 0; i <= 7; i++)
        {

            if (i == 8)
            {

                return true;
            }


        }
        return false;
    }
}
