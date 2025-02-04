using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonAutoAlign : MonoBehaviour
{
    public RectTransform buttonContainer; // הפאנל שמכיל את הכפתורים
    public List<Button> buttons = new List<Button>(); // רשימה של כל הכפתורים

    private void Start()
    {
        AlignButtons();
    }

    public void AlignButtons()
    {
        if (buttons.Count == 0)
        {
            Debug.LogWarning("⚠ No buttons found in ButtonAutoAlign!");
            return;
        }

        float panelWidth = buttonContainer.rect.width; // רוחב הפאנל
        float spacing = 20f; // רווח בין כפתורים
        float totalButtonWidth = (buttons.Count * buttons[0].GetComponent<RectTransform>().sizeDelta.x) + ((buttons.Count - 1) * spacing);
        float startX = -totalButtonWidth / 2f + buttons[0].GetComponent<RectTransform>().sizeDelta.x / 2f;

        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform btnRect = buttons[i].GetComponent<RectTransform>();
            btnRect.anchoredPosition = new Vector2(startX + (i * (btnRect.sizeDelta.x + spacing)), btnRect.anchoredPosition.y);

            AdjustImageSize(buttons[i]); // דואג שהספירט לא ייחתך
        }
    }

    private void AdjustImageSize(Button button)
    {
        Image btnImage = button.GetComponent<Image>();
        if (btnImage == null) return;

        RectTransform btnRect = button.GetComponent<RectTransform>();
        float btnWidth = btnRect.sizeDelta.x;
        float btnHeight = btnRect.sizeDelta.y;

        btnImage.preserveAspect = true; // מוודא שהתמונה לא נמתחת
        btnImage.rectTransform.sizeDelta = new Vector2(btnWidth * 0.9f, btnHeight * 0.9f); // מתאים את גודל התמונה בתוך הכפתור
    }
}
