using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    private bool isScrolling = false;

    private BackgroundManager backgroundManager;

    // Method to assign the BackgroundManager
    public void SetBackgroundManager(BackgroundManager manager)
    {
        backgroundManager = manager;
        Debug.Log($"{gameObject.name}: BackgroundManager assigned.");
    }

    void Update()
    {
        if (isScrolling)
        {
            // Move the background to the left
            transform.Translate(Vector3.left * GameController.Instance.currentScrollSpeed * Time.deltaTime);

            // Log current position
            // Uncomment the following line if you need to see position updates
            // Debug.Log($"{gameObject.name}: Position X = {transform.position.x}");

            // Check if the background has moved past the left edge (e.g., x <= -15f)
            if (transform.position.x <= -15f)
            {
                // Notify the BackgroundManager to spawn the next background
                if (backgroundManager != null)
                {
                    Debug.Log($"{gameObject.name}: Reached end. Notifying BackgroundManager.");
                    backgroundManager.OnBackgroundReachedEnd();
                }
                else
                {
                    Debug.LogError("BackgroundManager reference not set in BackgroundScroller.");
                }

                // Destroy this background GameObject
                Destroy(gameObject);
                Debug.Log($"{gameObject.name}: Destroyed.");
            }
        }
    }

    // Start the scrolling movement
    public void StartScrolling()
    {
        isScrolling = true;
        Debug.Log($"{gameObject.name}: Started scrolling.");
    }

    // Stop the scrolling movement
    public void StopScrolling()
    {
        isScrolling = false;
        Debug.Log($"{gameObject.name}: Stopped scrolling.");
    }
}
