using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text countdownText;
    public Text gameTimerText;
    public Text messageText;
    public Button restartButton;

    void Start()
    {
        // Initially hide message and restart button
        messageText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
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
        restartButton.onClick.AddListener(RestartGame);
    }

    public void ShowTryAgain()
    {
        messageText.gameObject.SetActive(true);
        messageText.text = "Try Again!";
        restartButton.gameObject.SetActive(true);
        restartButton.onClick.AddListener(RestartGame);
    }

    void RestartGame()
    {
        // Disable button and remove listener to prevent multiple clicks
        restartButton.interactable = false;
        restartButton.onClick.RemoveAllListeners();
        
        // Call restart on GameController
        FindObjectOfType<GameController>().RestartGame();
    }
}
