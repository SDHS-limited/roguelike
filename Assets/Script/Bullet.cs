using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 50f;
    [SerializeField] float lifeTime = 3f;

    Rigidbody rb;

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
            Debug.Log("나 때렸어?");
        }
        
        // 여기서 데미지 처리 가능
        Destroy(gameObject);
    }
}
