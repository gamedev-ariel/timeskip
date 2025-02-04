using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        StartCoroutine(PlayLevel());
    }

    
    public IEnumerator PlayLevel()
    {
        Debug.Log($"🟡 Current level index: {currentLevel}");

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
            currentLevel++;
            if (currentLevel >= questionManager.GetTotalQuestions())
            {
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
            uiManager.UpdateLives(lives);
            if (lives <= 0)
            {
                uiManager.ShowGameOver();
            }
        }
    }
}
