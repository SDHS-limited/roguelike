using UnityEngine;

public class RestManager : MonoBehaviour
{
    
    //나중에 클래스 이름 바꾸고 하자(체력 회복 박스)
    [SerializeField] Effect effect;
    [SerializeField] HP_Slider hp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hp.curHP = 150;
            StartCoroutine(effect.Heal());
        }
    }
}
