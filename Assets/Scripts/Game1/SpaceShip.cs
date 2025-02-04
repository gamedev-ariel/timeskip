using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField] private float flySpeed = 2f;
    private bool shouldFly = false;

    private void Update()
    {
        if (shouldFly)
        {
            transform.Translate(Vector2.up * flySpeed * Time.deltaTime);
            
            if (transform.position.y >= 200f)
            {
                Debug.Log("Spaceship reached target height - quitting game");
                Application.Quit();
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #endif
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            shouldFly = true;
        }
    }
}