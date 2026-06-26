using UnityEngine;

public class BuildBullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private GameObject bloodEffectPrefab;

    private Vector3 _direction;
    private bool _launched;
    private Rigidbody _rb;
    private int _playerLayer;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
            _rb.useGravity = false;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        _playerLayer = LayerMask.NameToLayer("Player");
    }

    public void Launch(Vector3 dir, float overrideSpeed = -1f)
    {
        _direction = dir;
        if (overrideSpeed > 0f) speed = overrideSpeed;
        _launched = true;

        if (_rb != null)
        {
            _rb.linearVelocity = _direction * speed;
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!_launched) return;

        // Rigidbody가 없는 탄환 프리팹도 동작하도록 폴백 이동
        if (_rb == null)
        {
            transform.position += _direction * speed * Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;
        Player targetPlayer = hitObject.GetComponentInParent<Player>();
        HP_Slider targetHP = hitObject.GetComponentInParent<HP_Slider>();
        bool isPlayerHit =
            (_playerLayer != -1 && hitObject.layer == _playerLayer) ||
            targetPlayer != null ||
            targetHP != null;

        if (isPlayerHit)
        {
            if (targetPlayer != null) targetPlayer.TakeDamage(damage);
            else if (targetHP != null) targetHP.TakeDamage(damage);
            else hitObject.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

            // 피 파티클 생성
            if (bloodEffectPrefab != null)
            {
                Vector3 hitPoint = hitObject.transform.position;
                Vector3 hitNormal = -transform.forward;
                if (collision.contacts.Length > 0)
                {
                    hitPoint = collision.contacts[0].point;
                    hitNormal = collision.contacts[0].normal;
                }

                GameObject blood = Instantiate(bloodEffectPrefab, hitPoint, Quaternion.LookRotation(hitNormal), collision.transform);
                Destroy(blood, 2f);
            }

            Destroy(gameObject);
        }
        else if (!hitObject.CompareTag("Enemy"))
        {
            // 벽 등에 부딪혔을 때
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;
        Player targetPlayer = hitObject.GetComponentInParent<Player>();
        HP_Slider targetHP = hitObject.GetComponentInParent<HP_Slider>();
        bool isPlayerHit =
            (_playerLayer != -1 && hitObject.layer == _playerLayer) ||
            targetPlayer != null ||
            targetHP != null;

        if (!isPlayerHit) return;

        if (targetPlayer != null) targetPlayer.TakeDamage(damage);
        else if (targetHP != null) targetHP.TakeDamage(damage);
        else hitObject.SendMessageUpwards("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

        if (bloodEffectPrefab != null)
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            GameObject blood = Instantiate(bloodEffectPrefab, hitPoint, Quaternion.LookRotation(-transform.forward), other.transform);
            Destroy(blood, 2f);
        }

        Destroy(gameObject);
    }
    }
