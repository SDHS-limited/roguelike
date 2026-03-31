using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRot : MonoBehaviour
{
    [SerializeField] Transform playerBody;
    [SerializeField] Transform arm;

    [Header("Settings")]
    [SerializeField] float mouseSensitivity = 100f;
    [SerializeField] float smoothTime = 0.02f;   // 0 => 즉시, 큰 값 => 더 느리게 따라감
    [SerializeField] bool invertY = false;
    
    // 💡 1. 상하 제한을 레포데처럼 완벽한 90도로 변경했습니다.
    [SerializeField] float minPitch = -90f; // 아래로 내리는 한계
    [SerializeField] float maxPitch = 90f;  // 위로 올리는 한계
    [SerializeField] bool lockCursor = true;

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

        // 💡 2. 게임 시작 시 카메라가 꺾여있을 경우 각도 계산이 꼬이는 것을 방지합니다. (360도 체계를 -180~180도로 변환)
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
        // 💡 3. UI가 열려있을 때 마우스 입력을 아예 막도록 순서를 위로 올렸습니다.
        if (isUIOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return; 
        }

        // ESC로 커서 해제 토글 (선택)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 화면 클릭 시 커서 다시 잠금
        if (Input.GetMouseButtonDown(0) && lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 💡 4. 커서가 잠겨 있을 때(즉, 게임 플레이 중일 때만) 시야가 회전하도록 안전장치를 추가했습니다.
        //if (Cursor.lockState == CursorLockMode.Locked)
        //{
            // 마우스 입력 
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

            // invert Y 옵션
            if (invertY) mouseY = -mouseY;

            // 원하는 회전값 누적
            yaw += mouseX;
            // pitch -= mouseY;

            // 상하 각도 클램핑 (-90도 ~ 90도)
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // 부드럽게 보간 (SmoothDamp)
            Vector2 targetRotation = new Vector2(pitch, yaw);
            currentRotation = Vector2.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, smoothTime);

            // 💡 5. 주석 처리되어 있던 카메라 상하 회전 코드를 활성화했습니다!
            transform.localEulerAngles = new Vector3(currentRotation.x, 0f, 0f); // 카메라는 상하(Pitch)만 움직임
            playerBody.eulerAngles = new Vector3(0f, currentRotation.y, 0f);     // 플레이어 몸통은 좌우(Yaw)만 움직임
            
            // arm.localEulerAngles = new Vector3(currentRotation.x, 0f, 0f);
        //}
    }
}
