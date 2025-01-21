using UnityEngine;

public class EnvironmentSetup : MonoBehaviour
{
    public GameObject leftWall; 
    public GameObject rightWall;
    public GameObject floor;
    public GameObject background;

    void Start()
    {
        SetupWallsAndFloor();
    }

    void SetupWallsAndFloor()
    {
        //background bounds
        float backgroundWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
        float backgroundHeight = background.GetComponent<SpriteRenderer>().bounds.size.y;

        //leftWall position
        leftWall.transform.position = new Vector3(background.transform.position.x - backgroundWidth / 2, background.transform.position.y, 0);
        leftWall.transform.localScale = new Vector3(1, backgroundHeight, 1);

        //rightWall position
        rightWall.transform.position = new Vector3(background.transform.position.x + backgroundWidth / 2, background.transform.position.y, 0);
        rightWall.transform.localScale = new Vector3(1, backgroundHeight, 1);

        //floor position
        floor.transform.position = new Vector3(background.transform.position.x, background.transform.position.y - backgroundHeight / 2, 0);
        floor.transform.localScale = new Vector3(backgroundWidth, 1, 1);
    }
}
