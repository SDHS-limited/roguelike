using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 50f;
    [SerializeField] float lifeTime = 3f;

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
            Debug.Log("나 때렸어?");
            Destroy(gameObject);
           
            if (enemy != null)
            {
                Debug.Log("적 맞음!");
                enemy.hp -= 20;
            }

        }
        
        // 여기서 데미지 처리 가능
        Destroy(gameObject);
    }
}
