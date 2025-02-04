using UnityEngine;

public class ObjectPositionSaver : MonoBehaviour
{
    private string objectID;

    private void Start()
    {
        objectID = gameObject.name; // זיהוי ייחודי לאובייקט
        LoadPosition();
    }

    private void OnDisable()
    {
        SavePosition();
    }

    private void SavePosition()
    {
        Vector3 pos = transform.position;
        PlayerSpawnManager2.Instance.SavePickupPosition(objectID, pos);
    }

    private void LoadPosition()
    {
        if (PlayerSpawnManager2.Instance.TryGetPickupPosition(objectID, out Vector3 pos))
        {
            transform.position = pos;
        }
    }
}
