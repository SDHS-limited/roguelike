using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 50f;
    [SerializeField] float lifeTime = 3f;
    [SerializeField] public float Damage = 20;
    [SerializeField] GameObject hitEffectPrefab;

    Rigidbody rb;
    Enemy enemy;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            Destroy(gameObject);
           
            if (enemy != null)
            {
                enemy.hp -= Damage;
            }
            
        }
        else
        {
            // 충돌 지점 정보 가져오기
            ContactPoint contact = collision.contacts[0];

            // 파티클 생성 (법선 방향으로 회전)
            GameObject hitt = Instantiate(
                hitEffectPrefab,
                contact.point,
                Quaternion.LookRotation(contact.normal)
            );

            Destroy(hitt, 1f);
        }
        
        // 여기서 데미지 처리 가능
        Destroy(gameObject);
    }
}
