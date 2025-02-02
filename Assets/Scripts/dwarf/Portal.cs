using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad; // שם הסצנה לטעינה

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // בודק אם השחקן נכנס לפורטל
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}