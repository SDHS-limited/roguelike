using UnityEngine;

public class BuildBullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float damage = 20f;
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] Player player;

    private Vector3 _direction;
    private bool _launched;

    public void Launch(Vector3 dir)
    {
        _direction = dir;
        _launched = true;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!_launched) return;
        transform.position += _direction * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 플레이어 데미지 처리
            collision.gameObject.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            player.TakeDamage(5);

            // 피 파티클 생성
            if (bloodEffectPrefab != null)
            {
                ContactPoint contact = collision.contacts[0];
                GameObject blood = Instantiate(bloodEffectPrefab, contact.point, Quaternion.LookRotation(contact.normal), collision.transform);
                Destroy(blood, 2f);
            }

            Destroy(gameObject);
        }
        else if (!collision.gameObject.CompareTag("Enemy")) 
        {
            // 벽 등에 부딪혔을 때
            Destroy(gameObject);
        }
    }
    }
