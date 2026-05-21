using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class room : MonoBehaviour
{
    [SerializeField] private Text clearText;
    [SerializeField] private string fulltext = "Room Clear";    

    [Header("Detection Settings")]
    [SerializeField] private float radius = 10f;
    [SerializeField] private LayerMask playerMask;

    private bool isClearAnimationStarted = false;
    private bool isPlayerInRoom = false;

    void Update()
    {
        // 1. 플레이어가 해당 방 범위 내에 있는지 확인
        bool isDetected = Physics.CheckSphere(transform.position, radius, playerMask);

        if (isDetected)
        {
            isPlayerInRoom = true;

            // 2. 방 범위 내에 적(Enemy)이 없는지 확인
            if (!HasEnemiesInRange() && !isClearAnimationStarted)
            {
                isClearAnimationStarted = true;
                StopAllCoroutines();
                StartCoroutine(RoomClearEffect());
            }
        }
        else
        {
            // 3. 방을 나가면 텍스트를 즉시 비우고 상태 리셋
            if (isPlayerInRoom)
            {
                isPlayerInRoom = false;
                isClearAnimationStarted = false; // 다시 들어왔을 때 재표시 가능하게 함
                if (clearText != null)
                {
                    StopAllCoroutines();
                    clearText.text = "";
                }
            }
        }
    }

    private bool HasEnemiesInRange()
    {
        // GetComponentsInChildren<Enemy>() 대신 OverlapSphere로 주변의 적을 직접 감지합니다.
        // 이는 적이 방의 자식 오브젝트가 아니더라도 정상적으로 작동하게 합니다.
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (var hit in hits)
        {
            // 태그로 확인 (Enemy 또는 suicide 태그)
            if (hit.CompareTag("Enemy") || hit.CompareTag("suicide"))
            {
                return true;
            }

            // 컴포넌트로 확인 (태그가 설정되지 않은 경우를 대비하여 여러 타입 체크)
            if (hit.GetComponent<Enemy>() != null || 
                hit.GetComponent<BuildingEnemy>() != null || 
                hit.GetComponent<RangedEnemy>() != null || 
                hit.GetComponent<suicide>() != null)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator RoomClearEffect()
    {
        if (clearText == null) yield break;

        clearText.text = "";
        foreach (char c in fulltext)
        {
            clearText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
