using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Detect")]
    [SerializeField] float detectRange = 3f;
    [SerializeField] LayerMask playerMask = ~0;

    [Header("Slide Move")]
    [SerializeField] bool moveAlongX = true;   // true: X, false: Z
    [SerializeField] bool moveToPositive = true;
    [SerializeField] float moveDistance = 2f;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] Transform moveTarget;

    Vector3 closedLocalPosition;
    Vector3 openedLocalPosition;

    void Start()
    {
        if (moveTarget == null) moveTarget = transform;

        closedLocalPosition = moveTarget.localPosition;

        Vector3 moveAxis = moveAlongX ? Vector3.right : Vector3.forward;
        float directionSign = moveToPositive ? 1f : -1f;
        openedLocalPosition = closedLocalPosition + moveAxis * directionSign * moveDistance;
    }

    void Update()
    {
        bool playerInRange = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, detectRange, playerMask);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].CompareTag("Player"))
            {
                playerInRange = true;
                break;
            }
        }

        Vector3 targetLocalPosition = playerInRange ? openedLocalPosition : closedLocalPosition;
        moveTarget.localPosition = Vector3.MoveTowards(
            moveTarget.localPosition,
            targetLocalPosition,
            moveSpeed * Time.deltaTime
        );
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
