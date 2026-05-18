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

            // 2. 방의 자식으로 등록된 적(Enemy)이 없는지 확인
            // 적이 Destroy되면 GetComponentsInChildren 결과에서 제외됩니다.
            Enemy[] enemies = GetComponentsInChildren<Enemy>();

            if (enemies.Length == 0 && !isClearAnimationStarted)
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
