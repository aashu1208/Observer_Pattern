using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManagerTesting : MonoBehaviour
{

    void OnEnable()
    {
        UsingActionDelegates.OnScoreUpdates += UpdateScore;

    }


    void OnDisable()
    {
        UsingActionDelegates.OnScoreUpdates -= UpdateScore;
    }


    public void UpdateScore()
    {
        Debug.Log("Score updated: ");
    }
    
}
