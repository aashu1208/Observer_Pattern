using Fusion;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    
    private TMP_Text _messages;
    public bool isBigLocal = false;

    public GameObject cube;
    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);
        }

        
    }

    public void Update()
    {

        if(Object.HasInputAuthority)
        {

            if(Input.GetKeyDown(KeyCode.R))
            {

                RPC_SendMessage("Hey mate!");
            }

            // Scale-Descale Cube
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {

                RPC_UpdateScale(1.5f);
            }

            else if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                RPC_UpdateScale(0.5f);
            }
            
            
        }
    }
    

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_UpdateScale(float scaleMultiplier)
    {
        RPC_ScaleCube(scaleMultiplier);
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ScaleCube(float scaleMultiplier)
    {

        if(cube != null)
        {
            // Scale the cube
            cube.transform.localScale *= scaleMultiplier;
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        RPC_RelayMessage(message, info.Source);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage(string message, PlayerRef messageSource)
    {
        if (_messages == null)
            _messages = FindObjectOfType<TMP_Text>();

        if (messageSource == Runner.LocalPlayer)
        {
            message = $"You said: {message}\n";
        }
        else
        {
            message = $"Some other player said: {message}\n";
        }

        _messages.text += message;

        
    }
}
