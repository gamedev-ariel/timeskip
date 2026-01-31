using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ExperimentLogger
{
    // Memory Game dedicated API (part of dwarf scene)
    public void LogMemoryLevelStart(int level)
    {
        // Ensure minigame context for timing
        if (!string.Equals(currentMinigameName, "memory", StringComparison.Ordinal))
        {
            currentMinigameName = "memory";
            currentMinigameStartTime = Time.time;
        }
        LogMinigameEvent("memory", "level_start", $"level={level}");
    }

    public void LogMemoryClick(bool correct, int? level = null, int livesLeft = -1)
    {
        if (session == null) return;
        var sceneName = GetGroupedSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        var section = EnsureSceneSection(sceneName);
        var mg = EnsureMinigameSection(section, "memory");
        float tMg = (string.Equals(currentMinigameName, "memory", StringComparison.Ordinal)) ? (Time.time - currentMinigameStartTime) : 0f;

        var e = new MemoryClickEvent
        {
            time = DateTime.UtcNow.ToString("o"),
            timeSinceStart = R2(Time.time - sessionStartTime),
            timeSinceSceneStart = R2(Time.time - currentSceneStartTime),
            timeSinceMinigameStart = R2(tMg),
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            level = level ?? -1,
            result = correct ? "right" : "wrong",
            livesLeft = livesLeft < 0 ? 0 : livesLeft
        };
        mg.clicks.Add(e);
    }

    public void LogMemoryLevelResult(int level, bool win, string reason = "", int livesLeft = -1)
    {
        if (session == null) return;
        var sceneName = GetGroupedSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        var section = EnsureSceneSection(sceneName);
        var mg = EnsureMinigameSection(section, "memory");
        float tMg = (string.Equals(currentMinigameName, "memory", StringComparison.Ordinal)) ? (Time.time - currentMinigameStartTime) : 0f;

        var e = new MemoryLevelResult
        {
            time = DateTime.UtcNow.ToString("o"),
            timeSinceStart = R2(Time.time - sessionStartTime),
            timeSinceSceneStart = R2(Time.time - currentSceneStartTime),
            timeSinceMinigameStart = R2(tMg),
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            level = level,
            result = win ? "win" : "fail",
            reason = reason,
            livesLeft = livesLeft < 0 ? 0 : livesLeft
        };
        mg.levelResults.Add(e);
        // Also record as a generic outcome under the section for completeness
        LogOutcome("dwarf", win ? "memory_level_win" : "memory_level_fail", $"level={level}{(string.IsNullOrEmpty(reason)?"":";"+reason)}", 0, "memory");
    }

    // ------- Dwarf memory: per-level stimulus/response logging -------
    public void LogDwarfTrialStart(int level, IList<string> shownItems, string correctItem)
    {
        if (session == null) return;
        var sceneName = GetGroupedSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        var section = EnsureSceneSection(sceneName);
        var mg = EnsureMinigameSection(section, GetEffectiveMinigameName("memory"));

        var e = new DwarfTrialStartEvent
        {
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            minigameRunId = currentMinigameRunId,
            level = level,
            shownItems = shownItems != null ? new List<string>(shownItems) : new List<string>(),
            correctItem = string.IsNullOrEmpty(correctItem) ? "unknown" : correctItem,
            time = DateTime.UtcNow.ToString("o")
        };
        mg.dwarfTrialStarts.Add(e);
    }

    public void LogDwarfTrialResponse(int level, string selectedItem, bool isCorrect, int reactionTimeMs)
    {
        if (session == null) return;
        var sceneName = GetGroupedSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        var section = EnsureSceneSection(sceneName);
        var mg = EnsureMinigameSection(section, GetEffectiveMinigameName("memory"));

        var e = new DwarfTrialResponseEvent
        {
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            level = level,
            selectedItem = string.IsNullOrEmpty(selectedItem) ? "unknown" : selectedItem,
            isCorrect = isCorrect,
            reactionTimeMs = Mathf.Max(0, reactionTimeMs),
            time = DateTime.UtcNow.ToString("o")
        };
        mg.dwarfTrialResponses.Add(e);
    }
}
