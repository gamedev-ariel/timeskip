using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    public Transform target; // השחקן שהמצלמה עוקבת אחריו
    public Transform wallLeft; // הקיר השמאלי שמגביל את המצלמה
    public Transform wallRight; // הקיר הימני שמגביל את המצלמה
    public float smoothSpeed = 0.125f; // מידת החלקות של התנועה

    private float cameraHalfWidth;

    void Start()
    {
        // מחשב את חצי הרוחב של המצלמה
        Camera cam = Camera.main;
        cameraHalfWidth = cam.orthographicSize * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // מיקום רצוי של המצלמה (רק בציר ה-X)
        float desiredX = target.position.x;

        // מגביל את המיקום בתוך גבולות הקירות
        float minX = wallLeft.position.x + cameraHalfWidth;
        float maxX = wallRight.position.x - cameraHalfWidth;
        float clampedX = Mathf.Clamp(desiredX, minX, maxX);

        // תזוזה חלקה של המצלמה
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(clampedX, transform.position.y, transform.position.z), smoothSpeed);
        transform.position = smoothedPosition;
    }
}
