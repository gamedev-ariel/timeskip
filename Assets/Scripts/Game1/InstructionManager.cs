using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class InstructionManager : MonoBehaviour
{
    // --- Add this line ---
    private static bool instructionsWereShown = false;

    public bool isForest = false;
    public bool isRiver = false;
    private enum InstructionState
    {
        waitingForAnyKey,
        WaitingForJump,
        WaitingForArrow,
        WaitingForEnter,
        WaitingForRock,
        WaitingForTerrain,
        Completed
    }

    private InstructionState currentState = InstructionState.WaitingForJump;
    private Text instructionText;

    void Start()
    {
        // --- If instructions were shown before, skip them entirely ---
        if (instructionsWereShown)
        {
            currentState = InstructionState.Completed;
            // We still need a canvas and text so your triggers can change it,
            // but we'll just start off blank and do nothing else.
            EnsureEventSystem();
            SetupCanvasAndText();
            instructionText.text = "";
            return;
        }

        if (isForest || isRiver)
        {
            currentState = InstructionState.waitingForAnyKey;
        }

        EnsureEventSystem();
        SetupCanvasAndText();  // Moved UI setup into a separate method (see below).

        if (isForest)
        {
            instructionText.text = "Collect the screws to fix the time machine! Avoid the berries, they are poisonous! Press any key to start.";
        }
        else if (isRiver)
        {
            instructionText.text = "Welcome to the river! Be careful of the piranhas! Press any key to start.";
        }
        else
        {
            instructionText.text = "Use space key to jump.";
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case InstructionState.WaitingForJump:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentState = InstructionState.WaitingForArrow;
                    instructionText.text = "Use right arrow to go right or left arrow to go left.";
                }
                break;

            case InstructionState.waitingForAnyKey:
                if (Input.anyKeyDown)
                {
                    currentState = InstructionState.Completed;
                    instructionText.text = "";
                }
                break;

            case InstructionState.WaitingForArrow:
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentState = InstructionState.Completed;
                    instructionText.text = "";
                }
                break;

            case InstructionState.WaitingForEnter:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SceneManager.LoadScene("kitchen");
                }
                break;

            case InstructionState.WaitingForRock:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SceneManager.LoadScene("river");
                }
                break;

            case InstructionState.WaitingForTerrain:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SceneManager.LoadScene("forest");
                }
                break;

            case InstructionState.Completed:
                // --- Once we ever hit "Completed," we mark instructionsWereShown = true. ---
                if (!instructionsWereShown)
                {
                    instructionsWereShown = true;
                }
                break;
        }
    }

    // House triggers
    public void TriggerHouseInstruction()
    {
        if (currentState != InstructionState.WaitingForEnter)
        {
            currentState = InstructionState.WaitingForEnter;
            instructionText.color = Color.black;
            instructionText.text = "Click enter to enter the house.";
        }
    }
    public void ClearHouseInstruction()
    {
        if (currentState == InstructionState.WaitingForEnter)
        {
            instructionText.color = Color.white;
            instructionText.text = "";
            currentState = InstructionState.Completed;
        }
    }

    // Rock triggers
    public void TriggerRockInstruction()
    {
        if (currentState != InstructionState.WaitingForRock)
        {
            currentState = InstructionState.WaitingForRock;
            instructionText.text = "Click enter to go to the river.";
        }
    }
    public void ClearRockInstruction()
    {
        if (currentState == InstructionState.WaitingForRock)
        {
            instructionText.text = "";
            currentState = InstructionState.Completed;
        }
    }

    // Terrain triggers
    public void TriggerTerrainInstruction()
    {
        if (currentState != InstructionState.WaitingForTerrain)
        {
            currentState = InstructionState.WaitingForTerrain;
            instructionText.text = "Click enter to go to the forest.";
        }
    }
    public void ClearTerrainInstruction()
    {
        if (currentState == InstructionState.WaitingForTerrain)
        {
            instructionText.text = "";
            currentState = InstructionState.Completed;
        }
    }

    // Make sure EventSystem exists
    void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
    }

    // --- UI setup code extracted into a method for clarity ---
    void SetupCanvasAndText()
    {
        GameObject canvasGO = new GameObject("InstructionCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        if (Camera.main != null)
        {
            canvasGO.transform.SetParent(Camera.main.transform);
            canvasGO.transform.localPosition = new Vector3(0f, 0f, 2f);
            canvasGO.transform.localRotation = Quaternion.identity;
        }
        else
        {
            canvasGO.transform.position = new Vector3(0f, 0f, -9f);
        }
        canvasGO.transform.localScale = Vector3.one * 0.01f;

        RectTransform canvasRT = canvasGO.GetComponent<RectTransform>();
        canvasRT.sizeDelta = new Vector2(800, 600);

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800, 600);

        canvasGO.AddComponent<GraphicRaycaster>();

        // Create a Text UI element
        GameObject textGO = new GameObject("InstructionText");
        textGO.transform.SetParent(canvasGO.transform, false);
        instructionText = textGO.AddComponent<Text>();

        instructionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        instructionText.fontSize = 56;
        instructionText.alignment = TextAnchor.MiddleCenter;
        instructionText.color = Color.magenta;
        instructionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        instructionText.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0.5f, 0.5f);
        textRT.anchorMax = new Vector2(0.5f, 0.5f);
        textRT.pivot = new Vector2(0.5f, 0.5f);
        textRT.anchoredPosition = Vector2.zero;
        textRT.sizeDelta = new Vector2(600, 150);
    }
}
