using System;
using UnityEngine;

public partial class ExperimentLogger
{
    // De-dup fish spawn logs within a single scene visit
    private readonly System.Collections.Generic.HashSet<string> _fishSpawnedIdsInScene = new System.Collections.Generic.HashSet<string>();

    public void LogFishSpawn(string fishId, Vector2 fishPos)
    {
        if (session == null) return;
        if (string.IsNullOrEmpty(fishId)) fishId = "unknown";
        // De-dup: only first spawn per fishId within the scene visit
        if (_fishSpawnedIdsInScene.Contains(fishId)) return;
        _fishSpawnedIdsInScene.Add(fishId);
        string area = GetCurrentArea();
        var section = EnsureSceneSection(area);
        var e = new FishSpawnEvent
        {
            scene = area,
            fishId = fishId,
            fishPos = fishPos,
            time = DateTime.UtcNow.ToString("o"),
            tScene = R2(Time.time - currentSceneStartTime)
        };
        section.fishSpawns.Add(e);
    }
}
