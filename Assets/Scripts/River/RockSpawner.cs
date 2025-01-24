using UnityEngine;
using System.Collections.Generic;

public class RockSpawner : MonoBehaviour
{
    [Header("Rock Settings")]
    public List<GameObject> rockPrefabs; // List of rock prefabs to spawn
    public int numberOfRocks = 5; // Total rocks to spawn per background
    public float minDistance = 1f; // Minimum distance between rocks

    [Header("Spawn Area")]
    public Vector2 spawnAreaSize = new Vector2(10f, 2.5f); // Width and height of spawn area

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnRocks();
    }
public void SpawnRocks()
{
    for (int i = 0; i < numberOfRocks; i++)
    {
        Vector3 spawnPosition;
        bool validPosition = false;
        int attempts = 0;
        int maxAttempts = 10;

        do
        {
            float randomX = Random.Range(-spawnAreaSize.x*2 / 3, spawnAreaSize.x*2 / 3);
            // Shift Y range to lower half
            float randomY = Random.Range(-spawnAreaSize.y, 0f);

            spawnPosition = new Vector3(randomX, randomY, 0f) + transform.position;

            validPosition = true;
            foreach (Vector3 pos in spawnedPositions)
            {
                if (Vector3.Distance(spawnPosition, pos) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }

            attempts++;
            if (attempts >= maxAttempts) break;

        } while (!validPosition);

        spawnedPositions.Add(spawnPosition);

        int index = Random.Range(0, rockPrefabs.Count);
        Instantiate(rockPrefabs[index], spawnPosition, Quaternion.identity, this.transform);
    }
}}