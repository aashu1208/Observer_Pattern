using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TasksExecution : MonoBehaviour
{

    public TextMeshProUGUI debugTasksTex;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TasksExecuting();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {

            StopAllCoroutines();
        }
    }

    public void TasksExecuting()
    {
        debugTasksTex.text = "Tasks Executing";
        SequenceTasksTesting sequenceTasksTesting = FindObjectOfType<SequenceTasksTesting>();
        StartCoroutine(sequenceTasksTesting.Tasks());
    }
}
