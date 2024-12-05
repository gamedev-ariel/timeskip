using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Rotation speed in degrees per second.")]
    public float rotationSpeed = 45.0f;

    [Tooltip("Axis of rotation (1 = rotate, 0 = no rotation).")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0); // Default: Y-axis rotation

    void Update()
    {
        // Calculate the rotation amount for this frame
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Apply rotation to the GameObject
        transform.Rotate(rotationAxis.normalized * rotationAmount);
    }
}
