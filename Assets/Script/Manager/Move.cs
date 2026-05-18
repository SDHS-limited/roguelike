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

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

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
    private float currentCameraY;

    private float dashTimeLeft;
    private float dashCooldownTimer;
    private bool isDashing;
    private Vector3 dashDirection;

    private readonly float yOffset = 0.4f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        
        // Step Offset을 낮춰 적이나 낮은 장애물을 밟고 올라가는 현상 방지
        if (controller != null) 
        {
            controller.stepOffset = 0.2f; // 더 낮춤 (기본값 0.3보다 낮게)
        }

        targetHeight = standHeight;
        controller.height = standHeight;
        
        transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        originalCameraY = playerCamera.transform.localPosition.y;
        currentCameraY = originalCameraY; 
        
        UpdateControllerCenter();
    }

    // 적과 부딪혔을 때 위로 솟구치는 현상 방지
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 충돌 지점이 플레이어의 발 근처가 아니라면 (즉, 옆면이나 머리쪽 충돌)
        // CharacterController가 위로 밀어내려는 성질을 억제합니다.
        if (hit.gameObject.CompareTag("Enemy") || hit.gameObject.CompareTag("suicide") || hit.gameObject.CompareTag("BuildingEnemy"))
        {
            // 위쪽으로의 이동속도가 있다면 즉시 제거
            if (verticalVelocity > 0) 
            {
                verticalVelocity = 0;
            }

            // 충돌 법선(Normal)의 Y값이 클수록 플레이어를 위로 밀어내려 한다는 뜻입니다.
            // 법선의 Y가 0.7 이상인 경우(경사가 급함) 수직 속도를 하향 조정
            if (hit.normal.y > 0.7f)
            {
                verticalVelocity = -5f; // 강제로 바닥으로 누름
            }
            
            // 충돌 지점이 캐릭터 하단 절반보다 위라면 옆으로 밀려나게만 함
            Vector3 pushDir = new Vector3(hit.normal.x, 0, hit.normal.z).normalized;
            // 여기에 밀쳐내는 힘을 추가할 수도 있습니다.
        }
    }

    void Update()
    {
        CheckGround();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        HandleCrouch(); 
        HandleDash(); 
        
        Vector3 finalMove;
        if (isDashing)
        {
            finalMove = dashDirection * dashSpeed;
        }
        else
        {
            finalMove = moveVelocity + Vector3.up * verticalVelocity;
        }

        controller.Move(finalMove * Time.deltaTime);

        //ApplyHeadBob();
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

        isRunning = Input.GetKey(KeyCode.LeftShift) && v > 0.1f && !isCrouching && !isDashing;

        float targetSpeed = isCrouching ? crouchSpeed : isRunning ? runSpeed : walkSpeed;
        Vector3 targetVelocity = inputDir * targetSpeed;

        float accel = isGrounded ? 
                     (inputDir.magnitude > 0 ? acceleration : deceleration) : 
                     acceleration * airControl;

        moveVelocity = Vector3.Lerp(moveVelocity, targetVelocity, accel * Time.deltaTime);
    }

    void HandleDash()
    {
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0 && !isDashing && !isCrouching)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            
            if (h == 0 && v == 0)
            {
                dashDirection = transform.forward;
            }
            else
            {
                dashDirection = (transform.right * h + transform.forward * v).normalized;
            }

            isDashing = true;
            dashTimeLeft = dashDuration;
            dashCooldownTimer = dashCooldown;

            Effect effects = FindFirstObjectByType<Effect>();
            if (effects != null) effects.TriggerCameraShake(0.1f, 0.05f);
        }

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
            }
        }
    }

    void HandleJump()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        //{
        //    verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
        //}
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

        float targetCamY = isCrouching ? originalCameraY - (standHeight - crouchHeight) * 0.5f : originalCameraY;
        currentCameraY = Mathf.Lerp(currentCameraY, targetCamY, crouchTransitionSpeed * Time.deltaTime);
    }

    void UpdateControllerCenter()
    {
        controller.center = new Vector3(0, (controller.height / 2f) - yOffset, 0);
    }

    void ApplyHeadBob()
    {
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

        playerCamera.transform.localPosition = new Vector3(bobOffset.x, currentCameraY + bobOffset.y, playerCamera.transform.localPosition.z);
    }

    public bool IsGrounded => isGrounded;

    public void ApplyStagger(Vector3 force)
    {
        moveVelocity += force;
    }
}
