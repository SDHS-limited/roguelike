using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [Header("걷기 설정")]
    public float walkBobSpeed = 14f;
    public float walkBobAmountY = 0.05f;
    public float walkBobAmountX = 0.025f;

    [Header("달리기 설정")]
    public float runBobSpeed = 20f;
    public float runBobAmountY = 0.10f;
    public float runBobAmountX = 0.05f;

    [Header("복귀 / 전환 설정")]
    public float returnSpeed = 8f;       // 멈출 때 원위치 복귀 속도
    public float transitionSpeed = 5f;   // 걷기↔달리기 전환 속도

    [Header("참조")]
    public Transform cameraTransform;

    // ── 내부 상태 ──────────────────────────────────
    private float defaultPosY;
    private float defaultPosX;
    private float timer;

    // 현재 실제로 적용 중인 파라미터 (Lerp 대상)
    private float currentSpeed;
    private float currentAmountY;
    private float currentAmountX;

    void Start()
    {
        defaultPosY = cameraTransform.localPosition.y;
        defaultPosX = cameraTransform.localPosition.x;

        // 초기값 = 걷기 파라미터
        currentSpeed = walkBobSpeed;
        currentAmountY = walkBobAmountY;
        currentAmountX = walkBobAmountX;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift);

        // ── 목표 파라미터 결정 ──────────────────────
        float targetSpeed = isRunning ? runBobSpeed : walkBobSpeed;
        float targetAmountY = isRunning ? runBobAmountY : walkBobAmountY;
        float targetAmountX = isRunning ? runBobAmountX : walkBobAmountX;

        // 걷기 ↔ 달리기 부드럽게 Lerp
        float t = Time.deltaTime * transitionSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, t);
        currentAmountY = Mathf.Lerp(currentAmountY, targetAmountY, t);
        currentAmountX = Mathf.Lerp(currentAmountX, targetAmountX, t);

        if (isMoving)
        {
            // ── 타이머 진행 ──────────────────────────
            timer += Time.deltaTime * currentSpeed;

            // Y : sin(t)          → 위아래 (기본 주파수)
            // X : sin(t * 0.5)    → 좌우   (절반 주파수 = 8자 궤적)
            float bobY = defaultPosY + Mathf.Sin(timer) * currentAmountY;
            float bobX = defaultPosX + Mathf.Sin(timer * 0.5f) * currentAmountX;

            // 카메라 위치 바로 적용 (이미 Lerp된 파라미터이므로 자연스러움)
            cameraTransform.localPosition = new Vector3(
                bobX,
                bobY,
                cameraTransform.localPosition.z
            );
        }
        else
        {
            // ── 멈췄을 때: 타이머 0 리셋 없이 원위치로 Lerp ──
            // timer를 즉시 0으로 하면 카메라가 튀므로 페이드아웃
            timer = Mathf.Lerp(timer, 0f, Time.deltaTime * returnSpeed);

            Vector3 targetPos = new Vector3(
                defaultPosX,
                defaultPosY,
                cameraTransform.localPosition.z
            );
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                targetPos,
                Time.deltaTime * returnSpeed
            );
        }
    }
}