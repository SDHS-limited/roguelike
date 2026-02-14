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
        dash();
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

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
        if (!Input.GetMouseButtonDown(1))
            return;

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
        transform.Translate(dir * actualDistance, Space.World);

        dashCooldownTimer = dashCooldown;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hp.curHP -= damage;
            StartCoroutine(effect.Damage());
        }
    }
}
