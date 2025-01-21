//using UnityEngine;

//public class BouncyMushroom : MonoBehaviour
//{
//    public float bounceForce = 15f; 

//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        if (collision.gameObject.CompareTag("Player"))
//        {
//            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();

//            if (rb != null)
//            {
//                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
//                rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
//            }
//        }
//    }
//}

using UnityEngine;

public class BouncyMushroom : MonoBehaviour
{
    public float bounceMultiplier = 2f; // מכפיל קפיצה

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            PlayerMovementGame playerMovement = collision.gameObject.GetComponent<PlayerMovementGame>();

            if (rb != null && playerMovement != null)
            {
                // קבלת כוח הקפיצה של השחקן
                float playerJumpForce = playerMovement.jumpForce;

                // איפוס המהירות האנכית של השחקן
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

                // הוספת כוח קפיצה כפול מזה של השחקן
                rb.AddForce(Vector2.up * playerJumpForce * bounceMultiplier, ForceMode2D.Impulse);

                // עדכון המידע שהשחקן על פטרייה
                playerMovement.SetOnMushroom(true);
            }
        }
    }

    // ביטול המגע עם הפטרייה כאשר השחקן לא על הפטרייה יותר
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovementGame playerMovement = collision.gameObject.GetComponent<PlayerMovementGame>();
            if (playerMovement != null)
            {
                playerMovement.SetOnMushroom(false);
            }
        }
    }
}
