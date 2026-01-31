using UnityEngine;

// Attach this to a trigger collider placed at the Forest/River boundary
// When the player enters/exits, it switches ExperimentLogger's current area
// so logs use "forest" or "river" labels even within the same Unity scene.
public class AreaSetter : MonoBehaviour
{
    public enum TriggerPhase { OnEnter, OnExit }

    [Tooltip("Area name to set when triggered: 'forest' or 'river'. Any other value will be ignored.")]
    public string areaName = "river";

    [Tooltip("Whether to set the area on trigger enter or exit.")]
    public TriggerPhase triggerPhase = TriggerPhase.OnEnter;

    [Tooltip("Only react to objects with this tag. Leave empty to accept any.")]
    public string requiredTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerPhase != TriggerPhase.OnEnter) return;
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;
        try { ExperimentLogger.Instance?.SetArea(areaName); } catch { }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (triggerPhase != TriggerPhase.OnExit) return;
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag)) return;
        try { ExperimentLogger.Instance?.SetArea(areaName); } catch { }
    }
}
