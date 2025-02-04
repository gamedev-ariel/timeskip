using UnityEngine;
using UnityEngine.UI;

public class SceneSnapshot : MonoBehaviour
{
    public static SceneSnapshot Instance;
    public Image sceneImage;
    public Sprite[] sceneSprites;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("✅ SceneSnapshot Instance set.");
        }
        else
        {
            Debug.LogWarning("⚠ SceneSnapshot Instance already exists! Destroying duplicate.");
            Destroy(gameObject);
        }

        if (sceneSprites == null || sceneSprites.Length == 0)
        {
            Debug.LogError("❌ sceneSprites array is NULL or EMPTY! Make sure SceneSnapshot has sprites assigned.");
        }
        else
        {
            Debug.Log($"🔍 sceneSprites contains {sceneSprites.Length} sprites.");
            for (int i = 0; i < sceneSprites.Length; i++)
            {
                Debug.Log($"🔹 Sprite {i}: {sceneSprites[i]?.name ?? "NULL"}");
            }
        }
    }

    public void ShowSceneSnapshot(int level)
    {
        if (level < sceneSprites.Length && sceneImage != null)
        {
            sceneImage.sprite = sceneSprites[level];
            UIManagerMG.Instance.sceneSnapshotPanel.SetActive(true);
        }
    }

    public Sprite GetSnapshotForLevel(int level)
    {
        if (level < 0 || level >= sceneSprites.Length)
        {
            Debug.LogError($"❌ Invalid level index {level}! It should be between 0 and {sceneSprites.Length - 1}.");
            return null;
        }
        return sceneSprites[level];
    }
}
