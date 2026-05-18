using UnityEngine;

public class BuildingEnemy : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 25f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Firing")]
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireRate = 1.2f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private Transform head; // The part that rotates

    [Header("Info")]
    [SerializeField] private float hp = 150f;
    [SerializeField] private GameObject bloodSplatterPrefab;
    [SerializeField] private GameObject bloodSmokePrefab;

    private Transform _player;
    private float _fireCooldown;

    void Update()
    {
        DetectPlayer();

        if (_player != null)
        {
            HandleCombat();
        }
    }

    void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        if (hits.Length > 0)
        {
            _player = hits[0].transform;
        }
        else
        {
            _player = null;
        }
    }

    void HandleCombat()
    {
        // 1. Aiming
        if (head != null)
        {
            Vector3 targetPos = _player.position + Vector3.up * 1.2f;
            Vector3 dir = (targetPos - head.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            head.rotation = Quaternion.Slerp(head.rotation, targetRot, Time.deltaTime * 5f);
        }

        // 2. Firing
        _fireCooldown -= Time.deltaTime;
        if (_fireCooldown <= 0f)
        {
            Fire();
            _fireCooldown = 1f / fireRate;
        }
    }

    void Fire()
    {
        if (muzzle == null || projectilePrefab == null) return;

        Vector3 dir = (_player.position + Vector3.up * 1.2f - muzzle.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(dir));
        
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = dir * projectileSpeed;
        
        // Alternative for non-rigidbody projectiles
        proj.GetComponent<BuildBullet>()?.Launch(dir);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0f)
        {
            Explode();
        }
    }

    void Explode()
    {
        Fever_Slider fever = Object.FindFirstObjectByType<Fever_Slider>();
        if (fever != null) fever.AddFever(15f);

        DeathEffectUtil.SpawnDeathEffects(transform.position, bloodSmokePrefab, bloodSplatterPrefab);
        
        Effect effects = Object.FindFirstObjectByType<Effect>();
        if (effects != null) effects.TriggerCameraShake(0.2f, 0.2f);

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet b = collision.gameObject.GetComponent<Bullet>();
            if (b != null) TakeDamage(b.Damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
