using UnityEngine;

public class BuildBullet : MonoBehaviour
{
    [SerializeField] float speed   = 30f;
    [SerializeField] float damage  = 20f;
    [SerializeField] float lifetime = 4f;

    private Vector3 _dir;
    private bool _launched;

    public void Launch(Vector3 direction)
    {
        _dir = direction.normalized;
        transform.forward = _dir;
        _launched = true;
        Destroy(gameObject, lifetime);

        // 디버그: 방향 확인용 선
        Debug.DrawRay(transform.position, _dir * 5f, Color.red, 3f);
    }

    void Update()
    {
        if (!_launched) return;
        transform.position += _dir * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            
        }
    }
}
