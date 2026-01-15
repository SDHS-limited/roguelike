using UnityEngine;

public class ExperimentObj : MonoBehaviour
{
    [SerializeField] float range = 6f;
    [SerializeField] LayerMask playerMask;
    [SerializeField] GameObject experiment; // 실험창
    [SerializeField] Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        experiment.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, range, playerMask);

        if (hit.Length > 0)
        {
            experiment.gameObject.SetActive(true);
            anim.SetBool("isopen", true);
        }
    }
     void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
