using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // השחקן שהמצלמה עוקבת אחריו
    public float smoothSpeed = 5f; // מהירות התנועה של המצלמה
    public Vector3 offset = new Vector3(0f, 2f, -10f); // מיקום יחסי בין המצלמה לשחקן
    public float zoomSize = 5f; // רמת הזום של המצלמה

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographicSize = zoomSize; // קביעת גודל הזום
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // עוקב אחרי השחקן עם תנועה חלקה
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
