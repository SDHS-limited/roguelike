using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class suicide : MonoBehaviour
{
    [SerializeField] HP_Slider playerHP;
    [Header("AI")]
    [SerializeField] NavMeshAgent nav;
    [SerializeField] Transform target;

    [Header("Gizmos")]
    [SerializeField] float range;
    [SerializeField] LayerMask playerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Range();
    }
    void Range()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, range, playerMask);

        if(hit.Length > 0)
        {
            nav.isStopped = false;
            nav.SetDestination(target.position);
        }
        else
        {
            nav.isStopped = true;
        }
    }
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position, range);    
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //폭발 파티클 추가
            Destroy(this.gameObject);
            playerHP.curHP -= 15;
        }
    }
}
