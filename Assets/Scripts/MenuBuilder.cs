using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // Namespace for EventSystem

public class MenuBuilder : MonoBehaviour
{
    // Define button labels and corresponding scene names
    private readonly string[] buttonLabels = { "River", "Forest", "Giant" };
    private readonly string[] sceneNames = { "River", "Forest", "Giant" };

    // Button dimensions and spacing
    private readonly Vector2 buttonSize = new Vector2(160, 60);
    private readonly float spacing = 20f;

    void Start()
    {
        // Ensure an EventSystem exists
        EnsureEventSystem();

        // Create Canvas
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;

        // Add Canvas Scaler for responsiveness
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(800, 600);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // Add Graphic Raycaster for UI interactions
        canvasGO.AddComponent<GraphicRaycaster>();

        // Optional: Add a background image for better visibility
        GameObject backgroundGO = new GameObject("Background");
        backgroundGO.transform.SetParent(canvasGO.transform, false);
        Image backgroundImage = backgroundGO.AddComponent<Image>();
        backgroundImage.color = new Color(0.1f, 0.1f, 0.1f); // Dark background

        // Stretch the background to fill the Canvas
        RectTransform bgRT = backgroundGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        // Create a Panel to hold buttons (optional for better organization)
        GameObject panelGO = new GameObject("Panel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        RectTransform panelRT = panelGO.AddComponent<RectTransform>();
        panelRT.sizeDelta = new Vector2(400, 300);
        panelRT.anchorMin = new Vector2(0.5f, 0.5f);
        panelRT.anchorMax = new Vector2(0.5f, 0.5f);
        panelRT.anchoredPosition = Vector2.zero;

        // Add Vertical Layout Group for automatic button arrangement
        VerticalLayoutGroup layoutGroup = panelGO.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.spacing = spacing;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10); // Adding padding around the layout
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = false;

        // Add Content Size Fitter to adjust panel size based on children
        ContentSizeFitter fitter = panelGO.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Add a semi-transparent background to the Panel for visibility
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.8f, 0.8f, 0.8f, 0.5f); // Light gray with some transparency

        // Create Buttons
        for (int i = 0; i < buttonLabels.Length; i++)
        {
            CreateButton(panelGO.transform, buttonLabels[i], sceneNames[i]);
        }
    }

    // Method to ensure an EventSystem exists in the scene
    void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }
    }

    // Method to create a button with specified label and scene name
    void CreateButton(Transform parent, string label, string sceneName)
    {
        // Create Button GameObject
        GameObject buttonGO = new GameObject(label + "Button");
        buttonGO.transform.SetParent(parent, false);

        // Add Button component
        Button button = buttonGO.AddComponent<Button>();

        // Add Image component for button visuals
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 1f); // Blue color for visibility
        button.targetGraphic = buttonImage;

        // Configure RectTransform for the button
        RectTransform buttonRT = buttonGO.GetComponent<RectTransform>();
        buttonRT.sizeDelta = buttonSize;

        // Add a LayoutElement to control size within the layout group
        LayoutElement layoutElement = buttonGO.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = buttonSize.y;
        layoutElement.preferredWidth = buttonSize.x;

        // Create Text for Button
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);

        // Add Text component
        Text text = textGO.AddComponent<Text>();
        text.text = label;

        // Assign LegacyRuntime.ttf font
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        // Configure RectTransform for the text with padding
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0, 0);
        textRT.anchorMax = new Vector2(1, 1);
        textRT.offsetMin = new Vector2(10, 10); // Left and bottom padding
        textRT.offsetMax = new Vector2(-10, -10); // Right and top padding

        // Add OnClick Listener with Debug Logging
        button.onClick.AddListener(() => OnButtonClick(sceneName));
    }

    // Method called when a button is clicked
    void OnButtonClick(string sceneName)
    {
        Debug.Log($"Button clicked: {sceneName}");
        LoadScene(sceneName);
    }

    // Method to load the specified scene
    void LoadScene(string sceneName)
    {
        // Optional: Add error checking to ensure the scene exists
        if (IsSceneInBuild(sceneName))
        {
            Debug.Log($"Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' is not added to the Build Settings.");
        }
    }

    // Method to check if a scene is included in the Build Settings
    bool IsSceneInBuild(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for(int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if(name.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
