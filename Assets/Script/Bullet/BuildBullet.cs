using UnityEngine;

public class BuildBullet : MonoBehaviour
{
    [SerializeField] private float speed = 15f;      // Fire()의 projectileSpeed와 맞추세요
    [SerializeField] private float lifetime = 5f;    // 5초 후 자동 삭제
    [SerializeField] private float damage = 20f;
    [SerializeField] Player player;

    private Vector3 _direction;   // 발사 방향
    private bool _launched;       // Launch() 호출 여부

    public void Launch(Vector3 dir)
    {
        _direction = dir;
        _launched = true;

        // lifetime 후 자동 삭제
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!_launched) return;

        // 방향으로 매 프레임 이동
        transform.position += _direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그에 닿으면 피해 처리
        if (other.CompareTag("Player"))
        {
            // 플레이어 체력 스크립트에 맞게 수정하세요
            player.TakeDamage(8);
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
