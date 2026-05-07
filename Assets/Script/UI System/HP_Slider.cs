using UnityEngine;
using UnityEngine.UI;

public class HP_Slider : MonoBehaviour
{
    [SerializeField] public Slider HPslider;
    public float maxHp = 150f;
    public float curHP = 150f;

    void Start()
    {
        // 초기 슬라이더 설정
        UpdateHPUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.Log("데미지 입음!");
            // 1. 실제 체력 변수값을 깎습니다 (예: 10씩 감소)
            curHP -= 10f;

            // 2. 체력이 0보다 낮아지지 않게 제한합니다
            curHP = Mathf.Clamp(curHP, 0, maxHp);

            // 3. 바뀐 체력을 UI에 반영합니다
            UpdateHPUI();
        }
    }

    // UI를 업데이트하는 별도의 함수를 만드는 것이 관리하기 좋습니다.
    void UpdateHPUI()
    {
        if (HPslider != null)
        {
            HPslider.value = curHP / maxHp;
        }
    }
}
