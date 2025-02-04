using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main");
    }

    public void SkipToNextLevel()
    {
        SceneManager.LoadScene("Dwarf");
    }    
    
    public void GoToStart()
    {
        SceneManager.LoadScene("Start");
    }
}
