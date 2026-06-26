using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 50f;
    [SerializeField] float lifeTime = 3f;
    [SerializeField] public float Damage = 10;
    [SerializeField] GameObject hitEffectPrefab;
    [SerializeField] GameObject bloodEffectPrefab;

    public void Initialize(float multiplier)
    {
        Damage *= multiplier;
    }
    
    Rigidbody rb;
void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        ContactPoint contact = collision.contacts[0];

        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            // 데미지 전달
            collision.gameObject.SendMessage("TakeDamage", Damage, SendMessageOptions.DontRequireReceiver);

            // 피 파티클 생성
            GameObject effectToSpawn = bloodEffectPrefab; 
            
            if (effectToSpawn != null)
            {
                // Parent the blood effect to the hit object so it "keeps up" with movement
                GameObject blood = Instantiate(effectToSpawn, contact.point, Quaternion.LookRotation(contact.normal), collision.transform);
                Destroy(blood, 2f);
            }

            Destroy(gameObject);
        }
        else
        {
            // 벽 등 기타 충돌
            if (hitEffectPrefab != null)
            {
                GameObject hitt = Instantiate(
                    hitEffectPrefab,
                    contact.point,
                    Quaternion.LookRotation(contact.normal)
                );
                Destroy(hitt, 1f);
            }
            Destroy(gameObject);
        }
    }
    }
