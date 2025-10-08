using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Quick VR Button Fix for Meta XR SDK 78.0.0
/// Simple script that makes buttons clickable with VR controllers
/// </summary>
public class QuickVRButtonFix : MonoBehaviour
{
    [Header("Quick VR Button Fix")]
    public bool fixOnStart = true;
    
    void Start()
    {
        if (fixOnStart)
        {
            FixVRButtons();
        }
    }

    [ContextMenu("Fix VR Buttons")]
    public void FixVRButtons()
    {
        Debug.Log("🔧 Quick VR Button Fix starting...");

        try
        {
            // Step 1: Fix EventSystem
            FixEventSystem();
            
            // Step 2: Fix Canvas
            FixCanvas();
            
            // Step 3: Add Colliders to Buttons
            AddButtonColliders();
            
            // Step 4: Add Simple Ray Pointers
            AddSimpleRayPointers();
            
            Debug.Log("✅ VR Button Fix Complete!");
            Debug.Log("🎯 Point controllers at buttons and pull trigger to click");
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ VR Button Fix failed: {ex.Message}");
        }
    }

    void FixEventSystem()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("❌ No EventSystem found!");
            return;
        }

        // Remove StandaloneInputModule
        StandaloneInputModule[] modules = eventSystem.GetComponents<StandaloneInputModule>();
        foreach (var module in modules)
        {
            if (Application.isPlaying)
                Destroy(module);
            else
                DestroyImmediate(module);
        }

        // Add OVRInputModule
        if (eventSystem.GetComponent<OVRInputModule>() == null)
        {
            eventSystem.gameObject.AddComponent<OVRInputModule>();
        }

        Debug.Log("✅ EventSystem configured for VR");
    }

    void FixCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ No Canvas found!");
            return;
        }

        // Remove GraphicRaycaster
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
        {
            if (Application.isPlaying)
                Destroy(raycaster);
            else
                DestroyImmediate(raycaster);
        }

        // Add OVRRaycaster
        if (canvas.GetComponent<OVRRaycaster>() == null)
        {
            canvas.gameObject.AddComponent<OVRRaycaster>();
        }

        Debug.Log("✅ Canvas configured for VR");
    }

    void AddButtonColliders()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        Button[] buttons = canvas.GetComponentsInChildren<Button>();
        int buttonCount = 0;

        foreach (Button button in buttons)
        {
            // Add BoxCollider for VR ray interaction
            if (button.GetComponent<Collider>() == null)
            {
                BoxCollider boxCollider = button.gameObject.AddComponent<BoxCollider>();
                RectTransform rect = button.GetComponent<RectTransform>();
                
                // Set collider size
                boxCollider.size = new Vector3(rect.sizeDelta.x, rect.sizeDelta.y, 1f);
                boxCollider.center = Vector3.zero;
                
                buttonCount++;
                Debug.Log($"✅ Added collider to button: {button.name}");
            }
        }

        Debug.Log($"✅ {buttonCount} buttons configured for VR");
    }

    void AddSimpleRayPointers()
    {
        // Find controller anchors
        GameObject leftHand = FindControllerAnchor("Left");
        GameObject rightHand = FindControllerAnchor("Right");

        if (leftHand != null)
        {
            AddRayToController(leftHand, true);
        }

        if (rightHand != null)
        {
            AddRayToController(rightHand, false);
        }

        Debug.Log("✅ Ray pointers added to controllers");
    }

    GameObject FindControllerAnchor(string side)
    {
        // Try different naming conventions
        string[] names = {
            $"{side}HandAnchor",
            $"{side}ControllerAnchor", 
            $"{side}Hand",
            $"{side}Controller"
        };

        foreach (string name in names)
        {
            GameObject found = GameObject.Find(name);
            if (found != null)
            {
                Debug.Log($"✅ Found {side} controller: {found.name}");
                return found;
            }
        }

        Debug.LogWarning($"⚠️ {side} controller not found");
        return null;
    }

    void AddRayToController(GameObject controller, bool isLeft)
    {
        // Create or find ray object
        Transform rayTransform = controller.transform.Find("VRRay");
        GameObject rayObject = rayTransform?.gameObject;

        if (rayObject == null)
        {
            rayObject = new GameObject("VRRay");
            rayObject.transform.SetParent(controller.transform);
            rayObject.transform.localPosition = Vector3.zero;
            rayObject.transform.localRotation = Quaternion.identity;
        }

        // Add Line Renderer
        LineRenderer line = rayObject.GetComponent<LineRenderer>();
        if (line == null)
        {
            line = rayObject.AddComponent<LineRenderer>();
            
            // Configure line
            line.material = new Material(Shader.Find("Unlit/Color"));
            line.material.color = Color.cyan;
            line.startWidth = 0.005f;
            line.endWidth = 0.005f;
            line.positionCount = 2;
            line.useWorldSpace = true;
        }

        // Add simple ray controller
        SimpleVRRay rayController = rayObject.GetComponent<SimpleVRRay>();
        if (rayController == null)
        {
            rayController = rayObject.AddComponent<SimpleVRRay>();
            rayController.isLeftController = isLeft;
            rayController.lineRenderer = line;
        }
    }
}

/// <summary>
/// Simple VR Ray for button interaction
/// </summary>
public class SimpleVRRay : MonoBehaviour
{
    [Header("Settings")]
    public bool isLeftController = false;
    public LineRenderer lineRenderer;
    public float rayLength = 5f;
    
    [Header("Colors")]
    public Color normalColor = Color.cyan;
    public Color hoverColor = Color.yellow;
    
    private OVRInput.Controller controller;
    private Button hoveredButton;

    void Start()
    {
        controller = isLeftController ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
    }

    void Update()
    {
        if (!OVRInput.IsControllerConnected(controller))
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;
            return;
        }

        UpdateRayVisual();
        CheckButtonInteraction();
    }

    void UpdateRayVisual()
    {
        if (lineRenderer == null) return;

        Vector3 rayStart = transform.position;
        Vector3 rayDirection = transform.forward;
        Vector3 rayEnd = rayStart + (rayDirection * rayLength);

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, rayStart);

        // Cast ray to find buttons
        RaycastHit hit;
        if (Physics.Raycast(rayStart, rayDirection, out hit, rayLength))
        {
            Button button = hit.collider.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                // Hovering over button
                hoveredButton = button;
                rayEnd = hit.point;
                lineRenderer.material.color = hoverColor;
            }
            else
            {
                hoveredButton = null;
                lineRenderer.material.color = normalColor;
            }
        }
        else
        {
            hoveredButton = null;
            lineRenderer.material.color = normalColor;
        }

        lineRenderer.SetPosition(1, rayEnd);
    }

    void CheckButtonInteraction()
    {
        if (hoveredButton == null) return;

        // Check trigger press
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            // Click the button
            hoveredButton.onClick.Invoke();
            
            // Haptic feedback
            OVRInput.SetControllerVibration(0.3f, 0.3f, controller);
            
            Debug.Log($"🎯 VR Button clicked: {hoveredButton.name}");
        }
    }

    void OnDisable()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }
}