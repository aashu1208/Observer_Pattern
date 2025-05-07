using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private NetworkCharacterController _cc;
    public GameObject cube;

    private bool isBig = false;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        // ✅ Make sure only the input authority processes the logic
        if (!HasInputAuthority)
            return;

        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);

            // ✅ Only toggle once per press
            if (data.scaleToggle)
            {
                ToggleScale();
            }
        }
    }

    public void ToggleScale()
    {
        isBig = !isBig;

        if (cube != null)
        {
            cube.transform.localScale = isBig ? Vector3.one * 2f : Vector3.one;
            Debug.Log($"[Player] Toggled Scale: {cube.transform.localScale}");
        }
        else
        {
            Debug.LogWarning("[Player] Cube reference is missing!");
        }
    }
}
