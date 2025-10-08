using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractButtonCube : MonoBehaviour
{
    public GameObject cube;
    public void Update()
    {
        ToggleCube();
    }
    public void ActiveCube()
    {
        cube.SetActive(true);
    }
    public void DeactiveCube()
    {
        cube.SetActive(false);
    }
    public void ToggleCube()
    {

        if(OVRInput.GetDown(OVRInput.Button.One))
        {
            cube.SetActive(!cube.activeSelf);
        }
    }

}
