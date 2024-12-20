using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionArea : MonoBehaviour
{
    [Tooltip("The name of the scene to load when the player enters this area")]
    public string targetSceneName;

    [Tooltip("The x or y coordinate where the player should spawn in the new scene")]
    public float targetSpawnCoordinate;

    [Tooltip("Determines if the spawn coordinate is for X (horizontal) or Y (vertical) axis")]
    public bool isHorizontalTransition = true;

    [Tooltip("Offset to fine-tune the spawn position")]
    public float spawnOffset = 0f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TransitionToScene(collision.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TransitionToScene(collision.gameObject);
        }
    }

    private void TransitionToScene(GameObject player)
    {
        // Check if the target scene name is set
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Target scene name is not set for SceneTransitionArea!");
            return;
        }

        // Store the player's position to use in the next scene
        Vector3 playerPosition = player.transform.position;

        // Create a new position for the player in the next scene
        Vector3 newPosition;
        if (isHorizontalTransition)
        {
            // Preserve Y coordinate, set new X
            newPosition = new Vector3(targetSpawnCoordinate + spawnOffset, playerPosition.y, playerPosition.z);
        }
        else
        {
            // Preserve X coordinate, set new Y
            newPosition = new Vector3(playerPosition.x, targetSpawnCoordinate + spawnOffset, playerPosition.z);
        }

        // Pass the spawn position to the next scene
        PlayerSpawnManager.Instance.SetSpawnPosition(newPosition);

        // Load the target scene
        SceneManager.LoadScene(targetSceneName);
    }
}