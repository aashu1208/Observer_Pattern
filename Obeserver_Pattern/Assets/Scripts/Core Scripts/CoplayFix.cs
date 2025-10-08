using UnityEngine;

/// <summary>
/// Temporary fix for Coplay plugin NullReference error during builds
/// Add this to a GameObject in the scene if you're getting Coplay errors
/// </summary>
public class CoplayFix : MonoBehaviour
{
    void Awake()
    {
        // Disable any Coplay components that might cause NullReference
        try
        {
            var coplayObjects = FindObjectsOfType<MonoBehaviour>();
            foreach (var obj in coplayObjects)
            {
                if (obj.GetType().Name.Contains("Coplay"))
                {
                    obj.enabled = false;
                    Debug.Log($"Disabled Coplay component: {obj.GetType().Name}");
                }
            }
        }
        catch
        {
            // Ignore any errors from Coplay
        }
    }
}