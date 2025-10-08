using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// VR UI Pointer for interacting with UI elements using controller rays
/// Attach this to the controller GameObjects for ray-based UI interaction
/// </summary>
public class VRUIPointer : MonoBehaviour
{
    [Header("Pointer Settings")]
    public LineRenderer laserLine;
    public GameObject pointer;
    public float maxDistance = 10f;
    public LayerMask uiLayerMask = 1 << 5; // UI layer
    
    [Header("Visual Settings")]
    public Color normalColor = Color.blue;
    public Color highlightColor = Color.red;
    public float lineWidth = 0.01f;

    [Header("Controller Settings")]
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    public OVRInput.Button triggerButton = OVRInput.Button.PrimaryIndexTrigger;

    private Camera vrCamera;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;
    private PointerEventData pointerEventData;
    private Button currentButton;
    private bool isPointing = false;

    void Start()
    {
        InitializePointer();
        SetupLaser();
    }

    void InitializePointer()
    {
        // Find VR camera
        vrCamera = Camera.main;
        if (vrCamera == null)
        {
            vrCamera = FindObjectOfType<Camera>();
        }

        // Get EventSystem
        eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }

        // Find Canvas with GraphicRaycaster
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
                if (graphicRaycaster != null) break;
            }
        }

        Debug.Log("VR UI Pointer initialized");
    }

    void SetupLaser()
    {
        // Create laser line if not assigned
        if (laserLine == null)
        {
            GameObject laserObj = new GameObject("LaserLine");
            laserObj.transform.SetParent(transform);
            laserObj.transform.localPosition = Vector3.zero;
            
            laserLine = laserObj.AddComponent<LineRenderer>();
        }

        // Configure laser line
        laserLine.material = new Material(Shader.Find("Unlit/Color"));
        laserLine.material.color = normalColor;
        laserLine.startWidth = lineWidth;
        laserLine.endWidth = lineWidth;
        laserLine.positionCount = 2;
        laserLine.useWorldSpace = true;

        // Create pointer dot if not assigned
        if (pointer == null)
        {
            pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pointer.name = "UIPointer";
            pointer.transform.localScale = Vector3.one * 0.02f;
            pointer.GetComponent<Collider>().enabled = false;
            
            Renderer renderer = pointer.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Unlit/Color"));
            renderer.material.color = normalColor;
        }

        pointer.SetActive(false);
    }

    void Update()
    {
        HandlePointerInput();
        UpdateLaser();
    }

    void HandlePointerInput()
    {
        // Check if controller is connected
        if (!OVRInput.IsControllerConnected(controller))
        {
            isPointing = false;
            SetLaserActive(false);
            return;
        }

        // Show laser when pointing
        Vector3 forward = transform.forward;
        isPointing = true;
        SetLaserActive(true);

        // Raycast for UI interaction
        RaycastHit hit;
        bool hitUI = false;

        if (Physics.Raycast(transform.position, forward, out hit, maxDistance, uiLayerMask))
        {
            hitUI = true;
            UpdateLaserHit(hit.point);
            HandleUIInteraction(hit);

            // Check for button trigger
            if (OVRInput.GetDown(triggerButton, controller))
            {
                TriggerUIElement(hit);
            }
        }
        else
        {
            // No hit, show laser at max distance
            Vector3 endPoint = transform.position + forward * maxDistance;
            UpdateLaserHit(endPoint);
            currentButton = null;
        }

        // Update laser color based on interaction
        laserLine.material.color = hitUI && currentButton != null ? highlightColor : normalColor;
        if (pointer != null)
        {
            pointer.GetComponent<Renderer>().material.color = laserLine.material.color;
        }
    }

    void UpdateLaser()
    {
        if (laserLine != null && isPointing)
        {
            laserLine.SetPosition(0, transform.position);
        }
    }

    void UpdateLaserHit(Vector3 hitPoint)
    {
        if (laserLine != null)
        {
            laserLine.SetPosition(1, hitPoint);
        }

        if (pointer != null)
        {
            pointer.SetActive(true);
            pointer.transform.position = hitPoint;
        }
    }

    void SetLaserActive(bool active)
    {
        if (laserLine != null)
        {
            laserLine.enabled = active;
        }

        if (pointer != null)
        {
            pointer.SetActive(active);
        }
    }

    void HandleUIInteraction(RaycastHit hit)
    {
        // Get UI element
        Button button = hit.collider.GetComponent<Button>();
        if (button == null)
        {
            button = hit.collider.GetComponentInParent<Button>();
        }

        currentButton = button;

        // Visual feedback for hovering
        if (currentButton != null)
        {
            // Add hover effect if needed
            Debug.Log($"Hovering over button: {currentButton.name}");
        }
    }

    void TriggerUIElement(RaycastHit hit)
    {
        if (currentButton != null && currentButton.interactable)
        {
            Debug.Log($"Triggering button: {currentButton.name}");
            currentButton.onClick.Invoke();

            // Haptic feedback
            OVRInput.SetControllerVibration(0.1f, 0.1f, controller);

            // Visual feedback
            StartCoroutine(ButtonPressEffect());
        }
    }

    private System.Collections.IEnumerator ButtonPressEffect()
    {
        if (pointer != null)
        {
            Vector3 originalScale = pointer.transform.localScale;
            pointer.transform.localScale = originalScale * 1.5f;
            yield return new WaitForSeconds(0.1f);
            pointer.transform.localScale = originalScale;
        }
    }

    // Alternative method using Unity's EventSystem
    void UpdateEventSystem()
    {
        if (eventSystem == null || graphicRaycaster == null) return;

        // Create pointer event data
        if (pointerEventData == null)
        {
            pointerEventData = new PointerEventData(eventSystem);
        }

        // Convert 3D ray to screen point for UI raycasting
        Vector3 screenPoint = vrCamera.WorldToScreenPoint(transform.position + transform.forward * 2f);
        pointerEventData.position = screenPoint;

        // Perform UI raycast
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, raycastResults);

        // Handle results
        if (raycastResults.Count > 0)
        {
            GameObject hitObject = raycastResults[0].gameObject;
            Button button = hitObject.GetComponent<Button>();
            
            if (button != null && OVRInput.GetDown(triggerButton, controller))
            {
                button.onClick.Invoke();
            }
        }
    }

    void OnDisable()
    {
        SetLaserActive(false);
    }
}