using UnityEngine;

/// <summary>
/// 건물에 고정된 포탑형 적 AI.
/// 플레이어 감지 → 조준 → 발사 → 피격/사망 처리 흐름으로 동작합니다.
/// </summary>
public class BuildingEnemy : MonoBehaviour
{
    // ───────────────────────────────────────────
    // Detection
    // ───────────────────────────────────────────

    [Header("Detection")]

    /// <summary>플레이어를 감지할 구체 반경 (단위: 미터)</summary>
    [SerializeField] private float detectionRange = 25f;

    /// <summary>플레이어 레이어 마스크 — Inspector에서 "Player" 레이어 지정 필수</summary>
    [SerializeField] private LayerMask playerLayer;

    // ───────────────────────────────────────────
    // Firing
    // ───────────────────────────────────────────

    [Header("Firing")]

    /// <summary>총알이 생성될 발사구 Transform</summary>
    [SerializeField] private Transform muzzle;

    /// <summary>발사할 투사체 프리팹</summary>
    [SerializeField] private GameObject projectilePrefab;

    /// <summary>초당 발사 횟수 (예: 1.2 = 초당 1.2발)</summary>
    [SerializeField] private float fireRate = 1.2f;

    /// <summary>투사체 초기 속도 (단위: m/s)</summary>
    [SerializeField] private float projectileSpeed = 15f;

    /// <summary>조준 시 회전하는 포탑 머리 부분 Transform</summary>
    [SerializeField] private Transform head;

    // ───────────────────────────────────────────
    // Info
    // ───────────────────────────────────────────

    [Header("Info")]

    /// <summary>최대 체력</summary>
    [SerializeField] private float hp = 150f;

    /// <summary>건물 파괴 시 생성할 대형 폭발 이펙트</summary>
    [SerializeField] private GameObject buildingExplosionPrefab;

    /// <summary>사망 시 생성할 혈흔 스플래터 이펙트 프리팹</summary>
    [SerializeField] private GameObject bloodSplatterPrefab;

    /// <summary>사망 시 생성할 연기 이펙트 프리팹 (사용되지 않음 - buildingExplosion으로 대체 가능)</summary>
    [SerializeField] private GameObject bloodSmokePrefab;

    // ───────────────────────────────────────────
    // 런타임 캐시 (private)
    // ───────────────────────────────────────────

    /// <summary>현재 감지된 플레이어 Transform (없으면 null)</summary>
    private Transform _player;

    /// <summary>다음 발사까지 남은 시간 (초)</summary>
    private float _fireCooldown;

    /// <summary>
    /// [개선] FindFirstObjectByType을 매 사망마다 호출하는 비용 절감을 위해
    /// Start()에서 한 번만 캐싱합니다.
    /// </summary>
    private Fever_Slider _feverSlider;

    /// <summary>[개선] 카메라 쉐이크 컴포넌트 캐시</summary>
    private Effect _effectController;

    // ───────────────────────────────────────────
    // Unity Lifecycle
    // ───────────────────────────────────────────

    void Start()
    {
        // [개선] 씬 내 싱글턴성 컴포넌트를 미리 캐싱 — Explode()에서 매번 Find하던 비용 제거
        _feverSlider = Object.FindFirstObjectByType<Fever_Slider>();
        _effectController = Object.FindFirstObjectByType<Effect>();

        // [개선] DetectPlayer를 매 프레임(Update) 대신 0.2초 간격으로만 실행
        // OverlapSphere는 물리 연산 비용이 있으므로 주기를 늦춰 성능 최적화
        InvokeRepeating(nameof(DetectPlayer), 0f, 0.2f);
    }

    void Update()
    {
        // [개선] DetectPlayer() 호출을 Update에서 제거 (InvokeRepeating으로 이관)
        if (_player != null)
        {
            HandleCombat();
        }
    }

    // ───────────────────────────────────────────
    // Detection
    // ───────────────────────────────────────────

    /// <summary>
    /// 감지 범위 내 플레이어를 탐색합니다.
    /// InvokeRepeating으로 0.2초마다 호출됩니다.
    /// </summary>
    void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);

        // hits[0]만 사용 — 플레이어가 항상 1명인 구조 전제
        _player = hits.Length > 0 ? hits[0].transform : null;
    }

    // ───────────────────────────────────────────
    // Combat
    // ───────────────────────────────────────────

    /// <summary>
    /// 플레이어가 감지된 동안 매 프레임 호출됩니다.
    /// 조준(head 회전) → 쿨다운 감소 → 발사 순으로 처리합니다.
    /// </summary>
    void HandleCombat()
    {
        // ── 1. 조준 ──────────────────────────────
        if (head != null)
        {
            // 플레이어 중심부(+1.2f)를 조준 목표로 설정
            Vector3 targetPos = _player.position + Vector3.up * 1.2f;
            Vector3 dir = (targetPos - head.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);

            // Slerp으로 부드러운 회전 보간 (5f = 회전 속도)
            head.rotation = Quaternion.Slerp(head.rotation, targetRot, Time.deltaTime * 5f);
        }

        // ── 2. 발사 쿨다운 ──────────────────────
        _fireCooldown -= Time.deltaTime;
        if (_fireCooldown <= 0f)
        {
            // [개선] head가 null이면 조준이 안 된 상태이므로 발사하지 않음
            // 기존 코드는 head 없이도 발사하여 엉뚱한 방향으로 투사체가 나갔음
            if (head == null)
            {
                Debug.LogWarning($"[BuildingEnemy] head가 비어있어 발사를 건너뜁니다: {name}");
                _fireCooldown = 1f / fireRate;
                return;
            }

            Fire();
            _fireCooldown = 1f / fireRate; // 다음 발사 간격 리셋
        }
    }

    /// <summary>
    /// 투사체를 생성하고 플레이어 방향으로 발사합니다.
    /// Rigidbody 방식과 BuildBullet.Launch() 방식을 모두 지원합니다.
    /// </summary>
    void Fire()
    {
        if (muzzle == null || projectilePrefab == null) return;

        Vector3 dir = (_player.position + Vector3.up * 1.2f - muzzle.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(dir));

        // 자신의 콜라이더와 충돌 무시
        Collider myCol = GetComponent<Collider>();
        Collider projCol = proj.GetComponent<Collider>();
        if (myCol != null && projCol != null)
            Physics.IgnoreCollision(projCol, myCol);

        BuildBullet bullet = proj.GetComponent<BuildBullet>();
        if (bullet != null)
        {
            bullet.Launch(dir);
        }
        else
        {
            Debug.LogWarning("[BuildingEnemy] 프리팹에 BuildBullet 컴포넌트가 없습니다!");
        }
    }

    // ───────────────────────────────────────────
    // Damage / Death
    // ───────────────────────────────────────────

    /// <summary>
    /// 외부(총알 등)에서 호출하는 피격 처리 함수입니다.
    /// </summary>
    /// <param name="damage">받은 피해량</param>
    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0f)
        {
            Explode();
        }
    }

    /// <summary>
    /// 체력이 0 이하가 되었을 때 호출됩니다.
    /// Fever 게이지 증가 → 이펙트 재생 → 카메라 쉐이크 → 오브젝트 파괴 순으로 처리합니다.
    /// </summary>
    void Explode()
    {
        // [개선] Start()에서 캐싱한 참조 사용 (Find 반복 호출 제거)
        if (_feverSlider != null)
            _feverSlider.AddFever(15f);

        // 건물 전용 대형 폭발 생성
        if (buildingExplosionPrefab != null)
        {
            Instantiate(buildingExplosionPrefab, transform.position + Vector3.up, Quaternion.identity);
        }

        // 바닥 혈흔 생성
        if (bloodSplatterPrefab != null)
        {
            DeathEffectUtil.SpawnDeathEffects(transform.position, null, bloodSplatterPrefab);
        }

        if (_effectController != null)
            _effectController.TriggerCameraShake(0.3f, 0.4f); // 건물이므로 더 강한 진동

        Destroy(gameObject);
    }

    // ───────────────────────────────────────────
    // Collision
    // ───────────────────────────────────────────

    /// <summary>
    /// 충돌 감지 — "Bullet" 태그 오브젝트에 닿으면 피해를 받습니다.
    /// 주의: 이 포탑이 Kinematic Rigidbody라면 OnCollisionEnter가 발생하지 않을 수 있습니다.
    /// 그 경우 Bullet 스크립트에서 직접 TakeDamage()를 호출하는 방식으로 변경하세요.
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet b = collision.gameObject.GetComponent<Bullet>();
            if (b != null) TakeDamage(b.Damage);
        }
    }

    // ───────────────────────────────────────────
    // Editor Gizmos
    // ───────────────────────────────────────────

    /// <summary>
    /// 에디터에서 오브젝트 선택 시 감지 범위를 빨간 구체로 시각화합니다.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}