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
        // 맞은 대상 확인
        Debug.Log("Hit: " + collision.gameObject.name);

        // 여기서 데미지 처리 가능
        Destroy(gameObject);
    }
}
