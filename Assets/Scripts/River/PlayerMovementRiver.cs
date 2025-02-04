
using UnityEngine;

public class PlayerMovementRiver : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isOnMushroom = false; 

    private float defaultGravityScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        defaultGravityScale = rb.gravityScale; // Save the original gravity scale
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); 
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

                if (Input.GetKey(KeyCode.DownArrow))
        {
            Debug.Log("Down Arrow is held down.");
            rb.gravityScale = defaultGravityScale * 6;
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
        }
    }
    public void Jump()  
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); 
    }

}
