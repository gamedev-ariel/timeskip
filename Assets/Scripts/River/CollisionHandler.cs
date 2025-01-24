using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public UIManager uiManager;

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
