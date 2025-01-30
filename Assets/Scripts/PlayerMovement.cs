using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Sprite[] ACharIdle;  // Idle sprites
    public Sprite[] ACharLeft;  // Left movement sprites
    public Sprite[] ACharRight; // Right movement sprites

    public float moveSpeed = 5f;    // Horizontal movement speed
    public float jumpForce = 10f;  // Force applied for jumping
    public float animationSpeed = 0.2f; // Time between animation frames

    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private Sprite[] currentDirectionSprites;
    private int animationIndex = 0;
    private float animationTimer = 0f;
    private Rigidbody2D rb;

    private bool isGrounded = true; // Check if the player is on the ground
    public Transform groundCheck;  // Position to check if grounded
    public float groundCheckRadius = 0.2f; // Radius for ground check
    public LayerMask groundLayer;  // Layer considered as ground

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentDirectionSprites = ACharIdle; // Default idle animation
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Get horizontal input
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);

        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Determine direction and set current sprites
        if (moveInput.x > 0)
        {
            currentDirectionSprites = ACharRight;
        }
        else if (moveInput.x < 0)
        {
            currentDirectionSprites = ACharLeft;
        }
        else
        {
            currentDirectionSprites = ACharIdle;
        }

        // Handle animation if moving horizontally
        if (moveInput.x != 0)
        {
            AnimateMovement();
        }
        else
        {
            // Reset to idle frame
            animationIndex = 0;
            spriteRenderer.sprite = currentDirectionSprites[0];
        }
    }

    private void FixedUpdate()
    {
        // Apply horizontal movement
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void AnimateMovement()
    {
        // Update animation frame
        animationTimer += Time.deltaTime;
        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f;
            animationIndex = (animationIndex + 1) % currentDirectionSprites.Length;
            spriteRenderer.sprite = currentDirectionSprites[animationIndex];
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize ground check radius in editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
