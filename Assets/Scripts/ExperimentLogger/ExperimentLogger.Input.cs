using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class ExperimentLogger
{
    // Reusable buffer to avoid allocations each frame
    private readonly List<KeyCode> _keysBuffer = new List<KeyCode>(16);

    private void Update()
    {
        // Keyboard input logging with explicit action and held duration
        if (Input.anyKeyDown)
        {
            if (allKeyCodes == null)
            {
                allKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
            }
            for (int i = 0; i < allKeyCodes.Length; i++)
            {
                var kc = allKeyCodes[i];
                if (!IsKeyboardKey(kc)) continue;
                if (Input.GetKeyDown(kc))
                {
                    LogKeyInput(kc, action: "down", heldMs: 0f, isRepeat: false);
                    // start timer for held duration
                    _keyPressStartTimes[kc] = Time.time;
                }
            }
        }

        // Detect key ups only for keys that are currently held
        if (_keyPressStartTimes.Count > 0)
        {
            // Copy keys to avoid modifying collection during iteration
            _keysBuffer.Clear();
            foreach (var k in _keyPressStartTimes.Keys)
                _keysBuffer.Add(k);

            for (int i = 0; i < _keysBuffer.Count; i++)
            {
                var kc = _keysBuffer[i];
                if (Input.GetKeyUp(kc))
                {
                    var start = _keyPressStartTimes.TryGetValue(kc, out var t0) ? t0 : Time.time;
                    float heldMs = (Time.time - start) * 1000f;
                    // round similarly to R2 but keep ms precision to 1/100th
                    heldMs = R2(heldMs);
                    LogKeyInput(kc, action: "up", heldMs: heldMs, isRepeat: false);
                    _keyPressStartTimes.Remove(kc);
                }
            }
        }
        // All generic clicks disabled by request
        // Mouse logging (Memory/Dwarf only) — new schema with button/action/positions
        try
        {
            // Determine if we are in the memory minigame context (scene or active minigame)
            bool inMemory = string.Equals(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, "MemoryGame", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(currentMinigameName, "memory", StringComparison.Ordinal);
            if (inMemory)
            {
                // Left button (0)
                if (Input.GetMouseButtonDown(0))
                {
                    EmitMouseInput("left", "down");
                }
                if (Input.GetMouseButtonUp(0))
                {
                    EmitMouseInput("left", "up");
                }
                // Right button (1)
                if (Input.GetMouseButtonDown(1))
                {
                    EmitMouseInput("right", "down");
                }
                if (Input.GetMouseButtonUp(1))
                {
                    EmitMouseInput("right", "up");
                }
            }
        }
        catch (Exception) { }

        // Proximity approach detection (fish/blueberry, not screws)
        try { ProcessApproachDetection(); } catch (Exception) { }
    }

    private void LogKeyInput(KeyCode kc, string action, float heldMs, bool isRepeat)
    {
        string sceneName = GetGroupedSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        var ke = new KeyEvent
        {
            time = DateTime.UtcNow.ToString("o"),
            timeSinceStart = R2(Time.time - sessionStartTime),
            timeSinceSceneStart = R2(Time.time - currentSceneStartTime),
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            action = action,
            key = kc.ToString(),
            heldMs = heldMs,
            isRepeat = isRepeat
        };
        // Attach player position except in dwarf/memory grouped scenes
        try
        {
            if (ShouldAttachPlayerPos() && TryGetPlayerPos(out var p))
            {
                ke.playerPos = p;
            }
        }
        catch { }
        EnsureSceneSection(sceneName).keys.Add(ke);
    }

    private bool IsKeyboardKey(KeyCode kc)
    {
        if (kc == KeyCode.None) return false;
        string n = kc.ToString();
        if (n.StartsWith("Mouse", StringComparison.Ordinal)) return false;
        if (n.StartsWith("Joystick", StringComparison.Ordinal)) return false;
        return true;
    }

    private void EmitMouseInput(string button, string action)
    {
        if (session == null) return;
        // Only for memory minigame
        string mg = GetEffectiveMinigameName("memory");
        if (!string.Equals(mg, "memory", StringComparison.Ordinal)) return;
        // Do not suppress UI clicks in Memory: we need these for RT metrics

        string sceneName = GetGroupedSceneName(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        var section = EnsureSceneSection(sceneName);
        var mgSection = EnsureMinigameSection(section, mg);
        Vector3 sp = Input.mousePosition;
        Vector3 wp3 = (Camera.main != null) ? Camera.main.ScreenToWorldPoint(sp) : new Vector3(0, 0, 0);
        Vector2 wp = new Vector2(wp3.x, wp3.y);
        float tMini = (!string.IsNullOrEmpty(currentMinigameName) && string.Equals(currentMinigameName, mg, StringComparison.Ordinal))
            ? R2(Time.time - currentMinigameStartTime) : 0f;
        var e = new MouseInputEvent
        {
            time = DateTime.UtcNow.ToString("o"),
            scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
            sceneVisitId = currentSceneVisitId,
            minigameRunId = currentMinigameRunId,
            button = button,
            action = action,
            screenPos = new Vector2(sp.x, sp.y),
            worldPos = wp,
            tScene = R2(Time.time - currentSceneStartTime),
            tMini = tMini
        };
        mgSection.mouseInputs.Add(e);
    }
}
