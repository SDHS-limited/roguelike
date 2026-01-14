using UnityEngine;
using UnityEngine.UI;

public class UITest_Player : MonoBehaviour
{
    [SerializeField] float speed = 5.0f;
    Rigidbody rb;

    //[Header("info")]
    //[SerializeField] public float hp = 150;

    [Header("HP Slider")]
    public Slider hpSlider; //HP 슬라이더

    public float playerHp; //플레이어 HP
    private float maxHp; //최대 HP
    private float perHp; //백분율 HP 값

    //HP 슬라이더 상태
    public enum HPState
    {
        None, HPDesr, //HPHeal
    }
    public HPState hpState = HPState.None; //HP 슬라이더 기본 상태

    public float hpDesrSpeed; //HP 감소 속도
    //public float hpHealSpeed; //HP 회복 속도
    
    /*
     * 현재 시스템 : tag가 'Enemy'인 물체에 충돌하면 hp가 10씩 감소하는 시스템
     * -> hp가 10씩 감소하되 감소되는 hp와 슬라이더 UI가 동기화 되어야함
     * -> 
     * 
     */

    [Header("Fever Slider")]
    public Slider feverSlider; //피버 슬라이더

    public float feverValue; //피버 값
    public float maxFever; //최대 피버 값
    public float perFever; //백분율 피버 값

    public enum FeverState
    {
        None, feverDesr, feverIncr
    }

    public float feverIncrSpeed; //피버 증가 속도
    //public float feverDesrSpeed; //피버 감소 속도

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxHp = playerHp;
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = 60;
        move();
        Debug.Log(playerHp);
        PlayerHP();
    }
    void move()
    {
        float moveH = Input.GetAxisRaw("Horizontal");
        float moveV = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(moveH, 0, moveV);
        transform.Translate(move * speed * Time.deltaTime);
    }

    void PlayerHP()
    {
        switch (hpState)
        {
            case HPState.HPDesr:
                {
                    //HP슬라이더의 값을 hpPer값으로 hpSpeed의 속도로 일정하게 이동한다.
                    hpSlider.value = Mathf.MoveTowards(hpSlider.value, perHp, hpDesrSpeed * Time.deltaTime);

                    if (hpSlider.value == perHp)
                    {
                        hpState = HPState.None;
                    }
                    break;
                }
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            playerHp -= 10;
            perHp = playerHp / maxHp; //현재 남은 HP를 전체HP로 나눈다. //ex) 전체:1000 - 100 = 남은HP:900 | 1000/900 = 0.9
            hpState = HPState.HPDesr; //HP감소 상태로 변경
        }
    }
}
