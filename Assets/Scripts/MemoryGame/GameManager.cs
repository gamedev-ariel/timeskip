using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int lives = 3; // מספר חיים
    public int currentLevel = 0; // שלב נוכחי

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
        
        Sprite snapshot = SceneSnapshot.Instance.GetSnapshotForLevel(currentLevel); // השג את התמונה לשלב הזה
        string question = questionManager.GetQuestion(currentLevel); // השג את השאלה לשלב

        if (snapshot == null)
        {
            Debug.LogError($"❌ Snapshot sprite is NULL for level {currentLevel}! Check SceneSnapshot script.");
            yield break; // עצור את הפונקציה, אין טעם להמשיך בלי תמונה
        }

        if (string.IsNullOrEmpty(question))
        {
            Debug.LogError($"❌ Question is NULL or EMPTY for level {currentLevel}! Check QuestionManager.");
            yield break;
        }

        uiManager.ShowSceneSnapshot(snapshot, question); // הצג תמונת מצב
        yield return new WaitForSeconds(5f); // המתן לזמן הזיכרון
        uiManager.HideSceneSnapshot(); // הסתר את התמונה והצג את השאלה

        questionManager.LoadQuestion(currentLevel); // טען את השאלה של השלב
    }


    //public IEnumerator PlayLevel()
    //{
    //    //Sprite sceneSnapshot = questionManager.GetSceneSnapshot(currentLevel);
    //    //string question = questionManager.GetQuestion(currentLevel);


    //    //uiManager.ShowSceneSnapshot(sceneSnapshot, question); // הצגת תמונת המצב


    //    Sprite snapshot = questionManager.GetSceneSnapshot(currentLevel); // השג את התמונה לשלב הזה
    //    string question = questionManager.GetQuestion(currentLevel); // השג את השאלה לשלב

    //    if (snapshot == null)
    //    {
    //        Debug.LogError("❌ Snapshot sprite is NULL! Check GameManager.");
    //    }
    //    if (string.IsNullOrEmpty(question))
    //    {
    //        Debug.LogError("❌ Question is NULL or EMPTY! Check QuestionManager.");
    //    }

    //    uiManager.ShowSceneSnapshot(snapshot, question);



    //    yield return new WaitForSeconds(5f); // זמן לזכירת התמונה
    //    uiManager.HideSceneSnapshot();

    //    questionManager.LoadQuestion(currentLevel); // טעינת השאלה הנכונה לשלב
    //}

    public void CheckAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            currentLevel++;
            if (currentLevel >= questionManager.GetTotalQuestions()) // בדיקת כמות שלבים
            {
                SceneManager.LoadScene("NextScene"); // מעבר לסצנה הבאה
            }
            else
            {
                StartCoroutine(PlayLevel()); // מעבר לשלב הבא
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

    public void RestartGame()
    {
        lives = 3;
        currentLevel = 0;
        StartCoroutine(PlayLevel()); // התחלה מהשלב הראשון
    }

    //public Sprite GetSceneSnapshot(int level)
    //{
    //    if (level < SceneSnapshot.Instance.sceneSprites.Length) // וודא שהאינדקס תקין
    //    {
    //        return SceneSnapshot.Instance.sceneSprites[level];
    //    }

    //    Debug.LogError($"❌ No snapshot found for level {level}! Check sceneSprites array.");
    //    return null;
    //}


    public Sprite GetSceneSnapshot(int level)
    {
        if (SceneSnapshot.Instance == null)
        {
            Debug.LogError("❌ SceneSnapshot.Instance is NULL! Make sure SceneSnapshot exists in the scene.");
            return null;
        }

        if (SceneSnapshot.Instance.sceneSprites == null || SceneSnapshot.Instance.sceneSprites.Length == 0)
        {
            Debug.LogError("❌ sceneSprites array is NULL or EMPTY! Check SceneSnapshot component.");
            return null;
        }

        if (level < SceneSnapshot.Instance.sceneSprites.Length)
        {
            return SceneSnapshot.Instance.sceneSprites[level];
        }

        Debug.LogError($"❌ No snapshot found for level {level}! Check sceneSprites array.");
        return null;
    }


}
