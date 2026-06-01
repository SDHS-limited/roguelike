using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] ExperimentManager experimentManager;
    [SerializeField] Effect effect;
    [SerializeField] HP_Slider hp;
    [SerializeField] GameObject suicide;
    [SerializeField] public float attackPowerMultiplier = 1.0f;
    [SerializeField] public float speed = 5.0f;

    [Header("Dash")]
    [SerializeField] float dashDistance = 5f;
    [SerializeField] float dashCooldownW = 1f;
    [SerializeField] float dashCooldownA = 1f;
    [SerializeField] float dashCooldownS = 1f;
    [SerializeField] float dashCooldownD = 1f;
    [SerializeField] LayerMask dashBlockLayers = ~0;
    [SerializeField] float dashStopOffset = 0.1f;
    [SerializeField] float dashDoubleTapWindow = 0.25f;
    float dashCooldownTimerW;
    float dashCooldownTimerA;
    float dashCooldownTimerS;
    float dashCooldownTimerD;
    bool isDashing = false;
    float lastTapW = -1f;
    float lastTapA = -1f;
    float lastTapS = -1f;
    float lastTapD = -1f;

    [Header("Head Bob")]
    [SerializeField] Transform cameraHolder;
    [SerializeField] float bobSpeed = 10f;
    [SerializeField] float bobAmount = 0.05f;
    [SerializeField] float tiltAmount = 3f;

    float bobTimer;
    Vector3 camOriginPos;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camOriginPos = cameraHolder.localPosition;
    }

    void Update()
    {
        // move();

        //dashCooldownTimerW -= Time.deltaTime;
        //dashCooldownTimerA -= Time.deltaTime;
        //dashCooldownTimerS -= Time.deltaTime;
        //dashCooldownTimerD -= Time.deltaTime;

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    if (Time.time - lastTapW <= dashDoubleTapWindow)
        //    {
        //        dash(transform.forward, KeyCode.W);
        //    }
        //    lastTapW = Time.time;
        //}

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    if (Time.time - lastTapA <= dashDoubleTapWindow)
        //    {
        //        dash(-transform.right, KeyCode.A);
        //    }
        //    lastTapA = Time.time;
        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    if (Time.time - lastTapS <= dashDoubleTapWindow)
        //    {
        //        dash(-transform.forward, KeyCode.S);
        //    }
        //    lastTapS = Time.time;
        //}

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    if (Time.time - lastTapD <= dashDoubleTapWindow)
        //    {
        //        dash(transform.right, KeyCode.D);
        //    }
        //    lastTapD = Time.time;
        //}
    }

    void move()
    {
        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(moveH, 0, moveV);

        transform.Translate(move * speed * Time.deltaTime);

        // HandleHeadBob(move);
    }

    //void HandleHeadBob(Vector3 moveInput)
    //{
    //    bool isMoving = moveInput.magnitude > 0.1f && !isDashing;

    //    if (isMoving)
    //    {
    //        bobTimer += Time.deltaTime * bobSpeed;

    //        float intensity = moveInput.magnitude;

    //        // 🔥 좌우 중심
    //        float bobX = Mathf.Sin(bobTimer) * bobAmount * 1.5f;
    //        bobX = Mathf.Sign(bobX) * Mathf.Pow(Mathf.Abs(bobX), 0.5f);

    //        // 🔥 위아래 최소화
    //        float bobY = Mathf.Sin(bobTimer * 2) * bobAmount * 0.3f;

    //        // 🔥 기울기
    //        float tilt = Mathf.Sin(bobTimer) * 4f;

    //        cameraHolder.localPosition = camOriginPos + new Vector3(bobX * intensity, bobY * intensity, 0);
    //        cameraHolder.localRotation = Quaternion.Euler(0, 0, tilt * intensity);
    //    }
    //    else
    //    {
    //        bobTimer = 0;

    //        cameraHolder.localPosition = Vector3.Lerp(
    //            cameraHolder.localPosition,
    //            camOriginPos,
    //            Time.deltaTime * 6f
    //        );

    //        cameraHolder.localRotation = Quaternion.Lerp(
    //            cameraHolder.localRotation,
    //            Quaternion.identity,
    //            Time.deltaTime * 6f
    //        );
    //    }
    //}

    //void dash(Vector3 inputDir, KeyCode dashKey)
    //{
    //    if (isDashing) return;
    //    if (dashKey == KeyCode.W && dashCooldownTimerW > 0f) return;
    //    if (dashKey == KeyCode.A && dashCooldownTimerA > 0f) return;
    //    if (dashKey == KeyCode.S && dashCooldownTimerS > 0f) return;
    //    if (dashKey == KeyCode.D && dashCooldownTimerD > 0f) return;

    //    Vector3 dir = inputDir;
    //    dir.y = 0f;

    //    if (dir.sqrMagnitude < 0.01f)
    //        dir = transform.forward;
    //    else
    //        dir.Normalize();

    //    float actualDistance = dashDistance;

    //    float rayStartOffset = 0.2f;
    //    Vector3 rayOrigin = transform.position + dir * rayStartOffset;
    //    float rayLength = Mathf.Max(0.01f, dashDistance - rayStartOffset);

    //    if (Physics.Raycast(rayOrigin, dir, out RaycastHit hit, rayLength, dashBlockLayers))
    //    {
    //        actualDistance = rayStartOffset + Mathf.Max(0f, hit.distance - dashStopOffset);
    //    }

    //    StartCoroutine(DashCoroutine(dir, actualDistance));
    //    if (dashKey == KeyCode.W) dashCooldownTimerW = dashCooldownW;
    //    else if (dashKey == KeyCode.A) dashCooldownTimerA = dashCooldownA;
    //    else if (dashKey == KeyCode.S) dashCooldownTimerS = dashCooldownS;
    //    else if (dashKey == KeyCode.D) dashCooldownTimerD = dashCooldownD;
    //}

    IEnumerator DashCoroutine(Vector3 dir, float distance)
    {
        isDashing = true;

        float dashDuration = 0.2f;
        float elapsed = 0f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + dir * distance;

        // 👉 대쉬 시작 시 카메라 살짝 눌림
        cameraHolder.localPosition += Vector3.down * 0.1f;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dashDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;

        // 👉 카메라 원위치 복구
        cameraHolder.localPosition = camOriginPos;

        isDashing = false;
    }
    public void TakeDamage(float amount)
    {
        if (hp != null) hp.TakeDamage(amount);
        Debug.Log("응애");
        
        Fever_Slider fever = FindFirstObjectByType<Fever_Slider>();
        if (fever != null)
        {
            fever.AddFever(amount * 0.5f);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
        if (hit.gameObject.CompareTag("suicide"))
        {
            //폭발 파티클 추가
            Destroy(suicide);
            TakeDamage(10);
        }
    }
}