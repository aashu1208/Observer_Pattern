using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// VR UI Manager that handles button interactions and UI positioning for VR
/// Works with Meta XR SDK and provides easy button interaction methods
/// </summary>
public class VRUIManager : MonoBehaviour
{
    [Header("VR UI Settings")]
    public Canvas vrCanvas;
    public Camera vrCamera;
    public float uiDistance = 2f;
    public Vector3 uiOffset = new Vector3(0, 0.5f, 0);

    [Header("Sample UI Buttons")]
    public Button speakButton;
    public Button stopButton;
    public Button hindiButton;
    public Button testButton;

    [Header("UI Text")]
    public Text statusText;
    public InputField messageInput;

    private OVRPlayerController playerController;
    private Transform playerTransform;

    void Start()
    {
        InitializeVRUI();
        SetupButtonListeners();
        PositionUIInVR();
    }

    void InitializeVRUI()
    {
        // Find VR components
        playerController = FindObjectOfType<OVRPlayerController>();
        if (playerController != null)
        {
            playerTransform = playerController.transform;
        }

        // Get or create VR camera reference
        if (vrCamera == null)
        {
            vrCamera = Camera.main;
            if (vrCamera == null)
            {
                GameObject cameraRig = GameObject.Find("OVRCameraRig");
                if (cameraRig != null)
                {
                    vrCamera = cameraRig.GetComponentInChildren<Camera>();
                }
            }
        }

        // Setup Canvas for VR
        if (vrCanvas != null)
        {
            vrCanvas.gameObject.SetActive(true);
            vrCanvas.renderMode = RenderMode.WorldSpace;
            vrCanvas.worldCamera = vrCamera;

            // Scale down the canvas for VR
            vrCanvas.transform.localScale = Vector3.one * 0.001f;
        }

        // Enable EventSystem for VR
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.gameObject.SetActive(true);
        }

        Debug.Log("VR UI Manager initialized successfully!");
    }

    void SetupButtonListeners()
    {
        // TTS Button Interactions
        if (speakButton != null)
        {
            speakButton.onClick.AddListener(() => {
                string text = messageInput != null && !string.IsNullOrEmpty(messageInput.text) 
                    ? messageInput.text 
                    : "Hello from VR!";
                
                WindowsTTSManager.Speak(text, interrupt: true);
                UpdateStatus($"Speaking: {text}");
            });
        }

        if (stopButton != null)
        {
            stopButton.onClick.AddListener(() => {
                WindowsTTSManager.Stop();
                UpdateStatus("Speech stopped");
            });
        }

        if (hindiButton != null)
        {
            hindiButton.onClick.AddListener(() => {
                WindowsTTSManager.Speak("Namaste! Aap VR mein hain!", interrupt: true);
                UpdateStatus("Speaking Hindi message");
            });
        }

        if (testButton != null)
        {
            testButton.onClick.AddListener(() => {
                TestButtonClicked();
            });
        }
    }

    void PositionUIInVR()
    {
        if (vrCanvas == null || vrCamera == null) return;

        // Position UI in front of the player
        Vector3 forward = vrCamera.transform.forward;
        Vector3 position = vrCamera.transform.position + forward * uiDistance + uiOffset;
        
        vrCanvas.transform.position = position;
        vrCanvas.transform.LookAt(vrCamera.transform);
        vrCanvas.transform.Rotate(0, 180, 0); // Face the camera
    }

    void Update()
    {
        // Continuously update UI position to follow camera
        if (vrCanvas != null && vrCamera != null)
        {
            PositionUIInVR();
        }

        // VR Controller Button Input (if needed)
        HandleVRControllerInput();
    }

    void HandleVRControllerInput()
    {
        // Handle Oculus Touch controller inputs
        if (OVRInput.GetDown(OVRInput.Button.One)) // A button
        {
            TriggerSpeakButton();
        }

        if (OVRInput.GetDown(OVRInput.Button.Two)) // B button
        {
            TriggerStopButton();
        }

        if (OVRInput.GetDown(OVRInput.Button.Three)) // X button
        {
            TriggerHindiButton();
        }

        if (OVRInput.GetDown(OVRInput.Button.Four)) // Y button
        {
            TriggerTestButton();
        }
    }

    // Public methods for button triggers
    public void TriggerSpeakButton()
    {
        if (speakButton != null)
        {
            speakButton.onClick.Invoke();
        }
    }

    public void TriggerStopButton()
    {
        if (stopButton != null)
        {
            stopButton.onClick.Invoke();
        }
    }

    public void TriggerHindiButton()
    {
        if (hindiButton != null)
        {
            hindiButton.onClick.Invoke();
        }
    }

    public void TriggerTestButton()
    {
        if (testButton != null)
        {
            testButton.onClick.Invoke();
        }
    }

    public void TestButtonClicked()
    {
        UpdateStatus("Test button clicked!");
        WindowsTTSManager.Speak("Test button successfully clicked in VR!", interrupt: true);
        
        // Add some visual feedback
        if (testButton != null)
        {
            StartCoroutine(ButtonClickEffect(testButton));
        }
    }

    public void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = $"Status: {message}";
        }
        Debug.Log($"VR UI Status: {message}");
    }

    private System.Collections.IEnumerator ButtonClickEffect(Button button)
    {
        Color originalColor = button.image.color;
        button.image.color = Color.green;
        yield return new WaitForSeconds(0.2f);
        button.image.color = originalColor;
    }

    // Method to add new buttons dynamically
    public Button CreateVRButton(string buttonText, Vector3 localPosition, System.Action onClickAction)
    {
        if (vrCanvas == null) return null;

        GameObject buttonObj = new GameObject($"VRButton_{buttonText}");
        buttonObj.transform.SetParent(vrCanvas.transform);
        buttonObj.transform.localPosition = localPosition;
        buttonObj.transform.localScale = Vector3.one;

        // Add Image component
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.3f, 0.8f, 0.8f);

        // Add Button component
        Button button = buttonObj.AddComponent<Button>();
        
        // Add Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        textObj.transform.localPosition = Vector3.zero;
        textObj.transform.localScale = Vector3.one;
        
        Text text = textObj.AddComponent<Text>();
        text.text = buttonText;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        // Set button size
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 50);

        // Add click action
        if (onClickAction != null)
        {
            button.onClick.AddListener(() => onClickAction.Invoke());
        }

        return button;
    }
}