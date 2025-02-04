using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public Transform holdPosition;
    private GameObject heldObject;
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
            string objectID = collider.gameObject.name;

            // אם זה Machine - שמור אותו כ"אסוף" ומחק מהסצנה
            if (collider.CompareTag("Machine"))
            {
                PlayerSpawnManager2.Instance.CollectMachine(objectID);
                Destroy(collider.gameObject);
                return;
            }

            // אם זה Pickup - אסוף והזז
            if (collider.CompareTag("Pickup"))
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

            // שמירת מיקום חדש
            PlayerSpawnManager2.Instance.SavePickupPosition(heldObject.name, heldObject.transform.position);

            heldObject = null;
        }
    }
}

