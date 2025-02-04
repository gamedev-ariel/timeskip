using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public Sprite GetSceneSnapshot(int level)
    {
        if (level < questions.Length)
            return questions[level].sceneImage;
        return null;
    }

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

    public void LoadQuestion(int level)
    {
        if (level >= questions.Length) return;

        List<Sprite> answers = new List<Sprite>(questions[level].answerOptions);
        correctAnswerIndex = answers.IndexOf(questions[level].correctAnswer);

        uiManagerMG.ShowSceneSnapshot(questions[level].sceneImage, questions[level].question);
        SetAnswers(answers);
    }

    private void SetAnswers(List<Sprite> answers)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < answers.Count)
            {
                answerButtons[i].image.sprite = answers[i];
                answerButtons[i].gameObject.SetActive(true);
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
