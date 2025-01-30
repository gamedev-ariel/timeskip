//using UnityEngine;

//public class CameraFollow : MonoBehaviour
//{
//    public Transform player; // השחקן שהמצלמה עוקבת אחריו
//    public float smoothSpeed = 5f; // מהירות התנועה של המצלמה
//    public Vector3 offset = new Vector3(0f, 2f, -10f); // מיקום יחסי בין המצלמה לשחקן
//    public float zoomSize = 5f; // רמת הזום של המצלמה

//    private Camera cam;

//    void Start()
//    {
//        cam = GetComponent<Camera>();
//        if (cam != null)
//        {
//            cam.orthographicSize = zoomSize; // קביעת גודל הזום
//        }
//    }

//    void LateUpdate()
//    {
//        if (player == null) return;

//        // עוקב אחרי השחקן עם תנועה חלקה
//        Vector3 targetPosition = player.position + offset;
//        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
//    }
//}


//using UnityEngine;

//public class CameraFollow : MonoBehaviour
//{
//    public Transform player;   // השחקן
//    public float smoothSpeed = 0.125f;  // מהירות המעבר
//    public Vector3 offset;  // המרחק מהשחקן

//    void FixedUpdate()
//    {
//        // מציב את המיקום של המצלמה כדי לעקוב אחרי השחקן
//        Vector3 desiredPosition = new Vector3(player.position.x + offset.x, transform.position.y, offset.z);
//        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
//        transform.position = smoothedPosition;
//    }
//}



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