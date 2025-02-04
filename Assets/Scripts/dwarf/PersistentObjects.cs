using UnityEngine;

public class PersistentObjects : MonoBehaviour
{
    void Start()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Machine"))
        {
            if (PlayerSpawnManager2.Instance.IsMachineCollected(obj.name))
            {
                Destroy(obj);
            }
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Pickup"))
        {
            if (PlayerSpawnManager2.Instance.TryGetPickupPosition(obj.name, out Vector3 position))
            {
                obj.transform.position = position;
            }
        }
    }
}

