//using UnityEngine;

//public class PlayerMovementGame : MonoBehaviour
//{
//public float speed = 5f;
//public float jumpForce = 10f;
//private Rigidbody2D rb;
//private bool isGrounded = false; // default not on ground

//public Transform leftWall;
//public Transform rightWall;
//public float wallBounceForce = 5f; // bounce Force

//void Start()
//{
//    rb = GetComponent<Rigidbody2D>();
//    rb.freezeRotation = true;
//    rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
//    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
//}

//void FixedUpdate()
//{
//    float moveX = Input.GetAxis("Horizontal");

//    // Prevent exits and bounces from walls
//    if (transform.position.x < leftWall.position.x && moveX < 0)
//    {
//        moveX = 0;
//        BounceFromWall(Vector2.right);
//    }
//    else if (transform.position.x > rightWall.position.x && moveX > 0)
//    {
//        moveX = 0;
//        BounceFromWall(Vector2.left);
//    }

//    rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);
//}

//void Update()
//{
//    if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
//    {
//        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
//        isGrounded = false;
//        Debug.Log("jump");
//    }
//}

//void OnCollisionStay2D(Collision2D collision)
//{
//    // player is on ground
//    foreach (ContactPoint2D contact in collision.contacts)
//    {
//        if (Vector2.Dot(contact.normal, Vector2.up) > 0.5f)
//        {
//            isGrounded = true;
//            //Debug.Log("on ground");
//            break;
//        }
//    }
//}

//void OnCollisionExit2D(Collision2D collision)
//{
//    isGrounded = false;
//    //Debug.Log("player not on ground");
//}

//void BounceFromWall(Vector2 direction)
//{
//    rb.AddForce(direction * wallBounceForce, ForceMode2D.Impulse);
//}
//}


//using UnityEngine;

//public class PlayerMovementGame : MonoBehaviour
//{
//    public float speed = 5f;
//    public float jumpForce = 10f;
//    private Rigidbody2D rb;
//    private bool isGrounded = false; // ����� ����: ����� �� �� �����

//    public Transform leftWall;
//    public Transform rightWall;
//    public float wallBounceForce = 5f; // ��� ����� �����

//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        rb.freezeRotation = true;
//        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
//        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
//    }

//    void FixedUpdate()
//    {
//        Move();
//    }

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
//        {
//            Jump();
//        }
//    }

//    private void Move()
//    {
//        float moveX = Input.GetAxisRaw("Horizontal"); // ����� �- GetAxisRaw ������ ����� �������

//        // ����� ������� �� ������ ����� ������
//        if (transform.position.x < leftWall.position.x && moveX < 0)
//        {
//            moveX = 0;
//            BounceFromWall(Vector2.right);
//        }
//        else if (transform.position.x > rightWall.position.x && moveX > 0)
//        {
//            moveX = 0;
//            BounceFromWall(Vector2.left);
//        }

//        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y); // ����� ������ �- velocity
//    }

//    private void Jump()
//    {
//        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
//        isGrounded = false;
//        Debug.Log("Jump");
//    }

//    void OnCollisionStay2D(Collision2D collision)
//    {
//        foreach (ContactPoint2D contact in collision.contacts)
//        {
//            if (Vector2.Dot(contact.normal, Vector2.up) > 0.5f)
//            {
//                isGrounded = true;
//                break;
//            }
//        }
//    }

//    void OnCollisionExit2D(Collision2D collision)
//    {
//        isGrounded = false;
//    }

//    void BounceFromWall(Vector2 direction)
//    {
//        rb.AddForce(direction * wallBounceForce, ForceMode2D.Impulse);
//    }
//}



using UnityEngine;

public class PlayerMovementGame : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isOnMushroom = false; // משתנה לבדוק אם השחקן על פטרייה

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // תנועה ימינה/שמאלה
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // קפיצה
        }
    }

    // כאשר השחקן נוגע בפטרייה
    public void SetOnMushroom(bool value)
    {
        isOnMushroom = value;
    }
}
