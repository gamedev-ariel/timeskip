using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public UIManager uiManager;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        CheckOutOfBounds();
    }

    void CheckOutOfBounds()
    {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);
        
        // Check if player is out of camera view (viewport coordinates are normalized 0 to 1)
        if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
        {
            uiManager.ShowTryAgain();
            if (GameController.Instance != null)
            {
                GameController.Instance.EndGame();
                Destroy(gameObject);
            }
        }   
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fish"))
        {
            // Player touched a fish
            uiManager.ShowTryAgain();
            if (GameController.Instance != null)
            {
                GameController.Instance.EndGame();
            }
            else
            {
                Debug.LogError("GameController instance not found.");
            }
        }
        else if (collision.gameObject.CompareTag("RiverBankEnd"))
        {
            // Player reached the end river bank
            uiManager.ShowWellDone();
            if (GameController.Instance != null)
            {
                GameController.Instance.EndGame();
            }
            else
            {
                Debug.LogError("GameController instance not found.");
            }
        }
    }
}
