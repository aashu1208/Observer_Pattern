using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Simple Meta XR UI Setup - Works with Meta Quest controllers
/// This script adds the essential components for VR UI interaction
/// </summary>
public class SimpleMetaXRUI : MonoBehaviour
{
    [Header("Quick VR UI Fix")]
    public bool setupOnStart = true;
    
    void Start()
    {
        if (setupOnStart)
        {
            FixVRUIInteraction();
        }
    }

    [ContextMenu("Fix VR UI Interaction")]
    public void FixVRUIInteraction()
    {
        Debug.Log("🔧 Fixing VR UI Interaction...");

        try
        {
            Step1_FixEventSystem();
            Step2_AddOVRComponents();
            Step3_FixButtons();
            Debug.Log("✅ VR UI Interaction Fixed!");
            Debug.Log("📝 Instructions: Point controllers at buttons and pull trigger to click");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Fix failed: {ex.Message}");
        }
    }

    void Step1_FixEventSystem()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ No EventSystem found!");
            return;
        }

        // Remove StandaloneInputModule (for desktop)
        StandaloneInputModule[] inputModules = eventSystem.GetComponents<StandaloneInputModule>();
        foreach (var module in inputModules)
        {
            if (Application.isPlaying)
                Destroy(module);
            else
                DestroyImmediate(module);
        }

        // Add OVRInputModule (for VR)
        OVRInputModule ovrInput = eventSystem.GetComponent<OVRInputModule>();
        if (ovrInput == null)
        {
            ovrInput = eventSystem.gameObject.AddComponent<OVRInputModule>();
        }

        // Configure OVR Input - using properties available in Meta XR SDK 78.0.0
        // The OVRInputModule will automatically handle controller input

        Debug.Log("✅ EventSystem fixed with OVRInputModule");
    }

    void Step2_AddOVRComponents()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ No Canvas found!");
            return;
        }

        // Remove regular GraphicRaycaster
        GraphicRaycaster regularRaycaster = canvas.GetComponent<GraphicRaycaster>();
        if (regularRaycaster != null)
        {
            if (Application.isPlaying)
                Destroy(regularRaycaster);
            else
                DestroyImmediate(regularRaycaster);
        }

        // Add OVRRaycaster for VR
        OVRRaycaster ovrRaycaster = canvas.GetComponent<OVRRaycaster>();
        if (ovrRaycaster == null)
        {
            ovrRaycaster = canvas.gameObject.AddComponent<OVRRaycaster>();
        }

        Debug.Log("✅ Canvas configured with OVRRaycaster");

        // Add pointer to controllers
        AddPointerToControllers();
    }

    void AddPointerToControllers()
    {
        // Add to Left Controller
        GameObject leftController = GameObject.Find("LeftControllerAnchor");
        if (leftController == null)
            leftController = GameObject.Find("LeftHandAnchor");
        
        if (leftController != null)
        {
            AddOVRPointerToController(leftController, "Left");
        }

        // Add to Right Controller  
        GameObject rightController = GameObject.Find("RightControllerAnchor");
        if (rightController == null)
            rightController = GameObject.Find("RightHandAnchor");
            
        if (rightController != null)
        {
            AddOVRPointerToController(rightController, "Right");
        }

        Debug.Log("✅ OVR Pointers added to controllers");
    }

    void AddOVRPointerToController(GameObject controller, string side)
    {
        // Create pointer object
        Transform pointerTransform = controller.transform.Find($"{side}Pointer");
        GameObject pointerObj = pointerTransform?.gameObject;
        
        if (pointerObj == null)
        {
            pointerObj = new GameObject($"{side}Pointer");
            pointerObj.transform.SetParent(controller.transform);
            pointerObj.transform.localPosition = Vector3.zero;
            pointerObj.transform.localRotation = Quaternion.identity;
        }

        // Add OVRPhysicsRaycaster
        OVRPhysicsRaycaster physicsRaycaster = pointerObj.GetComponent<OVRPhysicsRaycaster>();
        if (physicsRaycaster == null)
        {
            physicsRaycaster = pointerObj.AddComponent<OVRPhysicsRaycaster>();
        }

        // Add Line Renderer for visual feedback
        LineRenderer lineRenderer = pointerObj.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = pointerObj.AddComponent<LineRenderer>();
            
            // Configure line renderer
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.material.color = Color.blue;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
        }

        // Add simple ray controller
        SimpleVRRayController rayController = pointerObj.GetComponent<SimpleVRRayController>();
        if (rayController == null)
        {
            rayController = pointerObj.AddComponent<SimpleVRRayController>();
            rayController.isLeftHand = side == "Left";
            rayController.lineRenderer = lineRenderer;
        }
    }

    void Step3_FixButtons()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        Button[] buttons = canvas.GetComponentsInChildren<Button>();
        
        foreach (Button button in buttons)
        {
            // Add collider for VR ray interaction
            Collider buttonCollider = button.GetComponent<Collider>();
            if (buttonCollider == null)
            {
                BoxCollider boxCollider = button.gameObject.AddComponent<BoxCollider>();
                RectTransform rectTransform = button.GetComponent<RectTransform>();
                
                // Set collider size based on button size
                boxCollider.size = new Vector3(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y, 10f);
                boxCollider.center = Vector3.zero;
            }

            Debug.Log($"✅ Button '{button.name}' VR-ready with collider");
        }

        Debug.Log($"✅ {buttons.Length} buttons configured for VR interaction");
    }
}

/// <summary>
/// Simple VR Ray Controller for button interaction
/// </summary>
public class SimpleVRRayController : MonoBehaviour
{
    [Header("Controller Settings")]
    public bool isLeftHand = false;
    public LineRenderer lineRenderer;
    public float maxDistance = 5f;

    private OVRInput.Controller controller;
    private Button currentButton;

    void Start()
    {
        controller = isLeftHand ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
    }

    void Update()
    {
        if (!OVRInput.IsControllerConnected(controller))
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;
            return;
        }

        UpdateRay();
        HandleButtonClick();
    }

    void UpdateRay()
    {
        if (lineRenderer == null) return;

        Vector3 rayStart = transform.position;
        Vector3 rayForward = transform.forward;
        Vector3 rayEnd = rayStart + rayForward * maxDistance;

        // Show ray
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, rayStart);

        // Raycast for buttons
        RaycastHit hit;
        bool hitButton = Physics.Raycast(rayStart, rayForward, out hit, maxDistance);

        if (hitButton)
        {
            Button button = hit.collider.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                currentButton = button;
                rayEnd = hit.point;
                lineRenderer.material.color = Color.red; // Highlight when over button
            }
            else
            {
                currentButton = null;
                lineRenderer.material.color = Color.blue;
            }
        }
        else
        {
            currentButton = null;
            lineRenderer.material.color = Color.blue;
        }

        lineRenderer.SetPosition(1, rayEnd);
    }

    void HandleButtonClick()
    {
        if (currentButton == null) return;

        // Check trigger press
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            Debug.Log($"🎯 VR Button clicked: {currentButton.name}");
            currentButton.onClick.Invoke();
            
            // Haptic feedback
            OVRInput.SetControllerVibration(0.2f, 0.2f, controller);
        }
    }
}