using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Door : MonoBehaviour
{
    [Header("Detect")]
    [SerializeField] float detectRange = 5f;
    [SerializeField] LayerMask playerMask = ~0;

    [Header("Slide Move")]
    [SerializeField] Vector2 moveOffsetXZ = new Vector2(2f, 0f); // x -> X axis, y -> Z axis offset
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] Transform moveTarget;

    [Header("Room Clear Settings")]
    public bool isLocked = true; // 방 클리어 전에는 굳게 닫혀있음

    Vector3 closedWorldPosition;
    Vector3 openedWorldPosition;

    void Start()
    {
        if (moveTarget == null) moveTarget = transform;

        // Runtime safety: if this object is still static, force it off.
        if (moveTarget.gameObject.isStatic) moveTarget.gameObject.isStatic = false;

        Renderer[] renderers = moveTarget.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && renderers[i].gameObject.isStatic)
                renderers[i].gameObject.isStatic = false;
        }

        closedWorldPosition = moveTarget.position;

        Vector3 slideOffset = new Vector3(moveOffsetXZ.x, 0f, moveOffsetXZ.y);
        openedWorldPosition = closedWorldPosition + slideOffset;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (moveTarget == null) moveTarget = transform;

        if (moveTarget != null)
        {
            GameObjectUtility.SetStaticEditorFlags(moveTarget.gameObject, 0);
            Renderer[] renderers = moveTarget.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                    GameObjectUtility.SetStaticEditorFlags(renderers[i].gameObject, 0);
            }
        }
    }
#endif

    // 💡 방이 클리어되었을 때 room.cs에서 호출하여 잠금만 해제
    public void UnlockDoor()
    {
        isLocked = false;
    }

    void Update()
    {
        bool playerInRange = false;

        // 🚪 문이 잠겨있지 않을 때(클리어 후)에만 플레이어의 접근을 감지합니다.
        if (!isLocked)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, playerMask);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].CompareTag("Player"))
                {
                    playerInRange = true;
                    break;
                }
            }
        }

        // 플레이어가 범위 내에 있을 때만 openedWorldPosition으로 이동
        Vector3 targetWorldPosition = playerInRange ? openedWorldPosition : closedWorldPosition;
        
        moveTarget.position = Vector3.MoveTowards(
            moveTarget.position,
            targetWorldPosition,
            moveSpeed * Time.deltaTime
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}