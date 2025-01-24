using UnityEngine;
using System.Collections.Generic;

public class BackgroundSpawner : MonoBehaviour
{
    public GameObject backPrefab;
    public float spawnInterval = 10f;
    public float spawnX = 15f;
    public float spawnY = 15f;
    private float timer = 0f;
    private bool canSpawn = false;

    // To keep track of fish positions to avoid overlap
    public List<Rect> fishAreas;

    void Start()
    {
        fishAreas = new List<Rect>();
    }

    void Update()
    {
        if (canSpawn)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnRock();
                timer = 0f;
            }
        }
    }

    void SpawnRock()
    {
        // Randomly select a rock prefab
        GameObject rock = Instantiate(backPrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity);
        // Optionally, set rock's parent to background for organization
        rock.transform.parent = this.transform;
    }

    public void StartSpawning()
    {
        canSpawn = true;
    }

    public void StopSpawning()
    {
        canSpawn = false;
    }
}
