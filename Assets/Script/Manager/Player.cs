using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] ExperimentManager experimentManager;
    [SerializeField] Effect effect;
    [SerializeField] HP_Slider hp;
    [SerializeField] public float damage = 10f;
    [SerializeField] public float speed = 5.0f;

    [Header("Dash")]
    [SerializeField] float dashDistance = 5f;
    [SerializeField] float dashCooldown = 1f;
    [SerializeField] LayerMask dashBlockLayers = ~0;
    [SerializeField] float dashStopOffset = 0.1f;
    float dashCooldownTimer;
    bool isDashing = false;

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

        dashCooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(1))
        {
            dash();
        }
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

    void dash()
    {
        if (isDashing) return;
        if (dashCooldownTimer > 0f) return;

        Vector3 dir = transform.forward;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f)
            dir = Vector3.forward;
        else
            dir.Normalize();

        float actualDistance = dashDistance;

        float rayStartOffset = 0.2f;
        Vector3 rayOrigin = transform.position + dir * rayStartOffset;
        float rayLength = Mathf.Max(0.01f, dashDistance - rayStartOffset);

        if (Physics.Raycast(rayOrigin, dir, out RaycastHit hit, rayLength, dashBlockLayers))
        {
            actualDistance = rayStartOffset + Mathf.Max(0f, hit.distance - dashStopOffset);
        }

        StartCoroutine(DashCoroutine(dir, actualDistance));
        dashCooldownTimer = dashCooldown;
    }

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

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Enemy"))
    //    {
    //        print("s");
    //        hp.curHP -= damage;
 
    //    }
    //}
}