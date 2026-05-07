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

    [Header("Bob")]
    public float bobFrequency = 2.2f;
    public float bobAmplitude = 0.05f;

    public HP_Slider hp;
    private CharacterController controller;
    private Vector3 moveVelocity;
    private float verticalVelocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isRunning;
    private float bobTimer;
    [SerializeField] Camera playerCamera;
    private float targetHeight;
    private float originalCameraY;

    // 추가: 바닥에서 띄울 높이 오프셋 (요청하신 0.17 반영)
    private readonly float yOffset = 0.4f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        
        // 초기 높이 설정 (standHeight로 시작)
        targetHeight = standHeight;
        controller.height = standHeight;
        
        // 초기 위치 강제 설정
        transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        
        originalCameraY = playerCamera.transform.localPosition.y;
        
        // 초기 Center 설정
        UpdateControllerCenter();
    }

    void Update()
    {
        CheckGround();
        HandleCrouch();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        ApplyHeadBob();

        // 최종 이동 적용
        controller.Move((moveVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
    }

    void CheckGround()
    {
        // 0.17 위치에서 아래로 살짝 내린 지점에서 체크
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
            // 머리 위 체크
            if (!Physics.SphereCast(transform.position + Vector3.up * yOffset, 0.4f, Vector3.up, out _, standHeight - crouchHeight))    
                isCrouching = false;
        }

        targetHeight = isCrouching ? crouchHeight : standHeight;

        // 높이 부드럽게 전환
        controller.height = Mathf.Lerp(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        
        // 중요: 높이가 변할 때마다 Center도 같이 업데이트
        UpdateControllerCenter();

        // 카메라 이동
        float targetCamY = isCrouching ? originalCameraY - (standHeight - crouchHeight) * 0.5f : originalCameraY;
        Vector3 camPos = playerCamera.transform.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCamY, crouchTransitionSpeed * Time.deltaTime);
        playerCamera.transform.localPosition = camPos;
    }

    // CharacterController의 중심점을 보정하는 함수
    void UpdateControllerCenter()
    {
        // Pivot이 중앙에 있다면: Center.y = (Height / 2) - Offset을 해주어야 
        // 캐릭터의 Transform 위치가 항상 바닥에서 Offset(0.17)만큼 뜬 상태가 됩니다.
        controller.center = new Vector3(0, (controller.height / 2f) - yOffset, 0);
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
        else { bobTimer = 0; }
    }

    public bool IsGrounded => isGrounded;
   

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Enemy"))
    //    {
    //        hp.curHP -= 10 ;
    //    }
    //}
}
