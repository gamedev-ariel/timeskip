using UnityEngine;

public class PlayerMovementGame : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isOnMushroom = false;

    // --- Add these lines ---
    private static Vector3 savedPosition = Vector3.zero;
    private static bool hasSavedPosition = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        // --- If we have a saved position from a previous visit, restore it ---
        if (hasSavedPosition)
        {
            transform.position = savedPosition;
        }
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveX * speed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // When the player object is disabled or destroyed, remember its position.
    void OnDisable()
    {
        hasSavedPosition = true;
        savedPosition = transform.position;
    }

    // אם תרצה להוסיף לוגיקה לפטרייה וכו', השאר כמות שהוא - לא נגעתי
    public void SetOnMushroom(bool value)
    {
        isOnMushroom = value;
    }
}
