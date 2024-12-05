using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleVisibility : MonoBehaviour
{
    [Header("Input Action Settings")]
    [Tooltip("The input action used to toggle visibility.")]
    public InputAction toggleAction;

    // Reference to the Renderer component
    private Renderer objectRenderer;

    void Awake()
    {
        // Get the Renderer component
        objectRenderer = GetComponent<Renderer>();

        // Enable the input action
        toggleAction.Enable();
    }

    void OnDestroy()
    {
        // Disable the input action when the object is destroyed
        toggleAction.Disable();
    }

    void Update()
    {
        // Check if the toggle action was triggered
        if (toggleAction.WasPressedThisFrame())
        {
            // Toggle the visibility by enabling/disabling the Renderer
            objectRenderer.enabled = !objectRenderer.enabled;
        }
    }
}
