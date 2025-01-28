using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // Singleton instance
    public static GameController Instance { get; private set; }

    [Header("Game Settings")]
    public float startDelay = 5f;
    public float gameDuration = 90f; // 1.5 minutes
    public float minScrollSpeed = 0.5f;
    public float maxScrollSpeed = 2f;
    public float currentScrollSpeed;
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
    // Find UIManager in scene if reference is lost
    if (uiManager == null)
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in scene!");
            return;
        }
    }
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

            // Update speed based on elapsed time
            float normalizedTime = timer / gameDuration;
            currentScrollSpeed = Mathf.Lerp(minScrollSpeed, maxScrollSpeed, normalizedTime);
            
            // // Update components with new speed
            // if (backgroundManager != null)
            // {
            //     backgroundManager.UpdateSpeed(currentScrollSpeed);
            // }

            // // Update fish spawners with new speed
            // FishSpawner[] fishSpawners = FindObjectsOfType<FishSpawner>();
            // foreach (FishSpawner spawner in fishSpawners)
            // {
            //     spawner.UpdateSpeed(currentScrollSpeed);
            // }

            if (timer >= gameDuration)
            {
                EndGame();
            }
        }
    }

    void StartGame()
    {
        gameStarted = true;
        currentScrollSpeed = minScrollSpeed;
        uiManager.HideCountdown();

        // Start background spawning
        if (backgroundManager != null)
        {
            backgroundManager.StartSpawning(currentScrollSpeed);
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
            // Find and destroy the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
            Debug.Log("GameController: Player destroyed.");
        }
        else
        {
            Debug.LogWarning("GameController: Player not found during EndGame.");
        }

        // Optionally, display end game UI
    }

    public void RestartGame()
    {
        // Destroy the persistent GameController before loading new scene
        Destroy(gameObject);
        Instance = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("GameController: Game restarted.");
    }
}
