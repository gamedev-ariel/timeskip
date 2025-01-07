using UnityEngine;

public class PlayerMovement_icy : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded = true;

    public Transform leftWall;
    public Transform rightWall;

    public float wallBounceForce = 5f; //Bounce of wall

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");

        if (transform.position.x < leftWall.position.x && moveX < 0)
        {
            moveX = 0; //stop leftWall
            BounceFromWall(Vector2.right);
        }
        else if (transform.position.x > rightWall.position.x && moveX > 0)
        {
            moveX = 0; //stop rightWall
            BounceFromWall(Vector2.left); 
        }

        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");

        //animation movement (does not work) (for future updates)
        animator.SetBool("MovingLeft", moveX < 0);
        animator.SetBool("MovingRight", moveX > 0);

        //Jump on space
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetBool("Jumping", true);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Vector2.Dot(contact.normal, Vector2.up) > 0.5f)
            {
                isGrounded = true;
                animator.SetBool("Jumping", false);
                break;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }

    void BounceFromWall(Vector2 direction)
    {
        rb.AddForce(direction * wallBounceForce, ForceMode2D.Impulse);
    }
}
