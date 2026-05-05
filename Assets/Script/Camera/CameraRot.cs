using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRot : MonoBehaviour
{
       [SerializeField] Transform playerBody;
    [SerializeField] Transform arm;

    [Header("Settings")]
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float smoothTime = 0.02f;
    [SerializeField] bool invertY = false;
    [SerializeField] float minPitch = -90f;
    [SerializeField] float maxPitch = 90f;
    [SerializeField] bool lockCursor = true;

    float pitch = 0f;
    float yaw = 0f;

    Vector2 currentRotation;
    Vector2 rotationVelocity;

    public bool isUIOpen = false;

    void Start()
    {
        if (playerBody == null)
        {
            Debug.LogError("CameraRot: playerBody가 할당되지 않았습니다.");
            enabled = false;
            return;
        }

        // 시작 시 현재 각도 동기화
        Vector3 euler = transform.localEulerAngles;
        pitch = euler.x > 180 ? euler.x - 360 : euler.x;
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
        // UI 열려있으면 커서 해제 후 카메라 입력 차단
        if (isUIOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        // ESC 커서 해제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 클릭 시 커서 재잠금
        if (Input.GetMouseButtonDown(0) && lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // ✅ 커서가 잠겨있을 때만 카메라 회전
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (invertY) mouseY = -mouseY;

        yaw   += mouseX;
        pitch -= mouseY;  // ✅ 주석 해제: 상하 회전 활성화

        // ✅ 업데이트 이후에 Clamp (순서 수정)
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 부드럽게 보간
        Vector2 targetRotation = new Vector2(pitch, yaw);
        currentRotation = Vector2.SmoothDamp(
            currentRotation, targetRotation,
            ref rotationVelocity, smoothTime
        );

        // 카메라는 상하(Pitch), 몸통은 좌우(Yaw)
        transform.localEulerAngles   = new Vector3(currentRotation.x, 0f, 0f);
        playerBody.eulerAngles       = new Vector3(0f, currentRotation.y, 0f);

        // 팔도 카메라와 같이 회전시키려면 아래 주석 해제
        // arm.localEulerAngles = new Vector3(currentRotation.x, 0f, 0f);
    }
}
