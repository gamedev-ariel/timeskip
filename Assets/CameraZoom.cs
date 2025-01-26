//using UnityEngine;

//public class CameraZoomOnStart : MonoBehaviour
//{
//    public Transform player; // השחקן שהמצלמה מתמקדת בו
//    public float zoomSize = 3.5f; // גודל הזום הרצוי
//    private Camera cam;
//    private Vector3 initialPosition;

//    void Start()
//    {
//        cam = GetComponent<Camera>();

//        // שמירת מיקום התחלתי של המצלמה
//        initialPosition = transform.position;

//        // שינוי גודל הזום של המצלמה
//        cam.orthographicSize = zoomSize;

//        // קיבוע המצלמה לנקודת המוצא של השחקן
//        transform.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
//    }
//}



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
