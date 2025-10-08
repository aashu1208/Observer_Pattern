using UnityEngine;

/// <summary>
/// Quick setup script - attach this to any GameObject and it will automatically setup VR UI
/// This is a simple helper to get VR UI working quickly
/// </summary>
public class QuickVRUISetup : MonoBehaviour
{
    [Header("One-Click Setup")]
    [SerializeField] private bool autoSetupOnStart = false; // Disabled to avoid conflicts

    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupEverything();
        }
    }

    [ContextMenu("Setup Complete VR UI System")]
    public void SetupEverything()
    {
        Debug.Log("🚀 Starting VR UI Auto-Setup...");

        // Step 1: Add VRUISetup component and run it
        VRUISetup setupComponent = gameObject.GetComponent<VRUISetup>();
        if (setupComponent == null)
        {
            setupComponent = gameObject.AddComponent<VRUISetup>();
        }
        
        setupComponent.SetupVRUI();

        // Step 2: Add TTSTester if not present
        TTSTester ttsComponent = FindObjectOfType<TTSTester>();
        if (ttsComponent == null)
        {
            GameObject ttsObj = new GameObject("TTS Tester");
            ttsComponent = ttsObj.AddComponent<TTSTester>();
            Debug.Log("✅ TTSTester added to scene");
        }

        Debug.Log("🎉 VR UI Setup Complete! You can now:");
        Debug.Log("   • Use VR controllers to point at buttons");
        Debug.Log("   • Press trigger to click buttons");
        Debug.Log("   • Use A/B/X/Y buttons on controllers for TTS");
        Debug.Log("   • UI will follow your camera view");
    }
}