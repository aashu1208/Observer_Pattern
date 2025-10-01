using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    [Header("Example Settings")]
    public float rotationSpeed = 50f;
    public bool enableRotation = true;
    public Color objectColor = Color.white;
    
    [Header("Debug Info")]
    public string currentStatus = "Initialized";
    
    private Renderer objectRenderer;
    private float timer = 0f;

    void Start()
    {
        // Get the renderer component if available
        objectRenderer = GetComponent<Renderer>();
        
        // Set initial color if renderer exists
        if (objectRenderer != null && objectRenderer.material != null)
        {
            objectRenderer.material.color = objectColor;
        }
        
        currentStatus = "Started";
        Debug.Log($"ExampleScript started on {gameObject.name}");
    }

    void Update()
    {
        timer += Time.deltaTime;
        
        // Rotate the object if enabled
        if (enableRotation)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        
        // Update status every 5 seconds
        if (timer >= 5f)
        {
            currentStatus = $"Running for {timer:F1} seconds";
            timer = 0f;
            Debug.Log($"ExampleScript update: {currentStatus}");
        }
    }
    
    // Public method that can be called from other scripts
    public void ChangeColor(Color newColor)
    {
        objectColor = newColor;
        if (objectRenderer != null && objectRenderer.material != null)
        {
            objectRenderer.material.color = objectColor;
        }
        Debug.Log($"Color changed to {newColor}");
    }
    
    // Public method to toggle rotation
    public void ToggleRotation()
    {
        enableRotation = !enableRotation;
        currentStatus = enableRotation ? "Rotation Enabled" : "Rotation Disabled";
        Debug.Log(currentStatus);
    }
    
    void OnDestroy()
    {
        Debug.Log($"ExampleScript destroyed on {gameObject.name}");
    }
}