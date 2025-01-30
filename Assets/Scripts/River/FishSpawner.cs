using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab;
    public float spawnInterval = 3f;

    // X range
    public float spawnXMin = 14f;
    public float spawnXMax = 16f;

    // Y range
    public float spawnYMin = -6f;
    public float spawnYMax = -5.5f; // Slight variation below camera

    private float timer = 0f;
    private bool canSpawn = false;

    void Update()
    {
        if (canSpawn)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnFish();
                timer = 0f;
            }
        }
    }

    void SpawnFish()
    {
        float spawnX = Random.Range(spawnXMin, spawnXMax);
        float spawnY = Random.Range(spawnYMin, spawnYMax);

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);
        GameObject fish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
        fish.transform.parent = this.transform;
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
