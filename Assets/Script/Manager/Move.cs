using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Move : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4.5f;
    public float runSpeed = 7.5f;
    public float crouchSpeed = 2.5f;
    public float acceleration = 10f;
    public float deceleration = 12f;
    public float airControl = 0.3f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -20f;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [Header("Crouch")]
    public float crouchHeight = 1.0f;
    public float standHeight = 2.0f;
    public float crouchTransitionSpeed = 8f;

    [Header("Bob Settings")]
    public float bobFrequency = 5f;
    public float bobAmplitude = 0.05f;
    public float bobHorizontalAmplitude = 0.02f;
    public float bobSmoothing = 10f;

    public HP_Slider hp;
    private CharacterController controller;
    private Vector3 moveVelocity;
    private float verticalVelocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isRunning;
    private float bobTimer;
    private Vector3 bobOffset;
    [SerializeField] Camera playerCamera;
    private float targetHeight;
    private float originalCameraY;

    // 추가: 바닥에서 띄울 높이 오프셋 (요청하신 0.4 반영)
    private readonly float yOffset = 0.4f;

    private float currentCameraY;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        
        targetHeight = standHeight;
        controller.height = standHeight;
        
        transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        originalCameraY = playerCamera.transform.localPosition.y;
        currentCameraY = originalCameraY; // 초기화 추가
        
        UpdateControllerCenter();
    }

    void Update()
    {
        CheckGround();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        HandleCrouch(); 
        
        // 최종 이동 적용
        controller.Move((moveVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);

        // 이동 후 헤드봅 적용
        // ApplyHeadBob();
    }

    void CheckGround()
    {
        Vector3 spherePos = transform.position + Vector3.up * (yOffset - groundCheckRadius);
        isGrounded = Physics.CheckSphere(spherePos, groundCheckRadius, groundLayer);

        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f; 
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = transform.right * h + transform.forward * v;
        if (inputDir.magnitude > 1f) inputDir.Normalize();

        isRunning = Input.GetKey(KeyCode.LeftShift) && v > 0.1f && !isCrouching;

        float targetSpeed = isCrouching ? crouchSpeed : isRunning ? runSpeed : walkSpeed;
        Vector3 targetVelocity = inputDir * targetSpeed;

        float accel = isGrounded ? 
                     (inputDir.magnitude > 0 ? acceleration : deceleration) : 
                     acceleration * airControl;

        moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, accel * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
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
        if (Input.GetKeyDown(KeyCode.LeftControl)) isCrouching = true;
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (!Physics.SphereCast(transform.position + Vector3.up * yOffset, 0.4f, Vector3.up, out _, standHeight - crouchHeight))    
                isCrouching = false;
        }

        targetHeight = isCrouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        
        UpdateControllerCenter();

        // 카메라의 '기본 높이'만 계산 (실제 적용은 ApplyHeadBob에서 수행)
        float targetCamY = isCrouching ? originalCameraY - (standHeight - crouchHeight) * 0.5f : originalCameraY;
        currentCameraY = Mathf.Lerp(currentCameraY, targetCamY, crouchTransitionSpeed * Time.deltaTime);
    }

    void UpdateControllerCenter()
    {
        controller.center = new Vector3(0, (controller.height / 2f) - yOffset, 0);
    }

    void ApplyHeadBob()
    {
        // 물리적 속도와 입력값을 동시에 체크하여 더 정확하게 움직임 감지
        float speed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
        bool isMovingInput = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1f;
        
        if (isGrounded && (speed > 0.1f || isMovingInput))
        {
            float speedMultiplier = isRunning ? 1.4f : 1f;
            bobTimer += Time.deltaTime * bobFrequency * speedMultiplier;

            float targetY = Mathf.Sin(bobTimer) * bobAmplitude;
            float targetX = Mathf.Cos(bobTimer * 0.5f) * bobHorizontalAmplitude;
            
            bobOffset = Vector3.Lerp(bobOffset, new Vector3(targetX, targetY, 0), Time.deltaTime * bobSmoothing);
        }
        else
        {
            bobTimer = 0;
            bobOffset = Vector3.Lerp(bobOffset, Vector3.zero, Time.deltaTime * bobSmoothing);
        }

        // 최종 적용: 계산된 기본 높이(currentCameraY) + 흔들림 오프셋(bobOffset)
        playerCamera.transform.localPosition = new Vector3(bobOffset.x, currentCameraY + bobOffset.y, playerCamera.transform.localPosition.z);
    }

    public bool IsGrounded => isGrounded;

    public void ApplyStagger(Vector3 force)
    {
        moveVelocity += force;
    }
}
