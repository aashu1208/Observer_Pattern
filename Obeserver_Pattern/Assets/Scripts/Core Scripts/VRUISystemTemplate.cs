using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// VR UI System Template - Creates all mandatory components for VR UI interaction
/// Based on analysis of working Meta XR setup
/// </summary>
public class VRUISystemTemplate : MonoBehaviour
{
    [Header("VR UI System Template")]
    [Tooltip("Automatically create complete VR UI system on Start")]
    public bool createSystemOnStart = true;
    
    [Header("System Configuration")]
    public Vector3 canvasPosition = new Vector3(0, 1.5f, 2f);
    public Vector3 canvasScale = new Vector3(0.001f, 0.001f, 0.001f);
    
    [Header("Dependencies")]
    [Tooltip("Drag your OVRCameraRig here, or leave null for auto-find")]
    public GameObject ovrCameraRig;

    void Start()
    {
        if (createSystemOnStart)
        {
            CreateVRUISystem();
        }
    }

    [ContextMenu("Create Complete VR UI System")]
    public void CreateVRUISystem()
    {
        Debug.Log("🚀 Creating Complete VR UI System...");

        try
        {
            // Level 1: Core System Components
            CreateEventSystem();
            CreateVRCanvas();
            
            // Level 2: Controller Integration  
            SetupControllerInteraction();
            
            // Level 3: Sample UI Elements
            CreateSampleButtons();
            
            Debug.Log("✅ Complete VR UI System Created!");
            Debug.Log("📋 System includes: EventSystem, Canvas, Controller Rays, Sample Buttons");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ VR UI System creation failed: {ex.Message}");
        }
    }

    void CreateEventSystem()
    {
        // Check if EventSystem already exists
        EventSystem existingEventSystem = FindObjectOfType<EventSystem>();
        GameObject eventSystemObj;

        if (existingEventSystem != null)
        {
            eventSystemObj = existingEventSystem.gameObject;
            Debug.Log("📍 Using existing EventSystem");
        }
        else
        {
            // Create new EventSystem
            eventSystemObj = new GameObject("VR EventSystem");
            eventSystemObj.transform.SetParent(this.transform);
            eventSystemObj.AddComponent<EventSystem>();
            Debug.Log("✅ Created new EventSystem");
        }

        // Ensure correct input module
        StandaloneInputModule[] standaloneModules = eventSystemObj.GetComponents<StandaloneInputModule>();
        foreach (var module in standaloneModules)
        {
            if (Application.isPlaying)
                Destroy(module);
            else
                DestroyImmediate(module);
        }

        // Add OVR Input Module
        if (eventSystemObj.GetComponent<OVRInputModule>() == null)
        {
            eventSystemObj.AddComponent<OVRInputModule>();
            Debug.Log("✅ Added OVRInputModule to EventSystem");
        }
    }

    void CreateVRCanvas()
    {
        // Create Canvas GameObject
        GameObject canvasObj = new GameObject("VR UI Canvas");
        canvasObj.transform.SetParent(this.transform);
        canvasObj.layer = LayerMask.NameToLayer("UI");

        // Add Canvas component
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        // Position canvas for VR
        canvasObj.transform.position = canvasPosition;
        canvasObj.transform.localScale = canvasScale;

        // Set camera reference
        Camera vrCamera = FindVRCamera();
        if (vrCamera != null)
        {
            canvas.worldCamera = vrCamera;
        }

        // Add CanvasScaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // Remove regular GraphicRaycaster and add OVRRaycaster
        GraphicRaycaster regularRaycaster = canvasObj.GetComponent<GraphicRaycaster>();
        if (regularRaycaster != null)
        {
            if (Application.isPlaying)
                Destroy(regularRaycaster);
            else
                DestroyImmediate(regularRaycaster);
        }

        canvasObj.AddComponent<OVRRaycaster>();

        Debug.Log("✅ Created VR Canvas with OVRRaycaster");
    }

    void SetupControllerInteraction()
    {
        // Find OVR Camera Rig
        if (ovrCameraRig == null)
        {
            ovrCameraRig = FindObjectOfType<OVRCameraRig>()?.gameObject;
        }

        if (ovrCameraRig == null)
        {
            Debug.LogWarning("⚠️ OVRCameraRig not found. Controller interaction may not work.");
            return;
        }

        // Check if OVRRayHelper already exists on controllers
        OVRRayHelper[] existingRayHelpers = ovrCameraRig.GetComponentsInChildren<OVRRayHelper>();
        
        if (existingRayHelpers.Length > 0)
        {
            Debug.Log($"✅ Found {existingRayHelpers.Length} existing OVRRayHelper components");
            
            // Ensure they are enabled
            foreach (var rayHelper in existingRayHelpers)
            {
                rayHelper.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No OVRRayHelper found. Please add OVRRayHelper to your controller prefabs.");
            Debug.Log("📝 Tip: Use OVR Controller prefabs or manually add OVRRayHelper to HandAnchor/Controller objects");
        }
    }

    void CreateSampleButtons()
    {
        Canvas canvas = this.GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas not found for creating sample buttons");
            return;
        }

        // Create panel container
        GameObject panel = new GameObject("Button Panel");
        panel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 200);
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // Semi-transparent black

        // Create sample buttons
        string[] buttonNames = { "VR Button 1", "VR Button 2", "VR Button 3" };
        for (int i = 0; i < buttonNames.Length; i++)
        {
            CreateVRButton(panel, buttonNames[i], new Vector3(i * 140 - 140, 0, 0));
        }

        Debug.Log($"✅ Created {buttonNames.Length} sample VR buttons");
    }

    GameObject CreateVRButton(GameObject parent, string buttonName, Vector3 localPosition)
    {
        // Create button GameObject
        GameObject buttonObj = new GameObject(buttonName);
        buttonObj.transform.SetParent(parent.transform, false);
        buttonObj.layer = LayerMask.NameToLayer("UI");

        // Add RectTransform
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(120, 60);
        buttonRect.localPosition = localPosition;

        // Add Button component
        Button button = buttonObj.AddComponent<Button>();

        // Add Image background
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.white;

        // Add BoxCollider for VR ray interaction
        BoxCollider buttonCollider = buttonObj.AddComponent<BoxCollider>();
        buttonCollider.size = new Vector3(buttonRect.sizeDelta.x, buttonRect.sizeDelta.y, 1f);

        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = buttonRect.sizeDelta;
        textRect.localPosition = Vector3.zero;

        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = buttonName;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 14;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.black;

        // Add click event (optional)
        button.onClick.AddListener(() => Debug.Log($"🎯 {buttonName} clicked in VR!"));

        return buttonObj;
    }

    Camera FindVRCamera()
    {
        // Find center eye camera
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam.name.Contains("Center") || cam.name.Contains("Eye"))
            {
                return cam;
            }
        }
        
        return Camera.main;
    }
}

/// <summary>
/// VR UI System Requirements Checker
/// Use this to validate your VR UI setup
/// </summary>
[System.Serializable]
public class VRUISystemChecker
{
    [System.Serializable]
    public class SystemRequirement
    {
        public string componentName;
        public bool isPresent;
        public bool isMandatory;
        public string description;
    }

    public SystemRequirement[] requirements = new SystemRequirement[]
    {
        new SystemRequirement { 
            componentName = "EventSystem", 
            isMandatory = true,
            description = "Routes UI events in the scene"
        },
        new SystemRequirement { 
            componentName = "OVRInputModule", 
            isMandatory = true,
            description = "Handles VR controller input for UI"
        },
        new SystemRequirement { 
            componentName = "Canvas (WorldSpace)", 
            isMandatory = true,
            description = "3D UI canvas for VR interaction"
        },
        new SystemRequirement { 
            componentName = "OVRRaycaster", 
            isMandatory = true,
            description = "VR-compatible ray detection for UI"
        },
        new SystemRequirement { 
            componentName = "OVRRayHelper", 
            isMandatory = true,
            description = "Controller ray interaction system"
        },
        new SystemRequirement { 
            componentName = "Button Colliders", 
            isMandatory = true,
            description = "BoxColliders on UI buttons for ray hits"
        }
    };

    public void CheckSystemRequirements()
    {
        Debug.Log("🔍 VR UI System Requirements Check:");
        
        foreach (var req in requirements)
        {
            string status = req.isPresent ? "✅" : (req.isMandatory ? "❌" : "⚠️");
            Debug.Log($"{status} {req.componentName}: {req.description}");
        }
    }
}