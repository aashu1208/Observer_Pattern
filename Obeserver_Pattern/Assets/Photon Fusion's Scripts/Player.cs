using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    public Transform cube;

    [Networked] private bool isBig { get; set; } = false;

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

        if (HasInputAuthority && Input.GetKeyDown(KeyCode.G))
        {
            isBig = !isBig;  // ✅ Only this player's cube will toggle size
        }

        cube.localScale = isBig ? Vector3.one * 2f : Vector3.one;
    }
}
