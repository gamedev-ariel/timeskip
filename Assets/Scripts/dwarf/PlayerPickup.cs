using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPosition; // נקודה ליד השחקן שבה יוחזק החפץ
    private GameObject heldObject = null; // משתנה ששומר את החפץ המוחזק
    private Rigidbody2D heldRb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
            {
                TryPickup();
            }
            else
            {
                DropObject();
            }
        }
    }

    void TryPickup()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Pickup")) // נניח שהאובייקטים שניתן להרים מסומנים עם תגית "Pickup"
            {
                heldObject = collider.gameObject;
                heldRb = heldObject.GetComponent<Rigidbody2D>();

                if (heldRb != null)
                {
                    heldRb.isKinematic = true;
                    heldObject.transform.position = holdPosition.position;
                    heldObject.transform.SetParent(transform);
                }
                break;
            }
        }
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            heldRb.isKinematic = false;
            heldObject.transform.SetParent(null);
            heldObject = null;
        }
    }
}
