﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManagerMG : MonoBehaviour
{
    public static UIManagerMG Instance;

    public GameObject sceneSnapshotPanel;
    public Image sceneSnapshotImage;
    public GameObject questionPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI livesText;

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

    public void ShowSceneSnapshot(Sprite snapshot)
    {
        sceneSnapshotPanel.SetActive(true);
        sceneSnapshotImage.sprite = snapshot;
    }

    public void HideSceneSnapshot()
    {
        sceneSnapshotPanel.SetActive(false);
    }

    public void ShowQuestionPanel()
    {
        questionPanel.SetActive(true);
    }

    public void HideQuestionPanel()
    {
        questionPanel.SetActive(false);
    }

    public void UpdateLives(int lives)
    {
        livesText.text = "Lives: " + lives;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(false);
        questionPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}
