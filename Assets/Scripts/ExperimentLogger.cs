using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[Serializable]
public class ClickEvent
{
    public string time;
    public float timeSinceStart;
    public string scene;
    public string area;
    public string target;
    public string inputType;
    public float x;
    public float y;
}

[Serializable]
public class OutcomeEvent
{
    public string time;
    public float timeSinceStart;
    public string scene;
    public string area;
    public string outcome;
    public string reason;
    public int livesLeft;
}

[Serializable]
public class KeyEvent
{
    public string time;
    public float timeSinceStart;
    public string scene;
    public string key;           // e.g., "Space", "A", "LeftArrow"
}

[Serializable]
public class SceneEvent
{
    public string time;
    public float timeSinceStart;
    public string scene;
    public string eventType; // loaded/unloaded/changed
}

[Serializable]
public class SessionData
{
    public string sessionId;
    public string playerName;
    public string startTime;
    public string endTime;
    public List<OutcomeEvent> outcomes = new List<OutcomeEvent>();
    public List<KeyEvent> keys = new List<KeyEvent>();
    public List<SceneEvent> scenes = new List<SceneEvent>();
}

public class ExperimentLogger : MonoBehaviour
{
    public static ExperimentLogger Instance { get; private set; }

    [Header("Session Info")]
    public string playerName = "";

    [Header("Supabase Settings")]
    [SerializeField] private string supabaseUrl = "https://npgswaexuhfhinxdxrmx.supabase.co";
    [SerializeField] private string supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5wZ3N3YWV4dWhmaGlueGR4cm14Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjUzMDc4MjksImV4cCI6MjA4MDg4MzgyOX0.x_scg6fi-dbJ2pKckNXkTUe7j3z2mnsqU1Fr2Emzn24";
    [SerializeField] private string supabaseTable = "session_logs";

    private SessionData session;
    private float sessionStartTime;
    private Vector3 lastMousePos;

    // limit key scan cost by caching enum values
    private static KeyCode[] allKeyCodes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        // Ensure Supabase is configured with provided values if placeholders were present
        if (string.IsNullOrWhiteSpace(supabaseUrl) || supabaseUrl.Contains("YOUR-PROJECT-REF"))
        {
            supabaseUrl = "https://npgswaexuhfhinxdxrmx.supabase.co";
        }
        if (string.IsNullOrWhiteSpace(supabaseAnonKey) || supabaseAnonKey.Contains("YOUR-ANON"))
        {
            supabaseAnonKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im5wZ3N3YWV4dWhmaGlueGR4cm14Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjUzMDc4MjksImV4cCI6MjA4MDg4MzgyOX0.x_scg6fi-dbJ2pKckNXkTUe7j3z2mnsqU1Fr2Emzn24";
        }
        if (string.IsNullOrWhiteSpace(supabaseTable))
        {
            supabaseTable = "session_logs";
        }
        StartNewSession();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureLoggerExists()
    {
        if (Instance == null)
        {
            var go = new GameObject("ExperimentLogger");
            go.AddComponent<ExperimentLogger>();
        }
    }

    public void StartNewSession()
    {
        sessionStartTime = Time.time;
        session = new SessionData
        {
            sessionId = Guid.NewGuid().ToString(),
            playerName = playerName,
            startTime = DateTime.UtcNow.ToString("o")
        };

        if (allKeyCodes == null)
        {
            allKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        LogSceneEvent("loaded");
    }

    public void SetPlayerName(string name, bool applyToCurrentSession = true)
    {
        playerName = name ?? string.Empty;
        if (applyToCurrentSession && session != null)
        {
            session.playerName = playerName;
        }
    }

    public void LogClick(string area, string target, Vector2 position, string inputType = "mouseLeft")
    {
        // Click logging removed per requirements. Keep method for API compatibility.
        return;
    }

    public void LogOutcome(string area, string outcome, string reason = "", int livesLeft = 0)
    {
        if (session == null) return;

        var e = new OutcomeEvent
        {
            time = DateTime.UtcNow.ToString("o"),
            timeSinceStart = Time.time - sessionStartTime,
            scene = SceneManager.GetActiveScene().name,
            area = area,
            outcome = outcome,
            reason = reason,
            livesLeft = livesLeft
        };
        session.outcomes.Add(e);
    }

    private void LogSceneEvent(string type)
    {
        if (session == null) return;
        session.scenes.Add(new SceneEvent
        {
            time = DateTime.UtcNow.ToString("o"),
            timeSinceStart = Time.time - sessionStartTime,
            scene = SceneManager.GetActiveScene().name,
            eventType = type
        });
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LogSceneEvent("loaded");
    }

    private void OnSceneUnloaded(Scene scene)
    {
        LogSceneEvent("unloaded");
    }

    private void OnApplicationQuit()
    {
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

    private void Update()
    {
        // Key typing logging
        if (Input.anyKeyDown)
        {
            // detect which keycode was pressed
            string pressedKey = null;
            if (allKeyCodes == null)
            {
                allKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
            }
            for (int i = 0; i < allKeyCodes.Length; i++)
            {
                var kc = allKeyCodes[i];
                // Skip None to reduce overhead
                if (kc == KeyCode.None) continue;
                if (Input.GetKeyDown(kc))
                {
                    pressedKey = kc.ToString();
                    break;
                }
            }

            var ke = new KeyEvent
            {
                time = DateTime.UtcNow.ToString("o"),
                timeSinceStart = Time.time - sessionStartTime,
                scene = SceneManager.GetActiveScene().name,
                key = pressedKey ?? "Unknown"
            };
            session.keys.Add(ke);
        }

    }

    private void AutoLogClick(string inputType)
    {
        try
        {
            Vector3 mousePos = Input.mousePosition;
            var targetName = "none";
            var area = "world";

            // First try UI raycast
            if (EventSystem.current != null)
            {
                var pointerData = new PointerEventData(EventSystem.current) { position = mousePos };
                var results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);
                if (results.Count > 0)
                {
                    targetName = results[0].gameObject.name;
                    area = "ui";
                }
            }

            // Then try 2D physics
            if (area == "world")
            {
                Vector3 worldPoint = Camera.main != null ? Camera.main.ScreenToWorldPoint(mousePos) : new Vector3(mousePos.x, mousePos.y, 0);
                var hit2D = Physics2D.OverlapPoint(new Vector2(worldPoint.x, worldPoint.y));
                if (hit2D != null)
                {
                    targetName = hit2D.gameObject.name;
                }
                else
                {
                    // Try 3D raycast
                    if (Camera.main != null)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(mousePos);
                        if (Physics.Raycast(ray, out RaycastHit hit))
                        {
                            targetName = hit.collider.gameObject.name;
                        }
                    }
                }
            }

            LogClick(area, targetName, mousePos, inputType);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[ExperimentLogger] AutoLogClick failed: {ex.Message}");
        }
    }

    public void EndAndSaveSession()
    {
        if (session == null) return;
        session.endTime = DateTime.UtcNow.ToString("o");
        StartCoroutine(SendSessionToSupabase());
    }

    private IEnumerator SendSessionToSupabase()
    {
        // Treat placeholder values as not configured to avoid noisy web errors
        bool notConfigured = string.IsNullOrWhiteSpace(supabaseUrl)
                             || string.IsNullOrWhiteSpace(supabaseAnonKey)
                             || string.IsNullOrWhiteSpace(supabaseTable)
                             || supabaseUrl.Contains("YOUR-PROJECT-REF")
                             || supabaseAnonKey.Contains("YOUR-ANON");

        if (notConfigured)
        {
            Debug.LogWarning("[ExperimentLogger] Supabase not configured. Saving session locally instead.");
            yield return SaveSessionLocally();
            yield break;
        }

        var row = new SupabaseSessionRow
        {
            player_name = playerName,
            session_id = session.sessionId,
            session_json = session
        };

        string jsonBody = JsonUtility.ToJson(row);

        string url = $"{supabaseUrl}/rest/v1/{supabaseTable}";

        using (UnityWebRequest req = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();

            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("apikey", supabaseAnonKey);
            req.SetRequestHeader("Authorization", "Bearer " + supabaseAnonKey);
            req.SetRequestHeader("Prefer", "return=minimal");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[ExperimentLogger] Supabase error: {req.responseCode} {req.error}\n{req.downloadHandler.text}");
                yield return SaveSessionLocally();
            }
            else
            {
                Debug.Log("[ExperimentLogger] Session successfully saved to Supabase.");
            }
        }
    }

    private IEnumerator SaveSessionLocally()
    {
        try
        {
            string dir = Path.Combine(Application.persistentDataPath, "session_logs");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, $"{session.sessionId}.json");
            string json = JsonUtility.ToJson(session, true);
            File.WriteAllText(path, json, Encoding.UTF8);
            Debug.Log($"[ExperimentLogger] Session saved locally at {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ExperimentLogger] Failed to save locally: {ex.Message}");
        }
        yield break;
    }

    [Serializable]
    public class SupabaseSessionRow
    {
        public string player_name;
        public string session_id;
        public SessionData session_json;
    }
}