using UnityEngine;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    [Header("Background Prefabs")]
    public GameObject backgroundStartPrefab;
    public GameObject backgroundMiddlePrefab;
    public GameObject backgroundEndPrefab;

    [Header("Spawning Settings")]
    public int numberOfMiddleBackgrounds = 7;
    private int middleBackgroundsSpawned = 0;

    public float backgroundWidth = 30f; // Width of each background (adjust based on your sprite)
    public float positionY = 0f; // Y position to spawn new backgrounds
    public float positionZ = 0f; // Z position to spawn new backgrounds

    private List<GameObject> activeBackgrounds = new List<GameObject>();

    private Camera mainCamera;
    private float cameraRightEdge;

void Start()
{
    mainCamera = Camera.main;
    CalculateCameraBounds();
    // Spawn only two backgrounds initially
    for (int i = 0; i < 2; i++)
    {
        float xPos = i * backgroundWidth; 
        SpawnBackground((i == 0) ? backgroundStartPrefab : backgroundMiddlePrefab,
            new Vector3(xPos, positionY, positionZ));
    }
}


    void Update()
    {
        // Update camera bounds in case of camera movement or changes
        CalculateCameraBounds();

        // Check if the last background is approaching the camera's right edge
        if (activeBackgrounds.Count > 0)
        {
            GameObject lastBackground = activeBackgrounds[activeBackgrounds.Count - 1];
            float lastBackgroundRightEdge = lastBackground.transform.position.x + (backgroundWidth / 2);

            if (lastBackgroundRightEdge < cameraRightEdge + (backgroundWidth / 2))
            {
                SpawnNextBackground();
            }
        }

        // Destroy backgrounds that have moved out of the camera's left edge
        for (int i = activeBackgrounds.Count - 1; i >= 0; i--)
        {
            GameObject bg = activeBackgrounds[i];
            float bgLeftEdge = bg.transform.position.x - (backgroundWidth / 2);
            float cameraLeftEdge = mainCamera.transform.position.x - (mainCamera.orthographicSize * mainCamera.aspect);

            if (bgLeftEdge < cameraLeftEdge - (backgroundWidth / 2))
            {
                Destroy(bg);
                activeBackgrounds.RemoveAt(i);
                Debug.Log($"BackgroundManager: Destroyed {bg.name}.");
            }
        }
    }

    void CalculateCameraBounds()
    {
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;

        cameraRightEdge = mainCamera.transform.position.x + (screenWidth / 2);
    }

void SpawnBackground(GameObject prefab, Vector3 position)
{
    GameObject newBg = Instantiate(prefab, position, Quaternion.identity, this.transform);
    BackgroundScroller scroller = newBg.GetComponent<BackgroundScroller>();
    if (scroller != null)
    {
        scroller.SetBackgroundManager(this);
        scroller.StartScrolling();
    }
    activeBackgrounds.Add(newBg);
}

void SpawnNextBackground()
{
    GameObject prefabToSpawn;
    if (middleBackgroundsSpawned < numberOfMiddleBackgrounds)
    {
        prefabToSpawn = backgroundMiddlePrefab;
        middleBackgroundsSpawned++;
    }
    else
    {
        prefabToSpawn = backgroundEndPrefab;
    }

    // Spawn the new background just to the right of the last one
    GameObject lastBackground = activeBackgrounds[activeBackgrounds.Count - 1];
    Vector3 nextPos = new Vector3(
        lastBackground.transform.position.x + backgroundWidth,
        positionY,
        positionZ
    );
    SpawnBackground(prefabToSpawn, nextPos);
}

    // This method is called by BackgroundScroller when a background reaches the end
    public void OnBackgroundReachedEnd()
    {
        Debug.Log("BackgroundManager: OnBackgroundReachedEnd called.");
        SpawnNextBackground();
    }

    public void StartSpawning(float speed)
    {
        // Implement starting logic here if necessary
    }

    // Optional: Implement if you have ongoing spawning mechanisms
    public void StopSpawning()
    {
        // Implement stopping logic here if necessary
    }
}
