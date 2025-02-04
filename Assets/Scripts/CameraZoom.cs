using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // äùç÷ï ùäîöìîä òå÷áú àçøéå
    public float smoothSpeed = 5f; // îäéøåú äúðåòä ùì äîöìîä
    public Vector3 offset = new Vector3(0f, 2f, -10f); // îé÷åí éçñé áéï äîöìîä ìùç÷ï
    public float zoomSize = 5f; // øîú äæåí ùì äîöìîä

    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 10f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographicSize = zoomSize; // ÷áéòú âåãì äæåí
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Determine the desired position based on the player's position and the offset.
        Vector3 targetPosition = player.position + offset;

        if(targetPosition.x < minX){
            targetPosition.x = minX;
        }
        else if(targetPosition.x > maxX){
            targetPosition.x = maxX;
        }
        if(targetPosition.y < minY){
            targetPosition.y = minY;
        }
        else if(targetPosition.y > maxY){
            targetPosition.y = maxY;
        }
        // Smoothly move the camera toward the clamped target position.
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}