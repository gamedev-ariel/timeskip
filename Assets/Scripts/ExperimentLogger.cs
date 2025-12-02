using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
public class SessionData
{
    public string sessionId;
    public string playerName;
    public string startTime;
    public string endTime;
    public List<ClickEvent> clicks = new List<ClickEvent>();
    public List<OutcomeEvent> outcomes = new List<OutcomeEvent>();
}

public class ExperimentLogger : MonoBehaviour
{
    public static ExperimentLogger Instance { get; private set; }

    [Header("Session Info")]
    public string playerName = "";

    [Header("Supabase Settings")]
    [SerializeField] private string supabaseUrl = "https://YOUR-PROJECT-REF.supabase.co";
    [SerializeField] private string supabaseAnonKey = "YOUR-ANON-PUBLIC-KEY";
    [SerializeField] private string supabaseTable = "session_logs";

    private SessionData session;
    private float sessionStartTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartNewSession();
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
    }

    public void LogClick(string area, string target, Vector2 position, string inputType = "mouseLeft")
    {
        if (session == null) return;

        var e = new ClickEvent
        {
            time = DateTime.UtcNow.ToString("o"),
            timeSinceStart = Time.time - sessionStartTime,
            scene = SceneManager.GetActiveScene().name,
            area = area,
            target = target,
            inputType = inputType,
            x = position.x,
            y = position.y
        };
        session.clicks.Add(e);
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

    public void EndAndSaveSession()
    {
        if (session == null) return;
        session.endTime = DateTime.UtcNow.ToString("o");
        StartCoroutine(SendSessionToSupabase());
    }

    private IEnumerator SendSessionToSupabase()
    {
        if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseAnonKey))
        {
            Debug.LogError("[ExperimentLogger] Supabase URL or anon key not set!");
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
            }
            else
            {
                Debug.Log("[ExperimentLogger] Session successfully saved to Supabase.");
            }
        }
    }

    [Serializable]
    private class SupabaseSessionRow
    {
        public string player_name;
        public string session_id;
        public SessionData session_json;
    }
}
