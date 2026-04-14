using System;
using UnityEngine;

public class BuildingEnemy : MonoBehaviour
{
    [Header("감지")]
    [SerializeField] float detectionRange = 20f;
    [SerializeField] LayerMask playerLayer;

    [Header("발사")]
    [SerializeField] Transform muzzle;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float fireRate = 1.5f;

    [SerializeField] float hp = 100f;

    private Transform _player;
    private float _fireCooldown;

    void Update()
    {
        DetectPlayer();

        if (_player != null)
        {
            _fireCooldown -= Time.deltaTime;
            if (_fireCooldown <= 0f)
            {
                Fire();
                _fireCooldown = 1f / fireRate;
            }
        }
    }

    void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, playerLayer);
        if (hits.Length > 0)
        {
            _player = hits[0].transform; // 가장 가까운 플레이어 선택
            Fire();
        }
        else
        {
            _player = null;
        }
        
    }

    void Fire()
    {
        Vector3 dir = (_player.position - muzzle.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        proj.GetComponent<BuildBullet>()?.Launch(dir);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hp -= 20f;
            if (hp <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
