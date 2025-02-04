using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class QuestionData
{
    public string question;
    public Sprite correctAnswer;
    public Sprite[] answerOptions;
    public Sprite sceneImage;
}

public class QuestionManager : MonoBehaviour
{
    public UIManagerMG uiManagerMG;
    public Button[] answerButtons;
    public QuestionData[] questions;
    private int correctAnswerIndex;

    public string GetQuestion(int level)
    {
        if (level < questions.Length)
            return questions[level].question;
        return "No question available";
    }

    public int GetTotalQuestions()
    {
        return questions.Length;
    }

    public void PreloadQuestion(int level)
    {
        if (level >= questions.Length) return;

        List<Sprite> answers = new List<Sprite>(questions[level].answerOptions);
        correctAnswerIndex = answers.IndexOf(questions[level].correctAnswer);

        SetAnswers(answers);
    }

    private void SetAnswers(List<Sprite> answers)
    {
        int buttonCount = answerButtons.Length;
        float buttonSpacing = 150f; // Adjust spacing dynamically
        float totalWidth = buttonSpacing * (answers.Count - 1);

        for (int i = 0; i < buttonCount; i++)
        {
            if (i < answers.Count)
            {
                answerButtons[i].image.sprite = answers[i];
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].transform.localPosition = new Vector3(-totalWidth / 2 + i * buttonSpacing, 0, 0);
                int index = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => AnswerSelected(index));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void AnswerSelected(int index)
    {
        GameManager.Instance.CheckAnswer(index == correctAnswerIndex);
    }
}
