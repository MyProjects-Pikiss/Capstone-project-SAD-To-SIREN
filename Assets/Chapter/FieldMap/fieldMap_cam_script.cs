using UnityEngine;

public class fieldMap_cam_script : MonoBehaviour
{
    public float dragSpeed = 3f;
    private Vector3 dragOrigin;

    public static Vector3 CameraMoveDelta { get; private set; } = Vector3.zero; // 카메라의 이동량을 저장하는 변수

    void LateUpdate()
    {
        CameraMoveDelta = Vector3.zero; // 매 프레임 시작 시 카메라의 이동량 초기화

        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0f);

        CameraMoveDelta = move; // 카메라의 이동량 업데이트

        transform.Translate(move, Space.World);
    }
}