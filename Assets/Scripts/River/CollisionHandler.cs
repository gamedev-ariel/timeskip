using Unity.VisualScripting;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    public UIManager uiManager;
    private Camera mainCamera;
    private AudioSource audioSource;

    [SerializeField] private AudioClip fishCollisionSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip screwCollectSound;
    
    void Start()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        // Add AudioSource component if it doesn't exist
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private bool PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
            return true;
        }
        return false;
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
            PlaySound(fishCollisionSound);
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
            PlaySound(victorySound);
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
        else if (collision.gameObject.CompareTag("Mushroom"))
        {
            PlaySound(jumpSound);
            GetComponent<PlayerMovementRiver>().Jump();
        }
        else if (collision.gameObject.CompareTag("Screw"))
        {
            PlaySound(screwCollectSound);
            Destroy(collision.gameObject);
            uiManager.CollectScrew();
            if ((uiManager.screwsCollected == uiManager.totalScrews) && (PlaySound(victorySound) == true))
            {
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
}