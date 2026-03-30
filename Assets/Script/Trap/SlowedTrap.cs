using UnityEngine;

public class SlowedTrap : MonoBehaviour
{
    [Header("Trap Range")]
    [SerializeField] float range = 6f;
    [SerializeField] LayerMask playerMask; // Player가 속한 레이어만 감지

    bool isSlowing = false;
    Player targetPlayer = null;
    float originalSpeed = 0f;

    void Update()
    {
        // 내 주변 range 안에 Player가 있는지 확인
        Collider[] hits = Physics.OverlapSphere(transform.position, range, playerMask);
        if (hits.Length > 0)
        {
            // Player가 하나라도 있으면, 첫 번째를 기준으로 처리
            Player p = hits[0].GetComponent<Player>();
            if (p != null)
            {
                // 아직 슬로우를 안 걸었으면, 지금 걸기
                if (!isSlowing)
                {
                    // 현재 속도를 기준으로 20% 감소
                    targetPlayer = p;
                    originalSpeed = targetPlayer.speed;
                    targetPlayer.speed = originalSpeed * 0.5f;
                    isSlowing = true;
                }
                return; // 범위 안에 있으므로 여기서 종료
            }
        }
        // 범위 안에 Player가 없을때
        if (isSlowing && targetPlayer != null)
        {
            // 이전에 슬로우 걸려 있던 플레이어가 있었다면, 원래 속도로 복구
            targetPlayer.speed = originalSpeed;
            isSlowing = false;
            targetPlayer = null;
        }
    }
    // 에디터에서 범위를 눈으로 확인하기 위한 Gizmo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}