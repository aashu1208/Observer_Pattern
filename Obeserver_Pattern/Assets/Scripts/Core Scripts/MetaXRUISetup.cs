using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Meta XR VR UI Setup - Adds proper Meta XR interaction components for VR UI
/// This makes buttons clickable with Meta Quest controllers
/// </summary>
public class MetaXRUISetup : MonoBehaviour
{
    [Header("Meta XR UI Setup")]
    public bool setupOnStart = true;
    public Canvas targetCanvas;
    
    [Header("Interaction Settings")]
    public LayerMask uiLayerMask = 1 << 5; // UI layer
    public float pointerLength = 10f;
    public bool addHapticFeedback = true;

    void Start()
    {
        if (setupOnStart)
        {
            SetupMetaXRUI();
        }
    }

    [ContextMenu("Setup Meta XR UI Interaction")]
    public void SetupMetaXRUI()
    {
        Debug.Log("🚀 Setting up Meta XR UI Interaction...");

        try
        {
            SetupCanvas();
            SetupOVRRaycaster();
            SetupControllerRays();
            SetupUIButtons();
            Debug.Log("✅ Meta XR UI Setup Complete!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Meta XR UI Setup failed: {ex.Message}");
        }
    }

    void SetupCanvas()
    {
        // Find Canvas
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
        }

        if (targetCanvas == null)
        {
            Debug.LogError("❌ No Canvas found!");
            return;
        }

        // Ensure Canvas is configured for VR
        targetCanvas.renderMode = RenderMode.WorldSpace;
        
        // Remove regular GraphicRaycaster and add OVRRaycaster for VR
        GraphicRaycaster regularRaycaster = targetCanvas.GetComponent<GraphicRaycaster>();
        if (regularRaycaster != null)
        {
            DestroyImmediate(regularRaycaster);
        }

        // Add OVRRaycaster for Meta XR interaction
        OVRRaycaster ovrRaycaster = targetCanvas.GetComponent<OVRRaycaster>();
        if (ovrRaycaster == null)
        {
            ovrRaycaster = targetCanvas.gameObject.AddComponent<OVRRaycaster>();
        }

        Debug.Log("✅ Canvas configured with OVRRaycaster");
    }

    void SetupOVRRaycaster()
    {
        // Enable OVR Input Module for VR interaction
        OVRInputModule ovrInputModule = FindObjectOfType<OVRInputModule>();
        if (ovrInputModule == null)
        {
            // Find EventSystem and replace input module
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
            {
                // Remove regular input modules
                StandaloneInputModule[] inputModules = eventSystem.GetComponents<StandaloneInputModule>();
                foreach (var module in inputModules)
                {
                    DestroyImmediate(module);
                }

                // Add OVR Input Module
                ovrInputModule = eventSystem.gameObject.AddComponent<OVRInputModule>();
                
                Debug.Log("✅ OVRInputModule added to EventSystem");
            }
        }

        // Configure OVR Input Module - using Meta XR SDK 78.0.0 compatible settings
        if (ovrInputModule != null)
        {
            ovrInputModule.rayTransform = null; // Will be set per controller
            // Note: Meta XR SDK 78.0.0 handles input automatically
        }
    }

    void SetupControllerRays()
    {
        // Setup Left Hand Ray
        GameObject leftHand = GameObject.Find("LeftHandAnchor");
        if (leftHand != null)
        {
            SetupControllerRay(leftHand, "Left Controller Ray", OVRInput.Controller.LTouch);
        }

        // Setup Right Hand Ray
        GameObject rightHand = GameObject.Find("RightHandAnchor");
        if (rightHand != null)
        {
            SetupControllerRay(rightHand, "Right Controller Ray", OVRInput.Controller.RTouch);
        }

        Debug.Log("✅ Controller rays setup complete");
    }

    void SetupControllerRay(GameObject handAnchor, string rayName, OVRInput.Controller controller)
    {
        // Create ray GameObject
        GameObject rayObject = handAnchor.transform.Find(rayName)?.gameObject;
        if (rayObject == null)
        {
            rayObject = new GameObject(rayName);
            rayObject.transform.SetParent(handAnchor.transform);
            rayObject.transform.localPosition = Vector3.zero;
            rayObject.transform.localRotation = Quaternion.identity;
        }

        // Add Line Renderer for visual ray
        LineRenderer lineRenderer = rayObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = rayObject.AddComponent<LineRenderer>();
        }

        // Configure Line Renderer
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.cyan;
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        // Add Meta XR Ray Interactor
        MetaXRRayInteractor rayInteractor = rayObject.GetComponent<MetaXRRayInteractor>();
        if (rayInteractor == null)
        {
            rayInteractor = rayObject.AddComponent<MetaXRRayInteractor>();
        }

        // Configure Ray Interactor
        rayInteractor.controller = controller;
        rayInteractor.lineRenderer = lineRenderer;
        rayInteractor.maxDistance = pointerLength;
        rayInteractor.uiLayerMask = uiLayerMask;
    }

    void SetupUIButtons()
    {
        Button[] buttons = targetCanvas.GetComponentsInChildren<Button>();
        
        foreach (Button button in buttons)
        {
            // Ensure buttons have Colliders for VR interaction
            Collider buttonCollider = button.GetComponent<Collider>();
            if (buttonCollider == null)
            {
                BoxCollider boxCollider = button.gameObject.AddComponent<BoxCollider>();
                RectTransform rectTransform = button.GetComponent<RectTransform>();
                
                // Set collider size based on RectTransform
                boxCollider.size = new Vector3(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y, 1f);
                boxCollider.isTrigger = false; // Keep as solid for ray interaction
            }

            Debug.Log($"✅ Button '{button.name}' configured for VR interaction");
        }

        Debug.Log($"✅ {buttons.Length} buttons configured for Meta XR interaction");
    }
}

/// <summary>
/// Custom Ray Interactor for Meta XR UI interaction
/// Handles controller ray pointing and button clicking
/// </summary>
public class MetaXRRayInteractor : MonoBehaviour
{
    [Header("Controller Settings")]
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    
    [Header("Ray Settings")]
    public LineRenderer lineRenderer;
    public float maxDistance = 10f;
    public LayerMask uiLayerMask = 1 << 5;
    
    [Header("Visual Feedback")]
    public Color normalColor = Color.cyan;
    public Color hoverColor = Color.yellow;
    public Color pressColor = Color.red;

    private Button currentButton;
    private bool isHovering;

    void Update()
    {
        if (!OVRInput.IsControllerConnected(controller))
        {
            if (lineRenderer != null)
                lineRenderer.enabled = false;
            return;
        }

        UpdateRaycast();
        HandleInput();
    }

    void UpdateRaycast()
    {
        if (lineRenderer == null) return;

        Vector3 rayStart = transform.position;
        Vector3 rayDirection = transform.forward;
        Vector3 rayEnd = rayStart + rayDirection * maxDistance;

        // Enable line renderer
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, rayStart);

        // Perform raycast
        RaycastHit hit;
        bool hitUI = Physics.Raycast(rayStart, rayDirection, out hit, maxDistance, uiLayerMask);

        if (hitUI)
        {
            // Hit UI element
            rayEnd = hit.point;
            Button button = hit.collider.GetComponent<Button>();
            
            if (button != null && button.interactable)
            {
                // Hovering over interactable button
                currentButton = button;
                isHovering = true;
                lineRenderer.material.color = hoverColor;
            }
            else
            {
                // Hit UI but not interactable button
                currentButton = null;
                isHovering = false;
                lineRenderer.material.color = normalColor;
            }
        }
        else
        {
            // No UI hit
            currentButton = null;
            isHovering = false;
            lineRenderer.material.color = normalColor;
        }

        lineRenderer.SetPosition(1, rayEnd);
    }

    void HandleInput()
    {
        if (currentButton == null || !isHovering) return;

        // Check for trigger press
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            // Button press started
            lineRenderer.material.color = pressColor;
            
            // Haptic feedback
            OVRInput.SetControllerVibration(0.2f, 0.2f, controller);
            
            Debug.Log($"VR Button Pressed: {currentButton.name}");
        }

        // Check for trigger release
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            // Button click completed
            if (currentButton != null && currentButton.interactable)
            {
                currentButton.onClick.Invoke();
                Debug.Log($"VR Button Clicked: {currentButton.name}");
                
                // Haptic feedback for successful click
                OVRInput.SetControllerVibration(0.3f, 0.1f, controller);
            }
            
            // Reset color
            lineRenderer.material.color = isHovering ? hoverColor : normalColor;
        }
    }

    void OnDisable()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }
}