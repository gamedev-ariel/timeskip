using System;
using UnityEngine;

// Attach to any Fish GameObject to log a spawn timestamp once per activation
public class FishTrack : MonoBehaviour
{
    private bool _logged;

    private void OnEnable()
    {
        TryLogSpawn();
    }

    private void Start()
    {
        // Safety: in case OnEnable ran before logger existed
        if (!_logged)
            TryLogSpawn();
    }

    private void TryLogSpawn()
    {
        if (_logged) return;
        var logger = ExperimentLogger.Instance;
        if (logger == null) return;
        try
        {
            string baseId = !string.IsNullOrEmpty(gameObject.name) ? gameObject.name : "fish";
            string id = baseId + "#" + gameObject.GetInstanceID();
            Vector3 p3 = transform.position;
            Vector2 p = new Vector2(p3.x, p3.y);
            logger.LogFishSpawn(id, p);
            _logged = true;
        }
        catch (Exception)
        {
            // ignore
        }
    }
}
