using UnityEngine;

public class RockTrigger2D : MonoBehaviour
{
    private InstructionManager instructionManager;

    void Start()
    {
        instructionManager = FindObjectOfType<InstructionManager>();
    }

    // Called when the player enters the rock trigger.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            instructionManager.TriggerRockInstruction();
        }
    }

    // Called when the player exits the rock trigger.
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            instructionManager.ClearRockInstruction();
        }
    }
}
