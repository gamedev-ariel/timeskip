using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;
    public float spawnHeight = 3f;
    public float platformWidth = 2f;
    public int numberOfPlatforms = 5;
    public GameObject background;

    private Vector3 spawnPosition;
    private float backgroundWidth;

    void Start()
    {
        SetBackgroundBounds();  //background bounds
        SpawnPlatforms();  //steps maker
    }

    void SetBackgroundBounds()
    {
        backgroundWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void SpawnPlatforms()
    {
        spawnPosition = transform.position;

        for (int i = 0; i < numberOfPlatforms; i++)
        {
            float xPosition = Random.Range(-backgroundWidth / 2 + platformWidth / 2, backgroundWidth / 2 - platformWidth / 2);
            spawnPosition.x = xPosition;

            //make the step GameObject
            GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);

            // up to background
            platform.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, -1f);

            spawnPosition.y += spawnHeight;
        }
    }
}
