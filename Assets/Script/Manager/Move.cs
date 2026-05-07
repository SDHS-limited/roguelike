using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class Move : MonoBehaviour
{
[Header("Movement")]
    public float walkSpeed = 4.5f;         // L4D2 기본 이동속도
    public float runSpeed = 7.5f;
    public float crouchSpeed = 2.5f;
    public float acceleration = 10f;       // 가속도
    public float deceleration = 12f;       // 감속도 (더 빠르게)
    public float airControl = 0.3f;        // 공중 제어력

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -20f;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Crouch")]
    public float crouchHeight = 1.0f;
    public float standHeight = 2.0f;
    public float crouchTransitionSpeed = 8f;

    [Header("Bob")]
    public float bobFrequency = 2.2f;
    public float bobAmplitude = 0.05f;

    // 내부 변수
    private CharacterController controller;
    private Vector3 velocity;              // 현재 속도 (관성)
    private Vector3 moveVelocity;          // 수평 이동 속도
    private float verticalVelocity;        // 수직 속도 (중력/점프)
    private bool isGrounded;
    private bool isCrouching;
    private bool isRunning;
    private float bobTimer;
    private Camera playerCamera;
    private float targetHeight;
    private float originalCameraY;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        originalCameraY = playerCamera.transform.localPosition.y;
        targetHeight = standHeight;
    }

    void Update()
    {
        CheckGround();
        // HandleCrouch();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        ApplyHeadBob();

        // 최종 이동 적용
        controller.Move((moveVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    void CheckGround()
    {
        // 발 위치에서 구체 체크
        Vector3 spherePos = transform.position + Vector3.down * (controller.height / 2 - groundCheckRadius);
        isGrounded = Physics.CheckSphere(spherePos, groundCheckRadius, groundLayer);

        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f; // 바닥에 붙어있게
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 카메라 방향 기준으로 이동 방향 계산
        Vector3 inputDir = transform.right * h + transform.forward * v;
        if (inputDir.magnitude > 1f)
            inputDir.Normalize();

        // 달리기 판단 (앞으로 이동 + Shift, 앉지 않은 상태)
        isRunning = Input.GetKey(KeyCode.LeftShift) && v > 0.1f && !isCrouching;

        float targetSpeed = isCrouching ? crouchSpeed :
                           isRunning ? runSpeed : walkSpeed;

        Vector3 targetVelocity = inputDir * targetSpeed;

        // 공중 / 지상 제어력 다르게
        float accel = isGrounded ? 
                     (inputDir.magnitude > 0 ? acceleration : deceleration) : 
                     acceleration * airControl;

        moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, accel * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // v² = 2gh  →  v = sqrt(2 * |gravity| * jumpHeight)
            verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded)
            verticalVelocity += gravity * Time.deltaTime;
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            isCrouching = true;
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            // 머리 위 공간 체크 후 일어서기
            if (!Physics.SphereCast(transform.position, 0.4f, Vector3.up, out _, standHeight - crouchHeight))
                isCrouching = false;
        }

        targetHeight = isCrouching ? crouchHeight : standHeight;

        // 높이 부드럽게 전환
        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        controller.center = Vector3.up * controller.height / 2;

        // 카메라도 같이 이동
        float targetCamY = isCrouching ? originalCameraY - (standHeight - crouchHeight) * 0.5f : originalCameraY;
        Vector3 camPos = playerCamera.transform.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCamY, crouchTransitionSpeed * Time.deltaTime);
        playerCamera.transform.localPosition = camPos;
    }

    void ApplyHeadBob()
    {
        if (!isGrounded) return;

        float speed = moveVelocity.magnitude;
        if (speed > 0.1f)
        {
            bobTimer += Time.deltaTime * bobFrequency * (isRunning ? 1.5f : 1f);
            float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude * (speed / walkSpeed);

            Vector3 camPos = playerCamera.transform.localPosition;
            camPos.y += bobOffset;
            playerCamera.transform.localPosition = camPos;
        }
        else
        {
            bobTimer = 0;
        }
    }

    // 외부에서 읽을 수 있는 프로퍼티
    public bool IsGrounded => isGrounded;
    public bool IsRunning => isRunning;
    public bool IsCrouching => isCrouching;
    public Vector3 MoveVelocity => moveVelocity;
}
