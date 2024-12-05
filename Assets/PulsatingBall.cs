using UnityEngine;

public class PulsatingBall : MonoBehaviour
{
    [Header("Pulsating Settings")]
    [Tooltip("The speed of the pulsation (higher value = faster pulsation).")]
    public float pulsationSpeed = 2.0f;

    [Tooltip("The maximum scale factor for the pulsation.")]
    public float maxScale = 1.5f;

    [Tooltip("The minimum scale factor for the pulsation.")]
    public float minScale = 0.5f;

    // Original scale of the object
    private Vector3 originalScale;

    void Start()
    {
        // Store the initial scale of the GameObject
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Calculate the scale factor based on a sine wave
        float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * pulsationSpeed) + 1) / 2);

        // Apply the scale factor uniformly to all axes
        transform.localScale = originalScale * scale;
    }
}
