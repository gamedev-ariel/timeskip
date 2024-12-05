using UnityEngine;

public class Oscillator : MonoBehaviour
{
    [Header("Oscillation Settings")]
    [Tooltip("The speed of oscillation (higher value = faster movement).")]
    public float speed = 1.0f;

    [Tooltip("Amplitude of oscillation for each axis (X, Y, Z).")]
    public Vector3 amplitude = new Vector3(1.0f, 0.0f, 0.0f);

    // Stores the initial position of the GameObject
    private Vector3 initialPosition;

    // Keeps track of the oscillation progress
    private float timeCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Debug.Log(Starting...);

        // Record the initial position of the GameObject
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Updateing...);
        //GetComponent<Transform>().position += new Vector3(1*Time.deltaTime, 0, 0);

        // Increment the time counter based on speed
        timeCounter += Time.deltaTime * speed;

        // Calculate the oscillation offset using sine functions
        Vector3 offset = new Vector3(
            Mathf.Sin(timeCounter) * amplitude.x, // Oscillation on the X-axis
            Mathf.Sin(timeCounter) * amplitude.y, // Oscillation on the Y-axis
            Mathf.Sin(timeCounter) * amplitude.z  // Oscillation on the Z-axis
        );

        // Update the GameObject's position by adding the offset to the initial position
        transform.position = initialPosition + offset;
    }
}
