using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    public CanvasGroup fadeCanvas; // אם יש לך UI של fade
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // שהסקריפט ישאר פעיל גם בין סצנות
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (fadeCanvas != null) // Fade out
        {
            yield return StartCoroutine(Fade(1));
        }

        SceneManager.LoadScene(sceneName);

        if (fadeCanvas != null) // Fade in
        {
            yield return StartCoroutine(Fade(0));
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvas.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }
        fadeCanvas.alpha = targetAlpha;
    }
}
