
using UnityEngine;

public class PlayerMovementForest : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isOnMushroom = false; 
    private bool hasStartedLogging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); 
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);

        // Forest is not a minigame; no minigame start logging here

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }
    public void Jump()  
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); 
    }

}
