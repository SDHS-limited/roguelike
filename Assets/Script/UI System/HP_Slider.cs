using UnityEngine;
using UnityEngine.UI;

public class HP_Slider : MonoBehaviour
{
    [SerializeField] Slider HPslider;
    public float maxHp = 150;
    public float curHP = 150;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPslider.value = curHP / maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Backspace))
        // {
        //     curHP -= 10;
        // }
        // if(curHP >= 0)
        // {
        //     Debug.Log("기절");
        // }

        HPslider.value = curHP / maxHp;
    }
}
