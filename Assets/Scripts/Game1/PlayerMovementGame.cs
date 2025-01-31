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
