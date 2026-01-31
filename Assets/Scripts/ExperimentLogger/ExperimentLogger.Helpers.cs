using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class ExperimentLogger
{
    // Runtime-only maps to speed up grouping; not serialized
    private readonly Dictionary<string, SceneSection> _sceneSectionMap = new Dictionary<string, SceneSection>();
    private readonly Dictionary<string, MinigameSection> _minigameSectionMap = new Dictionary<string, MinigameSection>(); // key: scene+"|"+minigame

    private string GetGroupedSceneName(string actualSceneName)
    {
        // MemoryGame is grouped under dwarf scene
        if (string.Equals(actualSceneName, "MemoryGame", StringComparison.OrdinalIgnoreCase))
            return "dwarf";
        return actualSceneName;
    }

    private string GetEffectiveMinigameName(string explicitMinigame)
    {
        if (!string.IsNullOrEmpty(explicitMinigame)) return explicitMinigame;
        // If we are inside the MemoryGame scene, force minigame to "memory"
        if (string.Equals(SceneManager.GetActiveScene().name, "MemoryGame", StringComparison.OrdinalIgnoreCase))
            return "memory";
        return currentMinigameName;
    }

    private SceneSection EnsureSceneSection(string sceneName)
    {
        if (!_sceneSectionMap.TryGetValue(sceneName, out var section))
        {
            section = new SceneSection { scene = sceneName };
            _sceneSectionMap[sceneName] = section;
            session.scenes.Add(section);
        }
        return section;
    }

    private MinigameSection EnsureMinigameSection(SceneSection section, string minigameName)
    {
        if (string.IsNullOrEmpty(minigameName)) minigameName = "unknown";
        string key = section.scene + "|" + minigameName;
        if (!_minigameSectionMap.TryGetValue(key, out var mg))
        {
            mg = new MinigameSection { name = minigameName };
            _minigameSectionMap[key] = mg;
            section.minigames.Add(mg);
        }
        return mg;
    }

    private float R2(float v)
    {
        return Mathf.Round(v * 100f) / 100f;
    }

    // --- Player position helpers ---
    private Transform _playerTransformCached;
    private int _playerLastLookupFrame = -1000;

    private bool ShouldAttachPlayerPos()
    {
        // Exclude dwarf/memory scenes per requirement. MemoryGame groups to "dwarf".
        string grouped = GetGroupedSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        return !string.Equals(grouped, "dwarf", StringComparison.OrdinalIgnoreCase);
    }

    private bool TryGetPlayerPos(out Vector2 pos)
    {
        pos = default;
        // Refresh cached transform at most once per frame
        if (_playerTransformCached == null || !_playerTransformCached.gameObject.activeInHierarchy)
        {
            if (_playerLastLookupFrame != Time.frameCount)
            {
                _playerLastLookupFrame = Time.frameCount;
                _playerTransformCached = null;
                try
                {
                    // First try by tag
                    var go = GameObject.FindWithTag("Player");
                    if (go != null) _playerTransformCached = go.transform;
                }
                catch { }
                // Fallback: common movement scripts
                if (_playerTransformCached == null)
                {
                    try
                    {
                        var pmr = GameObject.FindFirstObjectByType<PlayerMovementRiver>(FindObjectsInactive.Include);
                        if (pmr != null) _playerTransformCached = pmr.transform;
                    }
                    catch { }
                }
                if (_playerTransformCached == null)
                {
                    try
                    {
                        var pmf = GameObject.FindFirstObjectByType<PlayerMovementForest>(FindObjectsInactive.Include);
                        if (pmf != null) _playerTransformCached = pmf.transform;
                    }
                    catch { }
                }
            }
        }

        if (_playerTransformCached != null)
        {
            var p3 = _playerTransformCached.position;
            pos = new Vector2(p3.x, p3.y);
            return true;
        }
        return false;
    }
}
