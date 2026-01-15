using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public enum Demon
{
    idle,
    walk,
    attack,
}

public class Enemy : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Demon currentState;
    [Header("Gizmos")]
    [SerializeField] float range;
    [SerializeField] float smallrange;
    [SerializeField] LayerMask playerMask;

    [Header("Ai")]
    [SerializeField] NavMeshAgent nav;
    [SerializeField] Transform target;

    [Header("Info")]
    [SerializeField] public float hp = 40;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Range();
        SmallRange();
        
    }

    void Range()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, range, playerMask);

        if (hit.Length > 0)
        {
            nav.isStopped = false;
            print("따라감");
            anim.SetBool("isMove", true);
            nav.SetDestination(target.position);
        }
        else
        {
            print("멈춤");
            anim.SetBool("isMove", false);
            nav.isStopped = true;
        } 
    }

    void SmallRange()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, smallrange, playerMask);

        if (hit.Length > 0)
        {
            nav.isStopped = true;
            anim.SetInteger("Attack", 1);
            Debug.Log("공격");
        }
        else
        {
            anim.SetInteger("Attack", 0);
            nav.isStopped = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, smallrange);
    }
}
