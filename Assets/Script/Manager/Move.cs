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

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Force Reboot Head Bob")]
    [SerializeField] private float bobVerticalAmplitude = 0.04f;
    [SerializeField] private float bobHorizontalAmplitude = 0.02f;
    [SerializeField] private float bobTiltAngle = 1.5f;
    [SerializeField] private float bobFrequency = 12f;
    [SerializeField] private float landPunchAmplitude = 0.15f;
    [SerializeField] private float landPunchSmoothing = 15f;

    public HP_Slider hp;
    private CharacterController controller;
    private Vector3 moveVelocity;
    private float verticalVelocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isRunning;
    
    private float bobTimer;
    private float currentLandPunch;
    private bool wasGrounded;
    [SerializeField] Camera playerCamera;
    private CameraRot camRotComp;
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
        if (playerCamera != null) camRotComp = playerCamera.GetComponent<CameraRot>();
        
        if (controller != null) 
        {
            controller.stepOffset = 0.2f; 
        }

        targetHeight = standHeight;
        controller.height = standHeight;
        
        transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        originalCameraY = playerCamera.transform.localPosition.y;
        currentCameraY = originalCameraY; 
        
        UpdateControllerCenter();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy") || hit.gameObject.CompareTag("suicide") || hit.gameObject.CompareTag("BuildingEnemy"))
        {
            if (verticalVelocity > 0) verticalVelocity = 0;

            if (hit.normal.y > 0.7f)
            {
                verticalVelocity = -5f;
            }
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

        ApplyForceRebootHeadBob();
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
        // Jump logic can be added here if needed
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

    void ApplyForceRebootHeadBob()
    {
        if (playerCamera == null) return;

        // 1. Landing Punch detection
        if (isGrounded && !wasGrounded)
        {
            currentLandPunch = landPunchAmplitude;
        }
        wasGrounded = isGrounded;

        // Recover from land punch
        currentLandPunch = Mathf.Lerp(currentLandPunch, 0f, Time.deltaTime * landPunchSmoothing);

        // 2. Velocity-based Head Bob
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        float speed = horizontalVelocity.magnitude;
        float hInput = Input.GetAxisRaw("Horizontal");

        Vector3 targetPos = new Vector3(0, currentCameraY - currentLandPunch, 0);
        Quaternion targetRot = Quaternion.identity;

        if (isGrounded && speed > 0.1f)
        {
            float freq = isRunning ? bobFrequency * 1.3f : bobFrequency;
            bobTimer += Time.deltaTime * freq;

            float bobY = Mathf.Sin(bobTimer) * bobVerticalAmplitude;
            float bobX = Mathf.Cos(bobTimer * 0.5f) * bobHorizontalAmplitude;
            
            targetPos += new Vector3(bobX, bobY, 0);

            float tilt = -hInput * bobTiltAngle;
            tilt += Mathf.Sin(bobTimer * 0.5f) * (bobTiltAngle * 0.5f);
            targetRot = Quaternion.Euler(0, 0, tilt);
        }
        else
        {
            bobTimer = Mathf.Lerp(bobTimer, 0, Time.deltaTime * 5f);
        }

        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPos, Time.deltaTime * 10f);

        if (camRotComp != null)
        {
            camRotComp.proceduralRotation = Quaternion.Slerp(camRotComp.proceduralRotation, targetRot, Time.deltaTime * 10f);
        }
    }

    public bool IsGrounded => isGrounded;

    public void ApplyStagger(Vector3 force)
    {
        moveVelocity += force;
    }
}
