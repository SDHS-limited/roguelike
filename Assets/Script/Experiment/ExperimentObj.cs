using UnityEngine;

public class ExperimentObj : MonoBehaviour
{
    [SerializeField] float range = 6f;
    [SerializeField] LayerMask playerMask;
    [SerializeField] GameObject experiment; // 실험창
    [SerializeField] ExperimentManager experimentManager;
    // [SerializeField] Animator anim;
    [SerializeField] bool isvisit = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        experiment.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, range, playerMask);

        if (hit.Length > 0)
        {
            experiment.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            // anim.SetBool("isopen", true);
        }
        if (hit.Length > 1) 
        {
            isvisit = false;
        }
        if (experimentManager.isSelete)
        {
            experiment.gameObject.SetActive(false);
            isvisit = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
     void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
