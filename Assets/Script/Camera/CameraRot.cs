using UnityEngine;

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

    public Quaternion proceduralRotation = Quaternion.identity;

    void Update()
    {
        // UI 열려있으면 커서 해제 후 카메라 입력 차단
        if (isUIOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        // ESC 커서 해제/잠금 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = (Cursor.lockState != CursorLockMode.Locked);
        }

        // 클릭 시 커서 재잠금
        if (Input.GetMouseButtonDown(0) && lockCursor && !isUIOpen)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // ✅ 커서가 잠겨있을 때만 카메라 회전
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // GetAxisRaw를 사용하여 더 즉각적인 반응 제공
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * 0.02f;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * 0.02f;

        if (invertY) mouseY = -mouseY;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 부드럽게 보간 (smoothTime이 0이면 즉시 회전)
        if (smoothTime > 0)
        {
            Vector2 targetRotation = new Vector2(pitch, yaw);
            currentRotation = Vector2.SmoothDamp(
                currentRotation, targetRotation,
                ref rotationVelocity, smoothTime
            );
        }
        else
        {
            currentRotation = new Vector2(pitch, yaw);
        }

        // ✅ 카메라는 상하(Pitch) 회전 + 절차적 회전(Procedural Rotation) 적용
        transform.localRotation = Quaternion.Euler(currentRotation.x, 0f, 0f) * proceduralRotation;
        
        // ✅ 몸통은 좌우(Yaw) 회전
        playerBody.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);

        // ✅ 팔이 카메라의 자식이 아니라면 여기서 회전시켜줌 (자식이라면 불필요)
        if (arm != null && arm.parent != transform)
        {
            arm.localRotation = transform.localRotation;
        }
    }
}
