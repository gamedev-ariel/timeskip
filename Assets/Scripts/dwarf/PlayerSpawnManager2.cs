using UnityEngine;
using System.Collections.Generic;

public class PlayerSpawnManager2 : MonoBehaviour
{
    public static PlayerSpawnManager2 Instance { get; private set; }

    private Vector3 spawnPosition;
    private bool hasSpawnPosition = false;

    private HashSet<string> collectedMachines = new HashSet<string>(); // ����� ��������� ������
    private Dictionary<string, Vector3> movedPickups = new Dictionary<string, Vector3>(); // ����� �����

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

    // ����� ����� �����
    public void SetSpawnPosition(Vector3 position)
    {
        spawnPosition = position;
        hasSpawnPosition = true;
        Debug.Log("����� ����� ����: " + spawnPosition);
    }

    public bool TryGetSpawnPosition(out Vector3 position)
    {
        position = spawnPosition;
        return hasSpawnPosition;
    }

    // ����� ��������� ������
    public void CollectMachine(string objectID)
    {
        collectedMachines.Add(objectID);
    }

    public bool IsMachineCollected(string objectID)
    {
        return collectedMachines.Contains(objectID);
    }

    // ����� ����� �����
    public void SavePickupPosition(string objectID, Vector3 position)
    {
        movedPickups[objectID] = position;
    }

    public bool TryGetPickupPosition(string objectID, out Vector3 position)
    {
        return movedPickups.TryGetValue(objectID, out position);
    }
}


