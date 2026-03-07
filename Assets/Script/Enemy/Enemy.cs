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
    [SerializeField] ParticleSystem bloodEffect;
    [Header("Gizmos")]
    [SerializeField] float range;
    [SerializeField] float smallrange;
    [SerializeField] LayerMask playerMask;

    [Header("Ai")]
    [SerializeField] NavMeshAgent nav;
    [SerializeField] Transform target;

    [Header("Info")]
    [SerializeField] public float hp = 60;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        bloodEffect.Stop();
        RegisterToMaps();
    }

    void OnDestroy()
    {
        UnregisterFromMaps();
    }

    /// <summary> 미니맵/전체맵 매니저에 몬스터 아이콘 등록 (비활성 오브젝트도 찾기) </summary>
    void RegisterToMaps()
    {
        var minimap = FindFirstObjectByType<MinimapManeger>(FindObjectsInactive.Include);
        if (minimap != null) minimap.RegisterMonster(this);

        var fullmap = FindFirstObjectByType<FullMapManager>(FindObjectsInactive.Include);
        if (fullmap != null) fullmap.RegisterMonster(this);
    }

    /// <summary> 미니맵/전체맵에서 몬스터 아이콘 제거 </summary>
    void UnregisterFromMaps()
    {
        var minimap = FindFirstObjectByType<MinimapManeger>(FindObjectsInactive.Include);
        if (minimap != null) minimap.UnregisterMonster(this);

        var fullmap = FindFirstObjectByType<FullMapManager>(FindObjectsInactive.Include);
        if (fullmap != null) fullmap.UnregisterMonster(this);
    }

    // Update is called once per frame
    void Update()
    {
        Range();
        SmallRange();
        
        if(hp <= 0)
        {
            Destroy(gameObject);   
        }
    }

    void Range()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, range, playerMask);

        if (hit.Length > 0)
        {
            nav.isStopped = false;
            anim.SetBool("isMove", true);
            nav.SetDestination(target.position);
        }
        else
        {
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            bloodEffect.Play();        
        }
    }
}
