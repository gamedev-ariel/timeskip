using System;
using UnityEngine;

// Attach to forest collectible/hazard items (screw, blueberry) to emit spawn/resolve logs
public class ForestItemTrack : MonoBehaviour
{
    [Tooltip("Unique ID of this item instance. Auto-generated if empty.")]
    public string itemId;
    [Tooltip("Item type: 'screw' or 'blueberry'. Will try to infer from tag/name if empty.")]
    public string itemType;

    private bool resolved;
    private bool _spawnEmitted;

    private void OnEnable()
    {
        // Log spawn at first activation as well (covers late activations or when component was added while inactive)
        TryEmitSpawn();
    }

    private void Start()
    {
        // Safety: in case OnEnable ran before logger existed or identifiers not initialized
        TryEmitSpawn();
    }

    private string InferType()
    {
        try
        {
            if (CompareTag("Screw")) return "screw";
            if (CompareTag("Blueberry")) return "blueberry";
        }
        catch { }
        string n = gameObject.name.ToLowerInvariant();
        if (n.Contains("screw")) return "screw";
        if (n.Contains("blueberry")) return "blueberry";
        return "unknown";
    }

    private void EnsureInit()
    {
        if (string.IsNullOrEmpty(itemId)) itemId = Guid.NewGuid().ToString();
        if (string.IsNullOrEmpty(itemType)) itemType = InferType();
    }

    private void TryEmitSpawn()
    {
        if (_spawnEmitted) return;
        EnsureInit();
        try
        {
            ExperimentLogger.Instance?.LogForestItemSpawn(itemId, itemType, transform.position, "forest");
            _spawnEmitted = true;
        }
        catch (Exception) { }
    }

    public void Resolve(string resolution, string collector = "player")
    {
        if (resolved) return;
        resolved = true;
        try
        {
            ExperimentLogger.Instance?.LogForestItemResolve(itemId, itemType, resolution, collector, "forest");
        }
        catch (Exception) { }
    }

    private void OnDestroy()
    {
        // If destroyed without explicit resolve, assume expired
        if (!resolved)
        {
            try
            {
                // Use "expired" as default to differentiate from collected/missed explicitly
                ExperimentLogger.Instance?.LogForestItemResolve(itemId, itemType, "expired", "");
            }
            catch (Exception) { }
        }
    }
}
