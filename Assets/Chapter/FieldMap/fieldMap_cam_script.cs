using UnityEngine;

public class fieldMap_cam_script : MonoBehaviour
{
    public float dragSpeed = 3f;
    private Vector3 dragOrigin;

    public static Vector3 CameraMoveDelta { get; private set; } = Vector3.zero; // ī�޶��� �̵����� �����ϴ� ����

    void LateUpdate()
    {
        CameraMoveDelta = Vector3.zero; // �� ������ ���� �� ī�޶��� �̵��� �ʱ�ȭ

        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0f);

        CameraMoveDelta = move; // ī�޶��� �̵��� ������Ʈ

        transform.Translate(move, Space.World);
    }
}