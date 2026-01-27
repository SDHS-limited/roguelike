using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] ExperimentManager experimentManager;
    [SerializeField] HP_Slider hp;
    [SerializeField] public float speed = 5.0f;  //이동 속도
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        move();
    }
    void move()
    {
        // if (!experimentManager.isSelete)
        // {
            float moveH = Input.GetAxisRaw("Horizontal");
            float moveV = Input.GetAxisRaw("Vertical");
            Vector3 move = new Vector3(moveH, 0, moveV);
            transform.Translate(move * speed * Time.deltaTime);  
        // }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hp.curHP -= 3;
        }
    }
}
