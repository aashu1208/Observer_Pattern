using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// One-click VR UI setup script that automatically configures Canvas and creates buttons
/// Attach this to any GameObject and run to setup VR UI system
/// </summary>
public class VRUISetup : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool setupOnStart = true;
    public bool createSampleButtons = true;

    [Header("UI Configuration")]
    public Vector3 canvasPosition = new Vector3(0, 1.5f, 2f);
    public Vector3 canvasScale = Vector3.one * 0.001f;

    private Canvas vrCanvas;
    private VRUIManager uiManager;

    void Start()
    {
        if (setupOnStart)
        {
            SetupVRUI();
        }
    }

    [ContextMenu("Setup VR UI")]
    public void SetupVRUI()
    {
        Debug.Log("Setting up VR UI system...");

        EnableExistingCanvas();
        CreateUIManager();
        CreateSampleUI();
        SetupControllerPointers();

        Debug.Log("VR UI setup complete!");
    }

    void EnableExistingCanvas()
    {
        // Find existing Canvas
        vrCanvas = FindObjectOfType<Canvas>();
        
        if (vrCanvas == null)
        {
            Debug.LogError("No Canvas found in scene! Creating new one...");
            CreateNewCanvas();
            return;
        }

        // Configure existing Canvas for VR
        vrCanvas.gameObject.SetActive(true);
        vrCanvas.renderMode = RenderMode.WorldSpace;

        // Position Canvas in VR space
        vrCanvas.transform.position = canvasPosition;
        vrCanvas.transform.localScale = canvasScale;
        vrCanvas.transform.rotation = Quaternion.identity;

        // Setup Canvas components
        CanvasScaler scaler = vrCanvas.GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.dynamicPixelsPerUnit = 10;
        }

        // Add GraphicRaycaster if missing
        if (vrCanvas.GetComponent<GraphicRaycaster>() == null)
        {
            vrCanvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        Debug.Log("Canvas configured for VR");
    }

    void CreateNewCanvas()
    {
        GameObject canvasObj = new GameObject("VR Canvas");
        vrCanvas = canvasObj.AddComponent<Canvas>();
        vrCanvas.renderMode = RenderMode.WorldSpace;
        vrCanvas.transform.position = canvasPosition;
        vrCanvas.transform.localScale = canvasScale;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Set UI layer
        canvasObj.layer = LayerMask.NameToLayer("UI");
    }

    void CreateUIManager()
    {
        // Check if VRUIManager already exists
        uiManager = FindObjectOfType<VRUIManager>();
        
        if (uiManager == null)
        {
            GameObject managerObj = new GameObject("VR UI Manager");
            uiManager = managerObj.AddComponent<VRUIManager>();
        }

        // Assign Canvas to UI Manager
        uiManager.vrCanvas = vrCanvas;

        Debug.Log("VR UI Manager created");
    }

    void CreateSampleUI()
    {
        if (!createSampleButtons || vrCanvas == null) return;

        // Clear existing UI (optional)
        // foreach (Transform child in vrCanvas.transform)
        // {
        //     DestroyImmediate(child.gameObject);
        // }

        // Create main panel
        GameObject panel = CreateUIPanel("Main Panel", new Vector3(0, 0, 0), new Vector2(600, 400));
        
        // Create title text
        CreateUIText(panel, "VR TTS Controls", new Vector3(0, 150, 0), 24);

        // Create buttons
        Button speakBtn = CreateUIButton(panel, "Speak English", new Vector3(-200, 50, 0), () => {
            WindowsTTSManager.Speak("Hello from VR!", interrupt: true);
        });

        Button hindiBtn = CreateUIButton(panel, "Speak Hindi", new Vector3(0, 50, 0), () => {
            WindowsTTSManager.Speak("Namaste! VR mein welcome hai!", interrupt: true);
        });

        Button stopBtn = CreateUIButton(panel, "Stop Speech", new Vector3(200, 50, 0), () => {
            WindowsTTSManager.Stop();
        });

        Button testBtn = CreateUIButton(panel, "Test Button", new Vector3(0, -50, 0), () => {
            Debug.Log("Test button clicked in VR!");
            WindowsTTSManager.Speak("Test successful!", interrupt: true);
        });

        // Create input field
        CreateUIInputField(panel, "Enter custom message...", new Vector3(0, -100, 0));

        // Create status text
        CreateUIText(panel, "Status: Ready", new Vector3(0, -150, 0), 16);

        // Assign buttons to UI Manager
        if (uiManager != null)
        {
            uiManager.speakButton = speakBtn;
            uiManager.hindiButton = hindiBtn;
            uiManager.stopButton = stopBtn;
            uiManager.testButton = testBtn;
        }

        Debug.Log("Sample UI created with TTS controls");
    }

    GameObject CreateUIPanel(string name, Vector3 position, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(vrCanvas.transform);
        panel.transform.localPosition = position;
        panel.transform.localScale = Vector3.one;

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.sizeDelta = size;

        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        return panel;
    }

    Button CreateUIButton(GameObject parent, string text, Vector3 position, System.Action onClick)
    {
        GameObject buttonObj = new GameObject($"Button_{text}");
        buttonObj.transform.SetParent(parent.transform);
        buttonObj.transform.localPosition = position;
        buttonObj.transform.localScale = Vector3.one;

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 40);

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.4f, 0.8f, 0.9f);

        Button button = buttonObj.AddComponent<Button>();
        
        // Add text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        textObj.transform.localPosition = Vector3.zero;
        textObj.transform.localScale = Vector3.one;

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = rect.sizeDelta;

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 14;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;

        // Add collider for VR interaction
        BoxCollider collider = buttonObj.AddComponent<BoxCollider>();
        collider.size = new Vector3(rect.sizeDelta.x, rect.sizeDelta.y, 1);

        // Assign click action
        if (onClick != null)
        {
            button.onClick.AddListener(() => onClick.Invoke());
        }

        return button;
    }

    Text CreateUIText(GameObject parent, string text, Vector3 position, int fontSize)
    {
        GameObject textObj = new GameObject($"Text_{text}");
        textObj.transform.SetParent(parent.transform);
        textObj.transform.localPosition = position;
        textObj.transform.localScale = Vector3.one;

        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 30);

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = fontSize;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;

        return textComponent;
    }

    InputField CreateUIInputField(GameObject parent, string placeholder, Vector3 position)
    {
        GameObject inputObj = new GameObject("Input Field");
        inputObj.transform.SetParent(parent.transform);
        inputObj.transform.localPosition = position;
        inputObj.transform.localScale = Vector3.one;

        RectTransform rect = inputObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 30);

        Image image = inputObj.AddComponent<Image>();
        image.color = Color.white;

        InputField inputField = inputObj.AddComponent<InputField>();

        // Create text component
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputObj.transform);
        textObj.transform.localPosition = Vector3.zero;
        textObj.transform.localScale = Vector3.one;

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = rect.sizeDelta;

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 14;
        textComponent.color = Color.black;
        textComponent.supportRichText = false;

        inputField.textComponent = textComponent;

        // Create placeholder
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(inputObj.transform);
        placeholderObj.transform.localPosition = Vector3.zero;
        placeholderObj.transform.localScale = Vector3.one;

        RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
        placeholderRect.sizeDelta = rect.sizeDelta;

        Text placeholderText = placeholderObj.AddComponent<Text>();
        placeholderText.text = placeholder;
        placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        placeholderText.fontSize = 14;
        placeholderText.color = Color.gray;
        placeholderText.fontStyle = FontStyle.Italic;

        inputField.placeholder = placeholderText;

        return inputField;
    }

    void SetupControllerPointers()
    {
        // Find OVR hand anchors
        GameObject leftHand = GameObject.Find("LeftHandAnchor");
        GameObject rightHand = GameObject.Find("RightHandAnchor");

        if (leftHand != null)
        {
            VRUIPointer leftPointer = leftHand.GetComponent<VRUIPointer>();
            if (leftPointer == null)
            {
                leftPointer = leftHand.AddComponent<VRUIPointer>();
                leftPointer.controller = OVRInput.Controller.LTouch;
            }
        }

        if (rightHand != null)
        {
            VRUIPointer rightPointer = rightHand.GetComponent<VRUIPointer>();
            if (rightPointer == null)
            {
                rightPointer = rightHand.AddComponent<VRUIPointer>();
                rightPointer.controller = OVRInput.Controller.RTouch;
            }
        }

        // Enable EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.gameObject.SetActive(true);
        }

        Debug.Log("Controller pointers setup complete");
    }
}