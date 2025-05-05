using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    public bool isBig = false;
    public GameObject cube;
    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
/* 
        if(!HasInputAuthority)
            return; */
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5*data.direction*Runner.DeltaTime);
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            ToggleScale();
        }
    }


    public void ToggleScale()
    {
        isBig = !isBig;
        if(isBig)
        {
            cube.transform.localScale = Vector3.one * 2f;
        }

        else if(!isBig)
        {
            cube.transform.localScale = Vector3.one;
        }
        
        
    }
}