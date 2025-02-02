using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawnManager2 : MonoBehaviour
{
    public static PlayerSpawnManager2 Instance { get; private set; }

    private Vector3 spawnPosition;
    private bool hasSpawnPosition = false;

    private HashSet<string> collectedMachines = new HashSet<string>(); // שמירת אובייקטים שנאספו
    private Dictionary<string, Vector3> movedPickups = new Dictionary<string, Vector3>(); // מיקום חפצים

    private void Awake()
    {
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

    // שמירת נקודת השחקן
    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
        hasSpawnPosition = true;
        Debug.Log("מיקום השחקן נשמר: " + spawnPosition);
    }

    public bool TryGetSpawnPosition(out Vector3 position)
    {
        position = spawnPosition;
        return hasSpawnPosition;
    }

    // שמירת אובייקטים שנאספו
    public void CollectMachine(string objectID)
    {
        collectedMachines.Add(objectID);
    }

    public bool IsMachineCollected(string objectID)
    {
        return collectedMachines.Contains(objectID);
    }

    // שמירת מיקום חפצים
    public void SavePickupPosition(string objectID, Vector3 position)
    {
        movedPickups[objectID] = position;
    }

    public bool TryGetPickupPosition(string objectID, out Vector3 position)
    {
        return movedPickups.TryGetValue(objectID, out position);
    }
}


