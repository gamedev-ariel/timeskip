using System;
using UnityEngine;

public partial class ExperimentLogger
{
    private void LogMinigameEvent(string minigame, string eventType, string reason = "")
    {
        if (session == null) return;
        string mg = GetEffectiveMinigameName(minigame);
        float tMg = (!string.IsNullOrEmpty(currentMinigameName) && string.Equals(currentMinigameName, mg, StringComparison.Ordinal))
            ? (Time.time - currentMinigameStartTime)
            : 0f;
        string sceneName = GetGroupedSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        var e = new MiniGameEvent
        {
            time = DateTime.UtcNow.ToString("o"),
            timeSinceStart = R2(Time.time - sessionStartTime),
            timeSinceSceneStart = R2(Time.time - currentSceneStartTime),
            timeSinceMinigameStart = R2(tMg),
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            eventType = eventType,
            reason = reason
        };
        var section = EnsureSceneSection(sceneName);
        EnsureMinigameSection(section, mg).events.Add(e);
    }

    // Public API for Dwarf minigames
    public void LogDwarfMinigameStart(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "unknown";
        // Normalize name and avoid resetting start time if the same minigame is already active
        string norm = name.Trim().ToLowerInvariant();
        if (string.Equals(currentMinigameName, norm, StringComparison.Ordinal))
        {
            return; // already started for this minigame
        }
        currentMinigameName = norm;
        currentMinigameStartTime = Time.time;
        currentMinigameRunId = Guid.NewGuid().ToString();
        hasFinishedRun = false; // reset per-run final-outcome guard
        LogMinigameEvent(norm, "start");
    }

    public void LogDwarfMinigameEnd(string name = null, string reason = "")
    {
        var mg = GetEffectiveMinigameName(name);
        LogMinigameEvent(mg, "end", reason);
        // Set minigame final summary
        string sceneName = GetGroupedSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        var section = EnsureSceneSection(sceneName);
        var mgSection = EnsureMinigameSection(section, mg);
        mgSection.finishedTime = DateTime.UtcNow.ToString("o");
        mgSection.finishedSinceStart = R2(Time.time - sessionStartTime);
        mgSection.finishedSinceSceneStart = R2(Time.time - currentSceneStartTime);
        mgSection.finishedSinceMinigameStart = R2(Time.time - currentMinigameStartTime);
        // Heuristics to decide win/fail based on reason
        string lower = (reason ?? string.Empty).ToLowerInvariant();
        if (lower.Contains("game_over") || lower.Contains("fail"))
        {
            mgSection.finalResult = "fail";
            mgSection.finalReason = reason;
            LogOutcome("dwarf", "minigame_final_fail", $"minigame={mg}{(string.IsNullOrEmpty(reason)?"":";"+reason)}", 0, mg);
        }
        else if (lower.Contains("all_levels_cleared") || lower.Contains("reached_level_4") || lower.Contains("win"))
        {
            mgSection.finalResult = "win";
            mgSection.finalReason = reason;
            LogOutcome("dwarf", "minigame_final_win", $"minigame={mg}{(string.IsNullOrEmpty(reason)?"":";"+reason)}", 0, mg);
        }
        // Clear run id after logging any final outcomes
        currentMinigameRunId = null;
        if (string.IsNullOrEmpty(name) || string.Equals(name, currentMinigameName, StringComparison.Ordinal))
        {
            currentMinigameName = null;
            currentMinigameStartTime = 0f;
        }
    }

    public void LogDwarfMinigameWin(string name = null, string reason = "")
    {
        var mg = GetEffectiveMinigameName(name);
        LogOutcome("dwarf", "win", reason, 0, mg);
    }

    public void LogDwarfMinigameFail(string name = null, string reason = "touched_obstacle")
    {
        var mg = GetEffectiveMinigameName(name);
        LogOutcome("dwarf", "fail", reason, 0, mg);
    }
}
