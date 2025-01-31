using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // äùç÷ï ùäîöìîä òå÷áú àçøéå
    public float smoothSpeed = 5f; // îäéøåú äúðåòä ùì äîöìîä
    public Vector3 offset = new Vector3(0f, 2f, -10f); // îé÷åí éçñé áéï äîöìîä ìùç÷ï
    public float zoomSize = 5f; // øîú äæåí ùì äîöìîä

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

        // òå÷á àçøé äùç÷ï òí úðåòä çì÷ä
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}