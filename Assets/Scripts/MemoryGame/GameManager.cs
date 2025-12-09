using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int lives = 3; // Number of lives
    public int currentLevel = 0; // Current level index

    public UIManagerMG uiManager;
    public QuestionManager questionManager;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        try { ExperimentLogger.Instance?.LogOutcome("MemoryGame", "minigame_start", reason: "start", livesLeft: lives); } catch (Exception) { }
        StartCoroutine(PlayLevel());
    }

    
    public IEnumerator PlayLevel()
    {
        Debug.Log($"🟡 Current level index: {currentLevel}");
        try { ExperimentLogger.Instance?.LogOutcome("MemoryGame", "level_start", reason: $"level={currentLevel}", livesLeft: lives); } catch (Exception) { }

        // Make sure the question panel is hidden before showing the scene image
        uiManager.HideQuestionPanel();

        Sprite snapshot = SceneSnapshot.Instance.GetSnapshotForLevel(currentLevel);
        string question = questionManager.GetQuestion(currentLevel);

        if (snapshot == null || string.IsNullOrEmpty(question))
        {
            Debug.LogError($"❌ Missing snapshot or question for level {currentLevel}!");
            yield break;
        }

        uiManager.ShowSceneSnapshot(snapshot); // Show scene image
        questionManager.PreloadQuestion(currentLevel); // Preload question while image is visible
        yield return new WaitForSeconds(5f); // Wait for 5 seconds

        uiManager.HideSceneSnapshot(); // Hide image
        uiManager.ShowQuestionPanel(); // Show question panel after image disappears
    }


    public void CheckAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            try { ExperimentLogger.Instance?.LogOutcome("MemoryGame", "correct", reason: $"level={currentLevel}", livesLeft: lives); } catch (Exception) { }
            currentLevel++;

            if (currentLevel == 4)
            {
                try { ExperimentLogger.Instance?.LogOutcome("MemoryGame", "minigame_complete", reason: "reached_level_4", livesLeft: lives); } catch (Exception) { }
                SceneManager.LoadScene("kitchen");
                return;
            }

            if (currentLevel >= questionManager.GetTotalQuestions())
            {
                try { ExperimentLogger.Instance?.LogOutcome("MemoryGame", "minigame_complete", reason: "all_levels_cleared", livesLeft: lives); } catch (Exception) { }
                SceneManager.LoadScene("NextScene");
            }
            else
            {
                StartCoroutine(PlayLevel());
            }
        }
        else
        {
            lives--;
            try { ExperimentLogger.Instance?.LogOutcome("MemoryGame", "incorrect", reason: $"level={currentLevel}", livesLeft: lives); } catch (Exception) { }
            uiManager.UpdateLives(lives);
            if (lives <= 0)
            {
                try { ExperimentLogger.Instance?.LogOutcome("MemoryGame", "game_over", reason: "no_lives", livesLeft: 0); } catch (Exception) { }
                uiManager.ShowGameOver();
            }
        }
    }
}
