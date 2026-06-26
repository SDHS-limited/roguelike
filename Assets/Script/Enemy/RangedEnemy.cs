using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RangedEnemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float chaseRange = 30f;
    [SerializeField] private float idealDistance = 15f;
    [SerializeField] private float retreatDistance = 8f;
    
    [Header("Combat")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float projectileSpeed = 20f;

    [Header("Info")]
    [SerializeField] private float hp = 80f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private DamageText damageText;

    private NavMeshAgent nav;
    private Transform player;
    private float nextFireTime;
    private Animator anim;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        nav.stoppingDistance = idealDistance;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < chaseRange)
        {
            HandleAI(distance);
        }
        else
        {
            nav.isStopped = true;
            if(anim) anim.SetBool("isMove", false);
        }
    }

    void HandleAI(float distance)
    {
        // 1. Position Management
        if (distance < retreatDistance)
        {
            // Retreat: Move away from player
            Vector3 retreatDir = (transform.position - player.position).normalized;
            Vector3 targetPos = transform.position + retreatDir * 5f;
            nav.SetDestination(targetPos);
            nav.isStopped = false;
        }
        else if (distance > idealDistance + 2f)
        {
            // Approach
            nav.SetDestination(player.position);
            nav.isStopped = false;
        }
        else
        {
            // Within ideal range
            nav.isStopped = true;
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }

        if(anim) anim.SetBool("isMove", !nav.isStopped);

        // 2. Shooting
        if (distance < idealDistance + 5f && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    void Shoot()
    {
        if(anim) anim.SetTrigger("Attack");
        
        if (projectilePrefab != null && muzzle != null)
        {
            Vector3 dir = (player.position + Vector3.up * 1.5f - muzzle.position).normalized;
            GameObject proj = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(dir));
            
            // Reusing existing projectile script if possible, or simple velocity
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = dir * projectileSpeed;
        }
    }

    [Header("Juice")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject bloodExplosionPrefab;
    [SerializeField] private GameObject bloodSplatterPrefab;

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
            Fever_Slider fever = Object.FindFirstObjectByType<Fever_Slider>();
            if (fever != null) fever.AddFever(10f);

            // Trigger purification effect from previous implementation
            Effect effects = Object.FindFirstObjectByType<Effect>();
            if (effects != null) effects.TriggerPurification();
            
            SpawnDeathEffects();
            Destroy(gameObject);
        }
    }

    private void SpawnDeathEffects()
    {
        DeathEffectUtil.SpawnDeathEffects(transform.position, bloodExplosionPrefab, bloodSplatterPrefab);
    }
    }
