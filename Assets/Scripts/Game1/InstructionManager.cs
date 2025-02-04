using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class InstructionManager : MonoBehaviour
{
    // Define the instruction states.

    public bool isForest = false;
    public bool isRiver = false;
    private enum InstructionState
    {
        waitingForAnyKey,
        WaitingForJump,
        WaitingForArrow,
        WaitingForEnter,  // For house
        WaitingForRock,   // For rock
        WaitingForTerrain,   // For rock
        Completed
    }

    private InstructionState currentState = InstructionState.WaitingForJump;
    private Text instructionText;

    void Start()
    {
        if (isForest || isRiver)
        {
            currentState = InstructionState.waitingForAnyKey;
        }
        // Ensure an EventSystem exists in the scene.
        EnsureEventSystem();

        // Create a Canvas for UI in World Space mode.
        GameObject canvasGO = new GameObject("InstructionCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main; // assign main camera

        // Parent the canvas to the camera so that it moves with it.
        if (Camera.main != null)
        {
            canvasGO.transform.SetParent(Camera.main.transform);
            // Set local position relative to the camera so it's visible in front of it.
            canvasGO.transform.localPosition = new Vector3(0f, 0f, 2f);
            // Reset rotation so it faces the same direction as the camera.
            canvasGO.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("Main camera not found. Canvas will not move with the camera.");
            canvasGO.transform.position = new Vector3(0f, 0f, -9f);
        }

        // Scale down the canvas by 100 times.
        canvasGO.transform.localScale = Vector3.one * 0.01f;

        // Configure the RectTransform size (adjust as needed).
        RectTransform canvasRT = canvasGO.GetComponent<RectTransform>();
        canvasRT.sizeDelta = new Vector2(800, 600);

        // Add a Canvas Scaler (optional for scaling purposes).
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800, 600);

        // Add a Graphic Raycaster (if you plan to use UI interactions).
        canvasGO.AddComponent<GraphicRaycaster>();

        // Create a Text UI element to show instructions.
        GameObject textGO = new GameObject("InstructionText");
        textGO.transform.SetParent(canvasGO.transform, false);
        instructionText = textGO.AddComponent<Text>();

        // Use LegacyRuntime.ttf instead of Arial.ttf.
        instructionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        instructionText.fontSize = 28;
        instructionText.alignment = TextAnchor.MiddleCenter;
        instructionText.color = Color.black;
        if (isForest)
        {
            instructionText.text = "Collect the screws to fix the time machine! Avoid the berries, they are poisonous! Press any key to start.";
        }
        else if (isRiver)
        {
            instructionText.text = "Welcome to the river! Be carefull of the piranhas! Press any key to start.";
        }
        else
        {
            instructionText.text = "Use space key to jump.";
        }
        // Configure the RectTransform so that the text is centered in the canvas.
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0.5f, 0.5f);
        textRT.anchorMax = new Vector2(0.5f, 0.5f);
        textRT.pivot = new Vector2(0.5f, 0.5f);
        textRT.anchoredPosition = Vector2.zero;
        textRT.sizeDelta = new Vector2(600, 50);
    }

    void Update()
    {
        // Process input based on our current instruction state.
        switch (currentState)
        {
            case InstructionState.WaitingForJump:
                // When the space key is pressed, assume the player has jumped.
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
                // Once the player presses either the right or left arrow key, clear the message.
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    currentState = InstructionState.Completed;
                    instructionText.text = "";
                }
                break;

            case InstructionState.WaitingForEnter:
                // When the player presses Enter, load the "kitchen" scene.
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SceneManager.LoadScene("kitchen");
                }
                break;

            case InstructionState.WaitingForRock:
                // When the player presses Enter, load the "river" scene.
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SceneManager.LoadScene("river");
                }
                break;

            case InstructionState.WaitingForTerrain:
                // When the player presses Enter, load the "river" scene.
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SceneManager.LoadScene("forest");
                }
                break;

            case InstructionState.Completed:
                // Do nothing until further instruction.
                break;
        }
    }

    // This method is called by the house object when the player collides with it.
    public void TriggerHouseInstruction()
    {
        // Only change the state if we are not already waiting for Enter.
        if (currentState != InstructionState.WaitingForEnter)
        {
            currentState = InstructionState.WaitingForEnter;
            instructionText.text = "Click enter to enter the house.";
        }
    }

    // Clears the house instruction when the player is no longer colliding.
    public void ClearHouseInstruction()
    {
        if (currentState == InstructionState.WaitingForEnter)
        {
            instructionText.text = "";
            currentState = InstructionState.Completed;
        }
    }

    // --- New Methods for Rock Instruction ---

    // Called when the player collides with the rock.
    public void TriggerRockInstruction()
    {
        if (currentState != InstructionState.WaitingForRock)
        {
            currentState = InstructionState.WaitingForRock;
            instructionText.text = "Click enter to go to the river.";
        }
    }

    // Clears the rock instruction when the player is no longer colliding.
    public void ClearRockInstruction()
    {
        if (currentState == InstructionState.WaitingForRock)
        {
            instructionText.text = "";
            currentState = InstructionState.Completed;
        }
    }

    // Called when the player collides with the rock.
    public void TriggerTerrainInstruction()
    {
        if (currentState != InstructionState.WaitingForTerrain)
        {
            currentState = InstructionState.WaitingForTerrain;
            instructionText.text = "Click enter to go to the forest.";
        }
    }

    // Clears the rock instruction when the player is no longer colliding.
    public void ClearTerrainInstruction()
    {
        if (currentState == InstructionState.WaitingForTerrain)
        {
            instructionText.text = "";
            currentState = InstructionState.Completed;
        }
    }

    // Ensures there is an EventSystem in the scene for UI elements to work.
    void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
    }
}
