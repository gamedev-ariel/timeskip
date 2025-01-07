using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // ��� �� ����� ������ �� �����
        SceneManager.LoadScene("icy");
    }

    public void ShowInstructions()
    {
        // ��� �� ���� �������
        SceneManager.LoadScene("InstructionsScene");
    }

    public void ShowVideo()
    {
        // ��� �� ���� ������� �� ���� ����� �����
        SceneManager.LoadScene("VideoScene");
    }

    public void ShowInfo()
    {
        // ��� �� ���� �����
        SceneManager.LoadScene("InfoScene");
    }

    public void ShowMap()
    {
        // ��� �� ���� ��� �����
        SceneManager.LoadScene("515253");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("main");
    }


    public void QuitGame()
    {
        // ���� �� �����
        Application.Quit();
    }
}
