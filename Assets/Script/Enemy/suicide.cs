using UnityEngine;
using UnityEngine.AI;

public class suicide : MonoBehaviour
{
    [SerializeField] HP_Slider playerHP;
    [SerializeField] DamageText damageText;
    [SerializeField] float hp = 120;
    
    [Header("AI")]
    [SerializeField] NavMeshAgent nav;
    [SerializeField] Transform target;

    [Header("Gizmos")]
    [SerializeField] float range;
    [SerializeField] LayerMask playerMask;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRange = 4.0f;
    [SerializeField] private float primingDistance = 7.0f;
    [SerializeField] private float explosionDamage = 40f;
    [SerializeField] private float primingSpeedMultiplier = 1.4f;
    [SerializeField] private GameObject bloodSplatterPrefab;
    [SerializeField] private GameObject bloodSmokePrefab;
    
    private bool isPrimed = false;
    private float originalSpeed;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        if(nav != null) originalSpeed = nav.speed;
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null) target = playerObj.transform;
    }

    [SerializeField] private GameObject hitEffectPrefab;

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (damageText != null) damageText.SetDamage((int)damage);
        
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position + Vector3.up, Quaternion.identity, transform);
        }

        if (hp <= 0)
        {
            Explode();
        }
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        
        if (distance < range)
        {
            HandleAI(distance);
        }
        else
        {
            if(nav != null) nav.isStopped = true;
        }
    }

    void HandleAI(float distance)
    {
        if(nav == null) return;

        nav.isStopped = false;
        nav.SetDestination(target.position);

        if (!isPrimed && distance < primingDistance)
        {
            StartPriming();
        }

        if (isPrimed && distance < explosionRange)
        {
            Explode();
        }
    }

    void StartPriming()
    {
        isPrimed = true;
        if(nav != null) nav.speed = originalSpeed * primingSpeedMultiplier;
        
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null) renderer.material.color = Color.red;
    }

    void Explode()
    {
        Fever_Slider fever = Object.FindFirstObjectByType<Fever_Slider>();
        if (fever != null) fever.AddFever(15f);

        float distToPlayer = Vector3.Distance(transform.position, target.position);
        if (distToPlayer < explosionRange + 1f && playerHP != null)
        {
            playerHP.TakeDamage(explosionDamage);
        }

        SpawnDeathEffects();
        Effect effects = Object.FindFirstObjectByType<Effect>();
        if (effects != null) effects.TriggerCameraShake(0.3f, 0.4f);

        Destroy(gameObject);
    }

    private void SpawnDeathEffects()
    {
        DeathEffectUtil.SpawnDeathEffects(transform.position, bloodSmokePrefab, bloodSplatterPrefab);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position, range);    
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, primingDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, explosionRange);
    }
}
