using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] Transform weaponEnd; //총구 끝지점
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fire();
    }
    void fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
        }
    }    
}
