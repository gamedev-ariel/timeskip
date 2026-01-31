using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class ExperimentLogger
{
    private void StartSceneVisit(string newSceneName, string fromSceneName)
    {
        if (session == null) return;
        // Close any dangling visit first to guarantee one enter+exit per visit
        if (hasOpenSceneVisit)
        {
            EndSceneVisit(newSceneName);
        }

        currentSceneStartTime = Time.time;
        currentSceneNameGrouped = newSceneName;
        currentSceneVisitId = Guid.NewGuid().ToString();
        hasOpenSceneVisit = true;

        var enterEvent = new SceneEvent
        {
            time = DateTime.UtcNow.ToString("o"),
            timeSinceStart = R2(Time.time - sessionStartTime),
            timeSinceSceneStart = R2(Time.time - currentSceneStartTime), // ~0 at enter
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            eventType = "scene_enter",
            sceneVisitId = currentSceneVisitId,
            fromScene = fromSceneName,
            toScene = newSceneName
        };
        // Attach player position except in dwarf/memory grouped scenes
        try
        {
            if (ShouldAttachPlayerPos() && TryGetPlayerPos(out var p))
            {
                enterEvent.playerPos = p;
            }
        }
        catch { }
        EnsureSceneSection(newSceneName).events.Add(enterEvent);

        // Reset per-visit fish spawn de-duplication set
        try { _fishSpawnedIdsInScene.Clear(); } catch { }
    }

    private void EndSceneVisit(string toSceneName)
    {
        if (session == null) return;
        if (!hasOpenSceneVisit) return;

        string sceneName = currentSceneNameGrouped ?? GetGroupedSceneName(SceneManager.GetActiveScene().name);
        var exitEvent = new SceneEvent
        {
            time = DateTime.UtcNow.ToString("o"),
            timeSinceStart = R2(Time.time - sessionStartTime),
            timeSinceSceneStart = R2(Time.time - currentSceneStartTime),
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            eventType = "scene_exit",
            sceneVisitId = currentSceneVisitId,
            fromScene = sceneName,
            toScene = toSceneName
        };
        // Attach player position except in dwarf/memory grouped scenes
        try
        {
            if (ShouldAttachPlayerPos() && TryGetPlayerPos(out var p))
            {
                exitEvent.playerPos = p;
            }
        }
        catch { }
        EnsureSceneSection(sceneName).events.Add(exitEvent);

        hasOpenSceneVisit = false;
        // Keep currentSceneNameGrouped as the last scene for context until a new visit starts
        currentSceneVisitId = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset cached player transform so it can be found in the new scene
        _playerTransformCached = null;
        _playerLastLookupFrame = -1000;
        // Reset current minigame context on scene switches
        currentMinigameName = null;
        currentMinigameStartTime = 0f;
        currentMinigameRunId = null;

        string newScene = GetGroupedSceneName(scene.name);
        // If this load event refers to the same scene as the active visit, ignore to prevent duplicates
        if (hasOpenSceneVisit && string.Equals(currentSceneNameGrouped, newScene, StringComparison.Ordinal))
        {
            return;
        }

        // Close previous visit if still open, and start a new one
        string fromScene = hasOpenSceneVisit ? currentSceneNameGrouped : null;
        if (hasOpenSceneVisit)
        {
            EndSceneVisit(newScene);
        }
        EnsureSceneSection(newScene);
        StartSceneVisit(newScene, fromScene);

        // Default area switches to the grouped scene when a new scene is loaded
        currentArea = newScene;
        currentAreaStartTime = Time.time;

        // Auto-attach forest item trackers when entering the forest scene
        if (string.Equals(newScene, "forest", StringComparison.OrdinalIgnoreCase))
        {
            // Quick start to catch existing ones next frame
            StartCoroutine(AttachForestTrackersNextFrame());
            // Extended window to catch early delayed spawns/activations
            StartCoroutine(AttachForestTrackersRepeated(30.0f, 0.5f));
            // Lightweight continuous scan during the visit
            StartCoroutine(AttachForestTrackersContinuous(3.0f));
        }
        // Auto-attach fish trackers in river and forest (forest may reuse river elements)
        if (string.Equals(newScene, "river", StringComparison.OrdinalIgnoreCase) || string.Equals(newScene, "forest", StringComparison.OrdinalIgnoreCase))
        {
            // Extended window for early/pooled fish activations
            StartCoroutine(AttachFishTrackersRepeated(30.0f, 0.5f));
            // Lightweight continuous scan during the visit for late spawns
            StartCoroutine(AttachFishTrackersContinuous(3.0f));
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        // Invalidate player cache when scene unloads
        _playerTransformCached = null;
        _playerLastLookupFrame = -1000;
        string unloaded = GetGroupedSceneName(scene.name);
        if (hasOpenSceneVisit && string.Equals(currentSceneNameGrouped, unloaded, StringComparison.Ordinal))
        {
            // We don't know the next scene here yet; toScene is optional
            EndSceneVisit(null);
        }
    }

    private void OnApplicationQuit()
    {
        // Ensure the current visit is closed before saving
        if (hasOpenSceneVisit)
        {
            EndSceneVisit(null);
        }
        EndAndSaveSession();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
    }

    private IEnumerator AttachForestTrackersNextFrame()
    {
        // wait one frame to ensure scene objects are spawned
        yield return null;
        try { AttachForestTrackers(); } catch (Exception ex) { Debug.LogWarning($"[ExperimentLogger] AttachForestTrackers failed: {ex.Message}"); }
    }

    private IEnumerator AttachFishTrackersRepeated(float durationSeconds, float intervalSeconds)
    {
        // wait one frame to ensure scene objects are spawned
        yield return null;
        float end = Time.time + Mathf.Max(0.25f, durationSeconds);
        while (Time.time < end)
        {
            try { AttachFishTrackers(); } catch (Exception ex) { Debug.LogWarning($"[ExperimentLogger] AttachFishTrackers failed: {ex.Message}"); }
            yield return new WaitForSeconds(Mathf.Max(0.1f, intervalSeconds));
        }
    }

    private IEnumerator AttachFishTrackersContinuous(float intervalSeconds)
    {
        // Periodically attach trackers while in forest/river visit
        while (hasOpenSceneVisit && (string.Equals(currentSceneNameGrouped, "river", StringComparison.OrdinalIgnoreCase) || string.Equals(currentSceneNameGrouped, "forest", StringComparison.OrdinalIgnoreCase)))
        {
            try { AttachFishTrackers(); } catch (Exception ex) { Debug.LogWarning($"[ExperimentLogger] AttachFishTrackers (continuous) failed: {ex.Message}"); }
            yield return new WaitForSeconds(Mathf.Max(0.5f, intervalSeconds));
        }
    }

    private IEnumerator AttachForestTrackersRepeated(float durationSeconds, float intervalSeconds)
    {
        // wait one frame to ensure scene objects are spawned
        yield return null;
        float end = Time.time + Mathf.Max(0.25f, durationSeconds);
        while (Time.time < end)
        {
            try { AttachForestTrackers(); } catch (Exception ex) { Debug.LogWarning($"[ExperimentLogger] AttachForestTrackers failed: {ex.Message}"); }
            yield return new WaitForSeconds(Mathf.Max(0.1f, intervalSeconds));
        }
    }

    private IEnumerator AttachForestTrackersContinuous(float intervalSeconds)
    {
        while (hasOpenSceneVisit && string.Equals(currentSceneNameGrouped, "forest", StringComparison.OrdinalIgnoreCase))
        {
            try { AttachForestTrackers(); } catch (Exception ex) { Debug.LogWarning($"[ExperimentLogger] AttachForestTrackers (continuous) failed: {ex.Message}"); }
            yield return new WaitForSeconds(Mathf.Max(0.5f, intervalSeconds));
        }
    }

    private void AttachForestTrackers()
    {
        // Find potential forest collectible items and ensure they have a tracker
        var allTransforms = GameObject.FindObjectsOfType<Transform>(true);
        for (int i = 0; i < allTransforms.Length; i++)
        {
            var t = allTransforms[i];
            var go = t.gameObject;
            // Attach to both active and inactive so OnEnable will log upon activation
            // Identify by tag or name
            string type = null;
            string lowerName = go.name.ToLowerInvariant();
            // Exclude spawners/managers/controllers
            if (lowerName.Contains("spawner") || lowerName.Contains("manager") || lowerName.Contains("controller"))
                continue;
            if (SafeHasTag(go, "Screw") || go.name.IndexOf("screw", StringComparison.OrdinalIgnoreCase) >= 0)
                type = "screw";
            else if (SafeHasTag(go, "Blueberry") || go.name.IndexOf("blueberry", StringComparison.OrdinalIgnoreCase) >= 0)
                type = "blueberry";
            if (type == null) continue;

            var tracker = go.GetComponent<ForestItemTrack>();
            if (tracker == null)
            {
                tracker = go.AddComponent<ForestItemTrack>();
            }
            if (string.IsNullOrEmpty(tracker.itemType)) tracker.itemType = type;
        }
    }

    private bool SafeHasTag(GameObject go, string tag)
    {
        try { return go.CompareTag(tag); } catch { return false; }
    }

    private void AttachFishTrackers()
    {
        var allTransforms = GameObject.FindObjectsOfType<Transform>(true);
        for (int i = 0; i < allTransforms.Length; i++)
        {
            var t = allTransforms[i];
            var go = t.gameObject;
            // Attach to both active and inactive so OnEnable/Start will log upon activation
            bool isFish = false;
            try { isFish = go.CompareTag("Fish"); } catch { }
            string lowerName = go.name.ToLowerInvariant();
            // Fallback by name, but exclude spawners/managers/controllers and blueberries
            if (!isFish)
            {
                if (lowerName.Contains("fish"))
                {
                    if (lowerName.Contains("spawner") || lowerName.Contains("manager") || lowerName.Contains("controller") || lowerName.Contains("blueberry"))
                    {
                        continue;
                    }
                    isFish = true;
                }
            }
            else
            {
                // Even if tagged Fish, exclude obvious non-fish helpers
                if (lowerName.Contains("spawner") || lowerName.Contains("manager") || lowerName.Contains("controller"))
                {
                    continue;
                }
            }
            if (!isFish) continue;

            var tracker = go.GetComponent<FishTrack>();
            if (tracker == null)
            {
                tracker = go.AddComponent<FishTrack>();
            }
        }
    }
}
