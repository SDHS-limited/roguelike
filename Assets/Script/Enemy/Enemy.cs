using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Gizmos")]
    [SerializeField] float range;
    [SerializeField] LayerMask playerMask;

    [Header("Ai")]
    [SerializeField] NavMeshAgent nav;
    [SerializeField] Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, range, playerMask);

        if (hit.Length > 0)
        {
            nav.SetDestination(target.position);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
