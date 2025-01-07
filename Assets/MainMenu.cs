using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // טען את הסצנה הראשית של המשחק
        SceneManager.LoadScene("GameScene");
    }

    public void ShowInstructions()
    {
        // טען את סצנת ההוראות
        SceneManager.LoadScene("InstructionsScene");
    }

    public void ShowVideo()
    {
        // טען את סצנת הווידאו או הפעל וידאו מובנה
        SceneManager.LoadScene("VideoScene");
    }

    public void ShowInfo()
    {
        // טען את סצנת המידע
        SceneManager.LoadScene("InfoScene");
    }

    public void ShowMap()
    {
        // טען את סצנת מפת המשחק
        SceneManager.LoadScene("515253");
    }

    public void QuitGame()
    {
        // סגור את המשחק
        Application.Quit();
    }
}
