using UnityEngine;

public class HouseTrigger2D : MonoBehaviour
{
    private InstructionManager instructionManager;

    void Start()
    {
        instructionManager = FindObjectOfType<InstructionManager>();
    }

    // Called when the player enters the trigger.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            instructionManager.TriggerHouseInstruction();
        }
    }
    
    // Added: Called when the player exits the trigger.
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            instructionManager.ClearHouseInstruction();
        }
    }
}
