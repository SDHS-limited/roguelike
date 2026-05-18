    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.AI;
    using System.Collections;

    public enum Demon
    {
        idle,
        walk,
        attack,
    }

    public class Enemy : MonoBehaviour
    {
        [SerializeField] Animator anim;
        [SerializeField] Demon currentState;
        [SerializeField] DamageText damageText;
        [SerializeField] private float rotationSpeed = 8f; // 회전 속도 (높을수록 빠름)
        //[SerializeField] ParticleSystem bloodEffect;

        [Header("Gizmos")]
        [SerializeField] float range;
        [SerializeField] float smallrange;
        [SerializeField] LayerMask playerMask;

        [Header("Ai")]
        [SerializeField] NavMeshAgent nav;
        [SerializeField] Transform target;

        [Header("Info")]
        [SerializeField] public float hp = 60;
        
        [Header("AI Settings")]
        [SerializeField] private float chaseSpeed = 9f;
        [SerializeField] private float aggressiveRange = 10f;
        [SerializeField] private float aggressiveSpeedMultiplier = 1.3f;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();

        nav.speed = chaseSpeed;
        nav.acceleration = 14f;
        nav.angularSpeed = 100f;
        nav.stoppingDistance = 2f;
        nav.updateRotation = false; // ← NavMesh 자동 회전 비활성화

        RegisterToMaps();
    }

    void OnDestroy()
        {
            UnregisterFromMaps();
        }

        void RegisterToMaps()
        {
            var minimap = FindFirstObjectByType<MinimapManeger>(FindObjectsInactive.Include);
            if (minimap != null) minimap.RegisterMonster(this);

            var fullmap = FindFirstObjectByType<FullMapManager>(FindObjectsInactive.Include);
            if (fullmap != null) fullmap.RegisterMonster(this);
        }

        void UnregisterFromMaps()
        {
            var minimap = FindFirstObjectByType<MinimapManeger>(FindObjectsInactive.Include);
            if (minimap != null) minimap.UnregisterMonster(this);

            var fullmap = FindFirstObjectByType<FullMapManager>(FindObjectsInactive.Include);
            if (fullmap != null) fullmap.UnregisterMonster(this);
        }

    void Update()
    {
        float distance = target != null ? Vector3.Distance(transform.position, target.position) : float.MaxValue;

        if (distance < range)
        {
            HandleChasing(distance);
        }
        else
        {
            StopChasing();
        }

        SmallRange();
        FaceTarget(); // ← 매 프레임 플레이어 바라보기
    }

    void FaceTarget()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // 수직 회전 방지 (고개 들기 방지)

        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    void HandleChasing(float distance)
        {
            nav.isStopped = false;
            anim.SetBool("isMove", true);
            nav.SetDestination(target.position);

            // Aggressive mode when close
            if (distance < aggressiveRange)
            {
                nav.speed = chaseSpeed * aggressiveSpeedMultiplier;
            }
            else
            {
                nav.speed = chaseSpeed;
            }
        }

        void StopChasing()
        {
            anim.SetBool("isMove", false);
            nav.isStopped = true;
        }

        void SmallRange()
        {
            Collider[] hit = Physics.OverlapSphere(transform.position, smallrange, playerMask);

            if (hit.Length > 0)
            {
                nav.isStopped = true;
                anim.SetInteger("Attack", 1);
            }
            else
            {
                anim.SetInteger("Attack", 0);
                nav.isStopped = false;
            }
        }

        [Header("Juice")]
        [SerializeField] private Renderer[] meshRenderers;
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float flashDuration = 0.15f; 
        [SerializeField] private GameObject hitEffectPrefab; 
        private Color[] originalColors;

        public void TakeDamage(float damage)
        {
            hp -= damage;
            if (damageText != null) damageText.SetDamage((int)damage);
            StartCoroutine(FlashRoutine());

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position + Vector3.up, Quaternion.identity);
            }

            Effect effects = FindFirstObjectByType<Effect>();
            if (effects != null)
            {
                effects.TriggerHitStop(0.08f);
                // effects.TriggerCameraShake(0.2f, 0.20f);
            }

            if (hp <= 0)
            {
                Fever_Slider fever = FindFirstObjectByType<Fever_Slider>();
                if (fever != null) fever.AddFever(5f); // 처치 시 피버 5 증가

                Destroy(gameObject);
            }
        }

        private IEnumerator FlashRoutine()
        {
            if (meshRenderers == null || meshRenderers.Length == 0)
                meshRenderers = GetComponentsInChildren<Renderer>();

            if (originalColors == null || originalColors.Length != meshRenderers.Length)
            {
                originalColors = new Color[meshRenderers.Length];
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    if (meshRenderers[i].material.HasProperty("_BaseColor"))
                        originalColors[i] = meshRenderers[i].material.GetColor("_BaseColor");
                    else if (meshRenderers[i].material.HasProperty("_Color"))
                        originalColors[i] = meshRenderers[i].material.color;
                    else
                        originalColors[i] = Color.white;
                }
            }

            foreach (var renderer in meshRenderers)
            {
                if (renderer.material.HasProperty("_BaseColor")) renderer.material.SetColor("_BaseColor", flashColor);
                else if (renderer.material.HasProperty("_Color")) renderer.material.color = flashColor;
            }

            yield return new WaitForSeconds(flashDuration);

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                if (meshRenderers[i].material.HasProperty("_BaseColor")) meshRenderers[i].material.SetColor("_BaseColor", originalColors[i]);
                else if (meshRenderers[i].material.HasProperty("_Color")) meshRenderers[i].material.color = originalColors[i];
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, range);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, smallrange);
        }
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                Bullet bullet = collision.gameObject.GetComponent<Bullet>();
                if (bullet != null) TakeDamage(bullet.Damage);
            }
        }
        }
