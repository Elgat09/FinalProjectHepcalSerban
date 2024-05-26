using UnityEngine;

public class MaterialExample : MonoBehaviour
{
    // Reference to the material that will be applied
    public Material newMaterial;

    // Variable to store the original material
    private Material originalMaterial;

    // Variable to store the renderer
    private Renderer targetRenderer;

    void Start()
    {
        // Get the Renderer component
        targetRenderer = GetComponent<Renderer>();

        // Make sure the renderer and new material are assigned
        if (targetRenderer == null || newMaterial == null)
        {
            Debug.LogError("Please assign the renderer and new material!");
            return;
        }

        // Save the original material
        originalMaterial = new Material(targetRenderer.sharedMaterial);

        // Apply the new material
        targetRenderer.sharedMaterial = newMaterial;
    }

    void OnDestroy()
    {
        // Restore the original material when the game stops or the object is destroyed
        if (targetRenderer != null && originalMaterial != null)
        {
            targetRenderer.sharedMaterial = originalMaterial;
        }
    }
}
