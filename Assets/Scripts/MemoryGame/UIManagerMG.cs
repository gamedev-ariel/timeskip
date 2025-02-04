using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // ייבוא TextMeshPro

public class UIManagerMG : MonoBehaviour
{
    public static UIManagerMG Instance;

    public GameObject sceneSnapshotPanel;
    public Image sceneSnapshotImage;
    public GameObject questionPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI questionText; // טקסט השאלה
    public TextMeshProUGUI livesText; // תצוגת חיים

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        questionPanel.SetActive(false);
        sceneSnapshotPanel.SetActive(true);
    }

    public void ShowSceneSnapshot(Sprite snapshot, string question)
    {
        if (sceneSnapshotImage != null && snapshot != null)
        {
            sceneSnapshotPanel.SetActive(true);
            sceneSnapshotImage.sprite = snapshot;
            Debug.Log("Loaded snapshot: " + snapshot.name);
            StartCoroutine(HideSceneSnapshotAfterDelay(question));
        }
        else
        {
            Debug.LogWarning("SceneSnapshotImage or snapshot is null!");
        }
    }

    private IEnumerator HideSceneSnapshotAfterDelay(string question)
    {
        yield return new WaitForSeconds(2);
        sceneSnapshotPanel.SetActive(false);
        questionPanel.SetActive(true);

        if (questionText != null)
        {
            questionText.text = question;
        }
        else
        {
            Debug.LogWarning("questionText is null!");
        }
    }

    public void HideSceneSnapshot()
    {
        sceneSnapshotPanel.SetActive(false);
    }

    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
