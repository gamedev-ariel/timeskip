using UnityEngine;

public class PlayerSpawner2 : MonoBehaviour
{
    private void Start()
    {
        // Check if there's a stored spawn position
        if (PlayerSpawnManager2.Instance.TryGetSpawnPosition(out Vector3 spawnPosition))
        {
            transform.position = spawnPosition;
        }
    }
}
