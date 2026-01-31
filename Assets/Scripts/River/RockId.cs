using UnityEngine;

// Assign this to each rock/mushroom platform in the River scene to provide a stable ID
public class RockId : MonoBehaviour
{
    [Tooltip("Stable rock ID, e.g., R01, R02. If empty, GameObject.name will be used.")]
    public string rockId;
}
