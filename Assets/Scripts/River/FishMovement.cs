using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("Vertical Movement")]
    public float initialUpSpeed = 5f;  // Initial upward "shoot" velocity
    public float gravity = 9.81f;      // Gravity to simulate downward acceleration

    [Header("Destruction")]
    public float destroyY = -10f;      // Y position at which to destroy the fish

    [Header("Optional: Horizontal Sway")]
    public bool enableSway = false;
    public float swayFrequency = 2f;
    public float swayAmplitude = 0.5f;

    private float verticalVelocity;     // Current vertical velocity

    void Start()
    {
        // Ensure Rigidbody is kinematic if present
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Initialize vertical velocity with a slight random variation for natural behavior
        verticalVelocity = initialUpSpeed + Random.Range(-1f, 1f);
    }

    void Update()
    {
        // 1. Move left based on GameController's scrollSpeed
        transform.Translate(Vector3.left * GameController.Instance.currentScrollSpeed * Time.deltaTime, Space.World);

        // ...existing code...
        // 2. Apply gravity to vertical velocity
        verticalVelocity -= gravity * Time.deltaTime;

        // 3. Move vertically based on current vertical velocity
        transform.Translate(Vector3.up * verticalVelocity * Time.deltaTime, Space.World);

        // 4. Optional: Add horizontal sway for natural movement
        if (enableSway)
        {
            float sway = Mathf.Sin(Time.time * swayFrequency) * swayAmplitude * Time.deltaTime;
            transform.Translate(Vector3.right * sway, Space.World);
        }

        // 5. Destroy fish if it goes below the destruction threshold
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}