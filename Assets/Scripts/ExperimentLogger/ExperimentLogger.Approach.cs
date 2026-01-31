using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ExperimentLogger
{
    // --- Proximity approach detection (fish/blueberry) ---
    private readonly List<Transform> _approachTargets = new List<Transform>(32);
    private readonly Dictionary<Transform, float> _approachLastDist = new Dictionary<Transform, float>();
    private readonly Dictionary<Transform, float> _approachLastEmit = new Dictionary<Transform, float>();
    private float _approachNextRefresh;
    private Vector2 _approachLastPlayerPos;

    private const float ApproachRefreshInterval = 0.5f; // seconds
    private const float ApproachCooldown = 0.75f; // seconds per target
    private const float MinDistanceDecrease = 0.001f; // world units per frame to count as "moving towards" (more sensitive)

    private void ProcessApproachDetection()
    {
        // Only in forest/river contexts
        string area;
        try { area = GetCurrentArea(); } catch { return; }
        if (!string.Equals(area, "forest", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(area, "river", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        // Require camera and player
        var cam = GetActiveCamera();
        if (cam == null) return;
        if (!TryGetPlayerPos(out var playerPos)) return;

        // Compute threshold: 1/4 of screen diagonal in world units (at player's plane)
        float threshold = ComputeScreenQuarterDiagonalWorld(cam, _playerTransformCached != null ? _playerTransformCached.position.z : 0f);
        if (threshold <= 0f) return;

        // Periodically refresh target list
        if (Time.time >= _approachNextRefresh)
        {
            RefreshApproachTargets();
            _approachNextRefresh = Time.time + ApproachRefreshInterval;
        }

        // Player movement delta for direction heuristic
        Vector2 playerDelta = playerPos - _approachLastPlayerPos;

        for (int i = 0; i < _approachTargets.Count; i++)
        {
            var t = _approachTargets[i];
            if (t == null || !t.gameObject.activeInHierarchy) continue;

            Vector2 targetPos = t.position;
            float curDist = Vector2.Distance(playerPos, targetPos);

            // Previous distance
            float prevDist;
            if (!_approachLastDist.TryGetValue(t, out prevDist))
            {
                _approachLastDist[t] = curDist;
                continue;
            }

            bool within = curDist <= threshold;
            bool gettingCloser = (prevDist - curDist) > MinDistanceDecrease;
            bool movingToward = true;
            if (playerDelta.sqrMagnitude > 0.0001f)
            {
                var toTarget = (targetPos - playerPos).normalized;
                var moveDir = playerDelta.normalized;
                movingToward = Vector2.Dot(moveDir, toTarget) > 0.1f; // small positive alignment
            }

            if (within && gettingCloser && movingToward)
            {
                float lastEmit;
                if (!_approachLastEmit.TryGetValue(t, out lastEmit) || (Time.time - lastEmit) >= ApproachCooldown)
                {
                    try { EmitApproachEvent(t, targetPos, curDist, threshold); } catch { }
                    _approachLastEmit[t] = Time.time;
                }
            }

            _approachLastDist[t] = curDist;
        }

        _approachLastPlayerPos = playerPos;
    }

    private Camera GetActiveCamera()
    {
        try
        {
            if (Camera.main != null) return Camera.main;
        }
        catch { }
        try
        {
            if (Camera.current != null) return Camera.current;
        }
        catch { }
        try
        {
            var cams = GameObject.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < cams.Length; i++)
            {
                var c = cams[i];
                if (c != null && c.isActiveAndEnabled) return c;
            }
            // fallback: any camera
            if (cams.Length > 0) return cams[0];
        }
        catch { }
        return null;
    }

    private float ComputeScreenQuarterDiagonalWorld(Camera cam, float playerZ)
    {
        try
        {
            if (cam.orthographic)
            {
                float h = cam.orthographicSize * 2f;
                float w = h * cam.aspect;
                float diag = Mathf.Sqrt(w * w + h * h);
                return 0.25f * diag;
            }
            else
            {
                float d = Mathf.Abs(cam.transform.position.z - playerZ);
                Vector3 a = cam.ViewportToWorldPoint(new Vector3(0f, 0f, d));
                Vector3 b = cam.ViewportToWorldPoint(new Vector3(1f, 1f, d));
                float diag = Vector3.Distance(a, b);
                return 0.25f * diag;
            }
        }
        catch { return 0f; }
    }

    private void RefreshApproachTargets()
    {
        _approachTargets.Clear();
        // Fish by tag
        try
        {
            var fish = GameObject.FindGameObjectsWithTag("Fish");
            for (int i = 0; i < fish.Length; i++)
            {
                var go = fish[i];
                if (go == null || !go.activeInHierarchy) continue;
                string ln = go.name.ToLowerInvariant();
                if (ln.Contains("spawner") || ln.Contains("manager") || ln.Contains("controller")) continue;
                _approachTargets.Add(go.transform);
            }
        }
        catch { }
        // Blueberries by tag
        try
        {
            var blues = GameObject.FindGameObjectsWithTag("Blueberry");
            for (int i = 0; i < blues.Length; i++)
            {
                var go = blues[i];
                if (go == null || !go.activeInHierarchy) continue;
                string ln = go.name.ToLowerInvariant();
                if (ln.Contains("spawner") || ln.Contains("manager") || ln.Contains("controller")) continue;
                _approachTargets.Add(go.transform);
            }
        }
        catch { }
        // Blueberries by name fallback (in case tag not set)
        try
        {
            var all = GameObject.FindObjectsOfType<Transform>(true);
            for (int i = 0; i < all.Length; i++)
            {
                var t = all[i];
                var go = t.gameObject;
                if (!go.activeInHierarchy) continue;
                string ln = go.name.ToLowerInvariant();
                bool isSpawnerLike = ln.Contains("spawner") || ln.Contains("manager") || ln.Contains("controller");
                if (!isSpawnerLike && go.name.IndexOf("blueberry", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Avoid duplicates when tag already matched
                    if (!_approachTargets.Contains(t)) _approachTargets.Add(t);
                }
                else if (!isSpawnerLike && go.name.IndexOf("fish", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Add fish by name as a fallback too
                    if (!_approachTargets.Contains(t)) _approachTargets.Add(t);
                }
            }
        }
        catch { }
        // Exclude screws explicitly (shouldn't be included, but double-guard)
        for (int i = _approachTargets.Count - 1; i >= 0; i--)
        {
            var go = _approachTargets[i] != null ? _approachTargets[i].gameObject : null;
            if (go == null) { _approachTargets.RemoveAt(i); continue; }
            try
            {
                if (go.CompareTag("Screw") || go.name.IndexOf("screw", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _approachTargets.RemoveAt(i);
                }
            }
            catch { }
        }
    }

    private void EmitApproachEvent(Transform target, Vector2 targetPos, float distance, float threshold)
    {
        if (session == null) return;
        string area = GetCurrentArea();
        var section = EnsureSceneSection(area);
        string targetType = InferTargetType(target);
        string targetId = !string.IsNullOrEmpty(target.name) ? target.name : "unknown";
        var e = new ProximityApproachEvent
        {
            scene = area,
            targetType = targetType,
            targetId = targetId,
            targetPos = targetPos,
            time = DateTime.UtcNow.ToString("o"),
            timeSinceStart = R2(Time.time - sessionStartTime),
            timeSinceSceneStart = R2(Time.time - currentSceneStartTime),
            distance = distance,
            threshold = threshold
        };
        try
        {
            if (TryGetPlayerPos(out var p)) e.playerPos = p;
        }
        catch { }
        section.approaches.Add(e);
    }

    private string InferTargetType(Transform t)
    {
        try
        {
            if (t == null) return "unknown";
            var go = t.gameObject;
            if (go.CompareTag("Fish") || go.name.IndexOf("fish", StringComparison.OrdinalIgnoreCase) >= 0)
                return "fish";
            if (go.CompareTag("Blueberry") || go.name.IndexOf("blueberry", StringComparison.OrdinalIgnoreCase) >= 0)
                return "blueberry";
        }
        catch { }
        return "unknown";
    }
}
