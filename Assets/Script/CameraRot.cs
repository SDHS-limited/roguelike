using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRot : MonoBehaviour
{
    public Transform playerBody;

    [Header("Settings")]
    public float mouseSensitivity = 100f;
    public float smoothTime = 0.02f;   // 0 => 즉시, 큰 값 => 더 느리게 따라감
    public bool invertY = false;
    public float minPitch = -80f; // 아래로 내리는 한계
    public float maxPitch = 80f;  // 위로 올리는 한계
    public bool lockCursor = true;

    float pitch = 0f; // 카메라 상하 회전(음수 = 아래, 양수 = 위)
    float yaw = 0f;   // 플레이어 좌우 회전
    Vector2 currentRotation;      // 현재 (pitch, yaw)
    Vector2 rotationVelocity;     // 부드러움 계산용
    public bool isUIOpen = false;

    void Start()
    {
        if (playerBody == null)
        {
            Debug.LogError("MouseLook: playerBody가 할당되지 않았습니다. 플레이어 Transform을 할당하세요.");
            enabled = false;
            return;
        }

        // 초기값
        Vector3 euler = transform.localEulerAngles;
        pitch = euler.x;
        yaw = playerBody.eulerAngles.y;
        currentRotation = new Vector2(pitch, yaw);

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // 마우스 입력 (GetAxis는 "Mouse X", "Mouse Y" 기본 매핑)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // invert Y 옵션
        if (invertY) mouseY = -mouseY;

        // 원하는 회전값 누적
        yaw += mouseX;
        pitch -= mouseY; // 마우스를 위로 올리면 pitch 감소(보통 그렇게 구현)

        // 상하 각도 클램핑 (pitch)
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 부드럽게 보간 (SmoothDamp)
        Vector2 targetRotation = new Vector2(pitch, yaw);
        currentRotation = Vector2.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, smoothTime);

        // 적용
        transform.localEulerAngles = new Vector3(currentRotation.x, 0f, 0f); // 카메라는 pitch만
        playerBody.eulerAngles = new Vector3(0f, currentRotation.y, 0f);    // 플레이어 바디는 yaw만

        if (isUIOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return; 
        }

        // ESC로 커서 해제 토글(선택)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonDown(0) && lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
