using System;
using UnityEngine;

public partial class ExperimentLogger
{
    public void LogForestItemSpawn(string itemId, string itemType, Vector2 spawnPos, string minigame = "forest")
    {
        if (session == null) return;
        string area = GetCurrentArea();
        var section = EnsureSceneSection(area);
        var mg = EnsureMinigameSection(section, GetEffectiveMinigameName(minigame));
        float tMini = (!string.IsNullOrEmpty(currentMinigameName) && string.Equals(currentMinigameName, mg.name, StringComparison.Ordinal))
            ? (Time.time - currentMinigameStartTime) : 0f;
        var e = new ForestItemSpawnEvent
        {
            scene = area,
            sceneVisitId = currentSceneVisitId,
            minigameRunId = currentMinigameRunId,
            itemId = string.IsNullOrEmpty(itemId) ? Guid.NewGuid().ToString() : itemId,
            itemType = string.IsNullOrEmpty(itemType) ? "unknown" : itemType,
            spawnPos = spawnPos,
            time = DateTime.UtcNow.ToString("o"),
            tScene = R2(Time.time - currentSceneStartTime),
            tMini = R2(tMini)
        };
        // Attach player position except in dwarf/memory grouped scenes
        try
        {
            if (ShouldAttachPlayerPos() && TryGetPlayerPos(out var p))
            {
                e.playerPos = p;
            }
        }
        catch { }
        mg.forestItemSpawns.Add(e);
    }

    public void LogForestItemResolve(string itemId, string itemType, string resolution, string collector = "player", string minigame = "forest")
    {
        if (session == null) return;
        string area = GetCurrentArea();
        var section = EnsureSceneSection(area);
        var mg = EnsureMinigameSection(section, GetEffectiveMinigameName(minigame));
        float tMini = (!string.IsNullOrEmpty(currentMinigameName) && string.Equals(currentMinigameName, mg.name, StringComparison.Ordinal))
            ? (Time.time - currentMinigameStartTime) : 0f;
        var e = new ForestItemResolveEvent
        {
            scene = area,
            sceneVisitId = currentSceneVisitId,
            minigameRunId = currentMinigameRunId,
            itemId = string.IsNullOrEmpty(itemId) ? null : itemId,
            itemType = string.IsNullOrEmpty(itemType) ? "unknown" : itemType,
            resolution = string.IsNullOrEmpty(resolution) ? "unknown" : resolution,
            collector = string.IsNullOrEmpty(collector) ? "player" : collector,
            time = DateTime.UtcNow.ToString("o"),
            tScene = R2(Time.time - currentSceneStartTime),
            tMini = R2(tMini)
        };
        // Attach player position except in dwarf/memory grouped scenes
        try
        {
            if (ShouldAttachPlayerPos() && TryGetPlayerPos(out var p))
            {
                e.playerPos = p;
            }
        }
        catch { }
        mg.forestItemResolves.Add(e);
    }
}
