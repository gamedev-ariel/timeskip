using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Add this line
public class UIManager : MonoBehaviour
{
    public Text countdownText;
    public Text gameTimerText;
    public Text messageText;
    public Button restartButton;
    public Button mainMenuButton;
    public bool isForest = false;
        // Add references for screws
    public List<GameObject> darkScrews = new List<GameObject>();
    public List<GameObject> normalScrews = new List<GameObject>();

    public int screwsCollected = 0;
    public int totalScrews = 5;

    void Start()
    {
        // Initially hide message and restart button
        messageText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        // Debug check for screw lists
        Debug.Log($"Dark screws count: {darkScrews.Count}, Normal screws count: {normalScrews.Count}");
        
        // Initialize screws UI regardless of isForest flag (remove the condition)
        if(isForest) InitializeScrewsUI();
    }

    void InitializeScrewsUI()
    {
        // Validate lists
        if (darkScrews.Count == 0 || normalScrews.Count == 0)
        {
            Debug.LogError("Screw lists not properly assigned in Unity Inspector!");
            return;
        }

        for(int i = 0; i < totalScrews; i++)
        {
            if(i < darkScrews.Count && i < normalScrews.Count)
            {
                darkScrews[i].SetActive(true);
                normalScrews[i].SetActive(false);
                }
        }
    }

    public void CollectScrew()
    {
        if(screwsCollected < totalScrews)
        {
            darkScrews[screwsCollected].gameObject.SetActive(false);
            normalScrews[screwsCollected].gameObject.SetActive(true);
            screwsCollected++;
        }
    }

    public void ShowStartCountdown(float time)
    {
        countdownText.gameObject.SetActive(true);
        countdownText.text = Mathf.Ceil(time).ToString();
    }

    public void UpdateCountdown(float timeLeft)
    {
        if (timeLeft > 0)
        {
            countdownText.text = Mathf.Ceil(timeLeft).ToString();
        }
    }

    public void HideCountdown()
    {
        countdownText.gameObject.SetActive(false);
    }

    public void UpdateGameTimer(float timeLeft)
    {
        gameTimerText.text = "Time: " + Mathf.Ceil(timeLeft).ToString() + "s";
    }

    public void ShowWellDone()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "Well Done!";
        restartButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true); // Add this line
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu); // Add this line
    }

    public void ShowTryAgain()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "Try Again!";
        restartButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true); // Add this line
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu); // Add this line
    }

    void RestartGame()
    {
        // Disable button and remove listener to prevent multiple clicks
        restartButton.interactable = false;
        restartButton.onClick.RemoveAllListeners();
        
        // Call restart on GameController
        FindObjectOfType<GameController>().RestartGame();
    }

    void GoToMainMenu()
    {
        // Load the main menu scene
        FindAnyObjectByType<GameController>().GoToMainMenu();
    }
}
