using UnityEngine;

public class RestManager : MonoBehaviour
{
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
