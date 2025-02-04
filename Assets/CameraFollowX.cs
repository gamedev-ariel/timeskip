using UnityEngine;

public class CameraFollowX : MonoBehaviour
{
    public Transform target; // ����� ������� ����� �����
    public Transform wallLeft; // ���� ������ ������ �� ������
    public Transform wallRight; // ���� ����� ������ �� ������
    public float smoothSpeed = 0.125f; // ���� ������ �� ������

    private float cameraHalfWidth;

    void Start()
    {
        // ���� �� ��� ����� �� ������
        Camera cam = Camera.main;
        cameraHalfWidth = cam.orthographicSize * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ����� ���� �� ������ (�� ���� �-X)
        float desiredX = target.position.x;

        // ����� �� ������ ���� ������ ������
        float minX = wallLeft.position.x + cameraHalfWidth;
        float maxX = wallRight.position.x - cameraHalfWidth;
        float clampedX = Mathf.Clamp(desiredX, minX, maxX);

        // ����� ���� �� ������
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(clampedX, transform.position.y, transform.position.z), smoothSpeed);
        transform.position = smoothedPosition;
    }
}
