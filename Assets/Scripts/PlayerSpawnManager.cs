using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance { get; private set; }

    private Vector3 spawnPosition;
    private bool hasSpawnPosition = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
        hasSpawnPosition = true;
    }

    public bool TryGetSpawnPosition(out Vector3 position)
    {
        position = spawnPosition;
        bool hadPosition = hasSpawnPosition;
        hasSpawnPosition = false;
        return hadPosition;
    }
}