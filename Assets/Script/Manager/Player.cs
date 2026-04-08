using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] ExperimentManager experimentManager;
    [SerializeField] Effect effect;
    [SerializeField] HP_Slider hp;
    [SerializeField] public float damage = 10f;
    [SerializeField] public float speed = 5.0f;  //이동 속도

    [Header("Dash")]
    [SerializeField] float dashDistance = 5f;           //대쉬 거리
    [SerializeField] float dashCooldown = 1f;           //대쉬 쿨타임
    [SerializeField] LayerMask dashBlockLayers = ~0;    //대쉬를 막을 레이어
    [SerializeField] float dashStopOffset = 0.1f;       //장애물과의 간격
    float dashCooldownTimer;                            //남은 쿨타임
    bool isDashing = false; 

    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        move();

        dashCooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(1))
        {
            dash();
        }
    }
    void move()
    {
        // if (!experimentManager.isSelete)
        // {
            float moveH = Input.GetAxisRaw("Horizontal");
            float moveV = Input.GetAxisRaw("Vertical");
            Vector3 move = new Vector3(moveH, 0, moveV);
            transform.Translate(move * speed * Time.deltaTime);  
        // }
    }
    void dash()
    {
        if (isDashing) return;

        if (dashCooldownTimer > 0f)
            return;

        //캐릭터가 보는 방향으로만 대쉬
        Vector3 dir = transform.forward;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) dir = Vector3.forward;
        else dir.Normalize();

        //앞에 Collider가 있으면 그 앞까지만 이동
        float actualDistance = dashDistance;
        float rayStartOffset = 0.2f; //자기 자신 콜라이더에 맞지 않도록 살짝 앞에서 시작
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

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dashDuration;

            // 부드러운 가속/감속 느낌
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
    }

    transform.position = targetPos;
    isDashing = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hp.HPslider.value = Mathf.Lerp(hp.curHP, hp.maxHp, hp.HPslider.value - Time.deltaTime);
            hp.curHP -= damage;
            StartCoroutine(effect.Damage());
        }
    }
}
