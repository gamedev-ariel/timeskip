using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // Singleton instance
    public static GameController Instance { get; private set; }

    [Header("Game Settings")]
    public float startDelay = 5f;
    public float gameDuration = 90f; // 1.5 minutes
    public float scrollSpeed = 2f; // Centralized scrollSpeed

    private float timer;
    private bool gameStarted = false;
    private bool gameEnded = false;

    [Header("References")]
    public UIManager uiManager;
    public BackgroundManager backgroundManager; // Reference to BackgroundManager

    void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes if necessary
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    void Start()
    {
        timer = 0f;
        uiManager.ShowStartCountdown(startDelay);
    }

    void Update()
    {
        if (!gameStarted)
        {
            // Countdown before starting the game
            timer += Time.deltaTime;
            uiManager.UpdateCountdown(startDelay - timer);
            if (timer >= startDelay)
            {
                StartGame();
            }
        }
        else if (!gameEnded)
        {
            // Game timer
            timer += Time.deltaTime;
            uiManager.UpdateGameTimer(gameDuration - timer);
            if (timer >= gameDuration)
            {
                EndGame();
            }
        }
    }

    void StartGame()
    {
        gameStarted = true;
        uiManager.HideCountdown();

        // Start background spawning
        if (backgroundManager != null)
        {
            backgroundManager.StartSpawning(scrollSpeed);
            Debug.Log("GameController: Started background spawning.");
        }
        else
        {
            Debug.LogError("BackgroundManager not assigned in GameController.");
        }

        // Start spawning fish
        FishSpawner[] fishSpawners = FindObjectsOfType<FishSpawner>();
        foreach (FishSpawner spawner in fishSpawners)
        {
            spawner.StartSpawning();
            Debug.Log("GameController: Started FishSpawner.");
        }
    }

    public void EndGame()
    {
        if (gameEnded) return; // Prevent multiple calls
        gameEnded = true;

        // Stop spawning fish
        FishSpawner[] fishSpawners = FindObjectsOfType<FishSpawner>();
        foreach (FishSpawner spawner in fishSpawners)
        {
            spawner.StopSpawning();
            Debug.Log("GameController: Stopped FishSpawner.");
        }

        // Optionally, stop background scrolling if needed
        if (backgroundManager != null)
        {
            backgroundManager.StopSpawning(); // Implement if necessary
            Debug.Log("GameController: Stopped BackgroundManager spawning.");
        }

        // Optionally, display end game UI
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("GameController: Game restarted.");
    }
}
