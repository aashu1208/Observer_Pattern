using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Simple and robust VR UI setup that works with Unity 2022.3+
/// Just enable your Canvas and add basic buttons for TTS
/// </summary>
public class SimpleVRUISetup : MonoBehaviour
{
    [Header("VR UI Configuration")]
    public bool setupOnStart = true;
    public Vector3 canvasPosition = new Vector3(0, 1.5f, 2f);
    public float canvasScale = 0.001f;

    void Start()
    {
        if (setupOnStart)
        {
            SimpleSetup();
        }
    }

    [ContextMenu("Simple VR UI Setup")]
    public void SimpleSetup()
    {
        Debug.Log("🚀 Starting Simple VR UI Setup...");

        try
        {
            SetupCanvas();
            EnableEventSystem();
            AddBasicButtons();
            SetupControllers();
            Debug.Log("✅ VR UI Setup Complete!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Setup failed: {ex.Message}");
        }
    }

    [ContextMenu("Quick Enable Canvas")]
    public void QuickEnableCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>(true);
        if (canvas != null)
        {
            canvas.gameObject.SetActive(true);
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.position = canvasPosition;
            canvas.transform.localScale = Vector3.one * canvasScale;
            Debug.Log("✅ Canvas manually enabled and configured!");
        }
        else
        {
            Debug.LogError("❌ No Canvas found!");
        }
    }

    void SetupCanvas()
    {
        // Find existing Canvas (including inactive ones)
        Canvas canvas = FindObjectOfType<Canvas>(true);
        if (canvas == null)
        {
            Debug.LogError("❌ No Canvas found! Please create a Canvas first.");
            return;
        }

        Debug.Log($"📝 Found Canvas: {canvas.name}, Active: {canvas.gameObject.activeSelf}");

        // Enable Canvas GameObject first
        canvas.gameObject.SetActive(true);
        
        // Configure Canvas for VR
        canvas.renderMode = RenderMode.WorldSpace;
        
        // Position Canvas for VR
        Transform canvasTransform = canvas.transform;
        canvasTransform.position = canvasPosition;
        canvasTransform.localScale = Vector3.one * canvasScale;
        canvasTransform.rotation = Quaternion.identity;

        // Add GraphicRaycaster if missing
        if (canvas.GetComponent<GraphicRaycaster>() == null)
        {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        Debug.Log($"✅ Canvas '{canvas.name}' configured for VR and enabled");
    }

    void EnableEventSystem()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>(true); // Find inactive objects too
        if (eventSystem != null)
        {
            eventSystem.gameObject.SetActive(true);
            Debug.Log("✅ EventSystem found and enabled");
        }
        else
        {
            // Create new EventSystem if none exists
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Debug.Log("✅ New EventSystem created");
        }
    }

    void AddBasicButtons()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        // Create main panel
        GameObject panel = CreatePanel(canvas.transform, "VR UI Panel", Vector3.zero, new Vector2(400, 300));

        // Create simple buttons using Unity's default UI
        CreateSimpleButton(panel.transform, "Speak", new Vector3(-100, 50, 0), () => {
            WindowsTTSManager.Speak("Hello from VR!");
            Debug.Log("Speak button clicked!");
        });

        CreateSimpleButton(panel.transform, "Hindi", new Vector3(100, 50, 0), () => {
            WindowsTTSManager.Speak("Namaste! VR mein welcome!");
            Debug.Log("Hindi button clicked!");
        });

        CreateSimpleButton(panel.transform, "Stop", new Vector3(0, -50, 0), () => {
            WindowsTTSManager.Stop();
            Debug.Log("Stop button clicked!");
        });

        Debug.Log("✅ Basic buttons created");
    }

    GameObject CreatePanel(Transform parent, string name, Vector3 position, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);
        panel.transform.localPosition = position;
        panel.transform.localScale = Vector3.one;

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.sizeDelta = size;

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        return panel;
    }

    Button CreateSimpleButton(Transform parent, string buttonText, Vector3 position, System.Action onClick)
    {
        // Create button GameObject
        GameObject buttonObj = new GameObject($"Button_{buttonText}");
        buttonObj.transform.SetParent(parent);
        buttonObj.transform.localPosition = position;
        buttonObj.transform.localScale = Vector3.one;

        // Setup RectTransform
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(120, 40);

        // Add Image component
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.3f, 0.5f, 0.8f, 1f);

        // Add Button component
        Button button = buttonObj.AddComponent<Button>();

        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        textObj.transform.localPosition = Vector3.zero;
        textObj.transform.localScale = Vector3.one;

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = buttonRect.sizeDelta;

        Text text = textObj.AddComponent<Text>();
        text.text = buttonText;
        text.fontSize = 16;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        // Try to get font - use fallback if needed
        try
        {
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        catch
        {
            // Fallback - will use Unity's default font
            Debug.LogWarning("⚠️ Using default font for text");
        }

        // Add collider for VR interaction
        BoxCollider collider = buttonObj.AddComponent<BoxCollider>();
        collider.size = new Vector3(buttonRect.sizeDelta.x, buttonRect.sizeDelta.y, 1);

        // Add click listener
        if (onClick != null)
        {
            button.onClick.AddListener(() => onClick.Invoke());
        }

        return button;
    }

    void SetupControllers()
    {
        // Add simple controller pointers
        GameObject leftHand = GameObject.Find("LeftHandAnchor");
        GameObject rightHand = GameObject.Find("RightHandAnchor");

        if (leftHand != null)
        {
            AddPointerToController(leftHand, OVRInput.Controller.LTouch);
        }

        if (rightHand != null)
        {
            AddPointerToController(rightHand, OVRInput.Controller.RTouch);
        }

        Debug.Log("✅ Controller pointers added");
    }

    void AddPointerToController(GameObject handAnchor, OVRInput.Controller controller)
    {
        // Check if pointer already exists
        if (handAnchor.GetComponent<SimpleVRPointer>() != null) return;

        SimpleVRPointer pointer = handAnchor.AddComponent<SimpleVRPointer>();
        pointer.controller = controller;
    }
}

/// <summary>
/// Simple VR pointer for UI interaction without complex dependencies
/// </summary>
public class SimpleVRPointer : MonoBehaviour
{
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    public float maxDistance = 5f;
    
    private LineRenderer laser;
    private GameObject dot;

    void Start()
    {
        SetupLaser();
    }

    void SetupLaser()
    {
        // Create laser line
        GameObject laserObj = new GameObject("Laser");
        laserObj.transform.SetParent(transform);
        laserObj.transform.localPosition = Vector3.zero;

        laser = laserObj.AddComponent<LineRenderer>();
        laser.material = new Material(Shader.Find("Unlit/Color"));
        laser.material.color = Color.blue;
        laser.startWidth = 0.01f;
        laser.endWidth = 0.01f;
        laser.positionCount = 2;

        // Create dot
        dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dot.name = "LaserDot";
        dot.transform.localScale = Vector3.one * 0.02f;
        dot.GetComponent<Collider>().enabled = false;
        dot.GetComponent<Renderer>().material.color = Color.blue;
        dot.SetActive(false);
    }

    void Update()
    {
        if (!OVRInput.IsControllerConnected(controller)) return;

        Vector3 start = transform.position;
        Vector3 forward = transform.forward;
        Vector3 end = start + forward * maxDistance;

        // Update laser
        laser.SetPosition(0, start);
        laser.SetPosition(1, end);

        // Raycast for UI
        RaycastHit hit;
        if (Physics.Raycast(start, forward, out hit, maxDistance))
        {
            Button button = hit.collider.GetComponent<Button>();
            if (button != null)
            {
                laser.material.color = Color.red;
                dot.transform.position = hit.point;
                dot.SetActive(true);

                // Click with trigger
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
                {
                    button.onClick.Invoke();
                    Debug.Log($"Button clicked: {button.name}");
                }
                return;
            }
        }

        // No UI hit
        laser.material.color = Color.blue;
        dot.SetActive(false);
    }
}