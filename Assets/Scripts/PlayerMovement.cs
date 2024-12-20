using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Sprite[] ACharDown; // 4 sprites for Down animation
    public Sprite[] ACharUp;   // 4 sprites for Up animation
    public Sprite[] ACharLeft; // 4 sprites for Left animation
    public Sprite[] ACharRight; // 4 sprites for Right animation

    public float moveSpeed = 5f; // Movement speed

    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private Sprite[] currentDirectionSprites;
    private int animationIndex = 0;
    private float animationTimer = 0f;
    private Rigidbody2D rb;
    public float animationSpeed = 0.2f; // Time between frames

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentDirectionSprites = ACharDown; // Default direction
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Get movement input
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Determine direction and set current sprites
        if (moveInput.x > 0)
        {
            currentDirectionSprites = ACharRight;
        }
        else if (moveInput.x < 0)
        {
            currentDirectionSprites = ACharLeft;
        }
        else if (moveInput.y > 0)
        {
            currentDirectionSprites = ACharUp;
        }
        else if (moveInput.y < 0)
        {
            currentDirectionSprites = ACharDown;
        }

        // Handle animation only if moving
        if (moveInput != Vector2.zero)
        {
            AnimateMovement();
        }
        else
        {
            // Reset to standing still frame
            animationIndex = 0;
            spriteRenderer.sprite = currentDirectionSprites[0];
        }
    }

    private void FixedUpdate()
    {
        // Update Rigidbody linear velocity (not MovePosition)
        rb.linearVelocity = moveInput.normalized * moveSpeed;
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
}
