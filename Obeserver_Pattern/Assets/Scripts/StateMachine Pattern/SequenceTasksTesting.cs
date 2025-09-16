using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
public class SequenceTasksTesting : MonoBehaviour
{
    public TextMeshProUGUI taskText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Tasks()
    {

        Debug.Log("Task 1 started");
        taskText.text = "Task 1 started";
        yield return new WaitForSeconds(1f);
        Debug.Log("Task 1 completed");
        taskText.text = "Task 1 completed";
    }
}
