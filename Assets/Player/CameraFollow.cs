using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // ����� ������� ����� �����
    public float smoothSpeed = 5f; // ������ ������ �� ������
    public Vector3 offset = new Vector3(0f, 2f, -10f); // ����� ���� ��� ������ �����
    public float zoomSize = 5f; // ��� ���� �� ������

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographicSize = zoomSize; // ����� ���� ����
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // ���� ���� ����� �� ����� ����
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
