using UnityEngine;

/// <summary>
/// VR UI Blueprint - Shows exactly how your current GameObject setup works
/// This documents your working VR UI interaction system for replication
/// </summary>
public class VRUIBlueprint : MonoBehaviour
{
    [Header("🔍 Your Current VR UI System Analysis")]
    [TextArea(5, 10)]
    public string systemDescription = @"
MANDATORY COMPONENTS FOR VR UI INTERACTION:

LEVEL 1 - CORE SYSTEM:
✅ EventSystem GameObject
   ├── EventSystem component
   └── OVRInputModule component
   
✅ Canvas GameObject  
   ├── Canvas (RenderMode.WorldSpace)
   ├── OVRRaycaster component
   └── CanvasScaler component

LEVEL 2 - CONTROLLER INTERACTION:
✅ OVRCameraRig (Your existing setup)
   └── TrackingSpace/
       ├── LeftHandAnchor/LeftControllerAnchor/*/OVRRayHelper
       └── RightHandAnchor/RightControllerAnchor/*/OVRRayHelper

LEVEL 3 - UI ELEMENTS:
✅ UI Buttons
   ├── Button component
   ├── BoxCollider (for ray hits)
   └── Visual components (Image, Text)
";

    [ContextMenu("📋 Print Complete VR UI Blueprint")]
    public void PrintCompleteBlueprint()
    {
        Debug.Log("📋 COMPLETE VR UI INTERACTION BLUEPRINT:");
        Debug.Log("=====================================");
        
        PrintCoreComponents();
        PrintControllerComponents();  
        PrintUIElements();
        PrintCreationSteps();
    }

    void PrintCoreComponents()
    {
        Debug.Log("\n🏗️ LEVEL 1 - CORE SYSTEM COMPONENTS:");
        Debug.Log("-----------------------------------");
        
        Debug.Log("1️⃣ EventSystem GameObject:");
        Debug.Log("   • EventSystem component - Routes UI events");
        Debug.Log("   • OVRInputModule component - VR controller input");
        Debug.Log("   • ❌ Remove: StandaloneInputModule (desktop only)");
        
        Debug.Log("\n2️⃣ Canvas GameObject:");
        Debug.Log("   • Canvas component (RenderMode = WorldSpace)");
        Debug.Log("   • OVRRaycaster component - VR ray detection");
        Debug.Log("   • CanvasScaler component - UI scaling");
        Debug.Log("   • worldCamera = CenterEyeAnchor camera");
        Debug.Log("   • ❌ Remove: GraphicRaycaster (desktop only)");
    }

    void PrintControllerComponents()
    {
        Debug.Log("\n🎮 LEVEL 2 - CONTROLLER INTERACTION:");
        Debug.Log("-----------------------------------");
        
        Debug.Log("3️⃣ OVRRayHelper (Per Controller):");
        Debug.Log("   • Location: HandAnchor/ControllerAnchor/Controller/OVRRayHelper");
        Debug.Log("   • OVRRayHelper component - Meta's interaction system");
        Debug.Log("   • Child 'Ray' GameObject - Visual ray line");
        Debug.Log("   • Child 'Cursor' GameObject - Ray hit indicator");
        
        Debug.Log("\n🔧 Your Current Setup Path:");
        Debug.Log("   Left:  /GameObject/OVRCameraRig/TrackingSpace/LeftHandAnchor/.../OVRRayHelper");
        Debug.Log("   Right: /GameObject/OVRCameraRig/TrackingSpace/RightHandAnchor/.../OVRRayHelper");
    }

    void PrintUIElements()
    {
        Debug.Log("\n🎯 LEVEL 3 - UI ELEMENT REQUIREMENTS:");
        Debug.Log("-----------------------------------");
        
        Debug.Log("4️⃣ VR-Compatible UI Buttons:");
        Debug.Log("   • Button component - Unity UI button logic");
        Debug.Log("   • BoxCollider component - For 3D ray collision");
        Debug.Log("   • Image component - Button background");
        Debug.Log("   • Text component - Button label");
        Debug.Log("   • RectTransform - UI positioning");
        
        Debug.Log("\n🎨 Visual Feedback Components:");
        Debug.Log("   • LineRenderer (on Ray) - Visual ray line");
        Debug.Log("   • Material with Unlit/Color shader - Ray appearance");
        Debug.Log("   • Dynamic color changes - Hover/press feedback");
    }

    void PrintCreationSteps()
    {
        Debug.Log("\n🚀 REPLICATION STEPS FOR NEW PROJECT:");
        Debug.Log("=====================================");
        
        Debug.Log("STEP 1 - Core Setup:");
        Debug.Log("• Create EventSystem → Add EventSystem + OVRInputModule");
        Debug.Log("• Create Canvas → WorldSpace + OVRRaycaster + CanvasScaler");
        Debug.Log("• Position Canvas at (0, 1.5, 2) with scale (0.001, 0.001, 0.001)");
        
        Debug.Log("\nSTEP 2 - VR Integration:");
        Debug.Log("• Add OVRCameraRig prefab to scene");
        Debug.Log("• Ensure OVRRayHelper exists on controller objects");
        Debug.Log("• Set Canvas.worldCamera to CenterEyeAnchor");
        
        Debug.Log("\nSTEP 3 - UI Creation:");
        Debug.Log("• Create UI buttons as children of Canvas");
        Debug.Log("• Add BoxCollider to each button (size = button dimensions + depth)");
        Debug.Log("• Test with VR controllers → rays should hit buttons");
        
        Debug.Log("\n✅ RESULT: VR controllers can point and click UI buttons!");
    }

    [ContextMenu("🔍 Analyze Current Scene Setup")]
    public void AnalyzeCurrentSetup()
    {
        Debug.Log("🔍 ANALYZING YOUR CURRENT VR UI SETUP:");
        Debug.Log("====================================");

        // Check EventSystem
        var eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem != null)
        {
            var ovrInput = eventSystem.GetComponent<UnityEngine.EventSystems.OVRInputModule>();
            Debug.Log($"✅ EventSystem found: {eventSystem.name}");
            Debug.Log($"   OVRInputModule: {(ovrInput != null ? "✅ Present" : "❌ Missing")}");
        }
        else
        {
            Debug.Log("❌ EventSystem not found");
        }

        // Check Canvas
        var canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            var ovrRaycaster = canvas.GetComponent<OVRRaycaster>();
            Debug.Log($"✅ Canvas found: {canvas.name}");
            Debug.Log($"   Render Mode: {canvas.renderMode}");
            Debug.Log($"   OVRRaycaster: {(ovrRaycaster != null ? "✅ Present" : "❌ Missing")}");
        }
        else
        {
            Debug.Log("❌ Canvas not found");
        }

        // Check OVRRayHelpers
        var rayHelpers = FindObjectsOfType<OVRRayHelper>();
        Debug.Log($"🎮 OVRRayHelper components found: {rayHelpers.Length}");
        foreach (var helper in rayHelpers)
        {
            Debug.Log($"   ✅ {helper.transform.GetPath()}");
        }

        // Check Buttons with Colliders
        var buttons = FindObjectsOfType<UnityEngine.UI.Button>();
        int buttonsWithColliders = 0;
        foreach (var button in buttons)
        {
            if (button.GetComponent<Collider>() != null)
            {
                buttonsWithColliders++;
            }
        }
        Debug.Log($"🎯 Buttons found: {buttons.Length}");
        Debug.Log($"   Buttons with Colliders: {buttonsWithColliders}");

        Debug.Log("\n📊 SYSTEM STATUS:");
        bool isComplete = eventSystem != null && canvas != null && rayHelpers.Length > 0 && buttonsWithColliders > 0;
        Debug.Log(isComplete ? "✅ VR UI System is COMPLETE and should work!" : "⚠️ VR UI System needs fixes");
    }
}

// Extension method for getting full path
public static class TransformExtensions
{
    public static string GetPath(this Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}