using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsingActionDelegates : MonoBehaviour
{

    public static Action OnScoreUpdates;

    void Start()
    {

        //UpdateScore(10);

        OnScoreUpdates?.Invoke();
    }
    public void UpdateScore(int score)
    {
        Debug.Log("Score updated: " + score);
        //OnScoreUpdates?.Invoke(10);
    }
}
