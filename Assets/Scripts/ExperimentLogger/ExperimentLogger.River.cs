using System;
using UnityEngine;

public partial class ExperimentLogger
{
    public string LogRiverJumpStart(string fromRockId = null)
    {
        if (session == null) return null;
        // Allow logs in River or Forest areas; remap type names in Forest
        string cur = GetCurrentArea();
        if (!string.Equals(cur, "river", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(cur, "forest", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        // Attach river events under current area (e.g., "river")
        string area = cur;
        var section = EnsureSceneSection(area);
        string jumpId = Guid.NewGuid().ToString();
        var e = new RiverJumpStartEvent
        {
            scene = area,
            jumpId = jumpId,
            fromRockId = string.IsNullOrEmpty(fromRockId) ? "unknown" : fromRockId,
            time = DateTime.UtcNow.ToString("o"),
            tMini = 0f
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
        // Rename event type when in Forest
        if (string.Equals(area, "forest", StringComparison.OrdinalIgnoreCase))
        {
            e.type = "forest_jump_start";
        }
        section.riverJumpStarts.Add(e);
        return jumpId;
    }

    public void LogRiverJumpLand(string jumpId, string toRockId, bool success)
    {
        if (session == null) return;
        // Allow logs in River or Forest areas; remap type names in Forest
        string cur = GetCurrentArea();
        if (!string.Equals(cur, "river", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(cur, "forest", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        // De-dup: allow only one land log per frame per jumpId
        int frame = Time.frameCount;
        if (_lastRiverLandFrame == frame && string.Equals(_lastRiverLandJumpId, jumpId, StringComparison.Ordinal))
        {
            return;
        }
        _lastRiverLandFrame = frame;
        _lastRiverLandJumpId = jumpId;

        string area = cur;
        var section = EnsureSceneSection(area);
        var e = new RiverJumpLandEvent
        {
            scene = area,
            jumpId = string.IsNullOrEmpty(jumpId) ? null : jumpId,
            toRockId = string.IsNullOrEmpty(toRockId) ? "unknown" : toRockId,
            success = success,
            time = DateTime.UtcNow.ToString("o"),
            tMini = 0f
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
        if (string.Equals(area, "forest", StringComparison.OrdinalIgnoreCase))
        {
            e.type = "forest_jump_land";
        }
        section.riverJumpLands.Add(e);
    }

    public void LogRiverCollision(string with, Vector2 pos)
    {
        if (session == null) return;
        // Allow logs in River or Forest areas; remap type names in Forest
        string cur = GetCurrentArea();
        if (!string.Equals(cur, "river", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(cur, "forest", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        string area = cur;
        var section = EnsureSceneSection(area);
        var e = new RiverCollisionEvent
        {
            scene = area,
            with = string.IsNullOrEmpty(with) ? "unknown" : with,
            pos = pos,
            time = DateTime.UtcNow.ToString("o")
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
        if (string.Equals(area, "forest", StringComparison.OrdinalIgnoreCase))
        {
            e.type = "forest_collision";
        }
        section.riverCollisions.Add(e);
    }

    public void LogRiverFinish(string result)
    {
        if (session == null) return;
        // Allow logs in River or Forest areas; remap type names in Forest
        string cur = GetCurrentArea();
        if (!string.Equals(cur, "river", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(cur, "forest", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        string area = cur;
        var section = EnsureSceneSection(area);
        var e = new RiverFinishEvent
        {
            scene = area,
            result = string.IsNullOrEmpty(result) ? "unknown" : result,
            time = DateTime.UtcNow.ToString("o")
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
        if (string.Equals(area, "forest", StringComparison.OrdinalIgnoreCase))
        {
            e.type = "forest_finish";
        }
        section.riverFinish.Add(e);
    }
}
