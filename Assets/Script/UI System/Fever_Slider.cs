using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UI;
using static UITest_Player;

public class Fever_Slider : MonoBehaviour
{
    /*
     * 피버 슬라이더 기능
     * -> 특정 조건 달성시 슬라이더가 점점 증가하다 최대값에 도달하면 일정한 속도로 감소
     * 
     * 현재 임시 조건
     * 조건 - 마우스 클릭 횟수
     * 
     * 후에 정식으로 채택할 조건
     * 조건 - 몬스터 특정 획수 만큼 죽일시
     */

    [Header("Fever Slider")]
    public Slider feverSlider; //피버 슬라이더

    public float feverValue; //피버 값
    [SerializeField] private float maxFever; //최대 피버 값
    [SerializeField] private float minFever; //최소 피버 값
    [SerializeField] private float currentFever; // 현재 피버 값
    private float perFever; //백분율 피버 값

    public enum FeverState
    {
        None, /*feverDesr,*/ feverIncr
    }
    public FeverState feverState = FeverState.None;

    public float feverIncrSpeed; //피버 증가 속도
    //public float feverDesrSpeed; //피버 감소 속도

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minFever = feverValue;
        feverSlider.value = currentFever / maxFever;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerFever();

        feverSlider.value = currentFever / maxFever;

        if (Input.GetMouseButtonDown(0)) {
            currentFever -= 10;
        }

    }

    void PlayerFever()
    {
        switch (feverState)
        {
            case FeverState.feverIncr:
                {
                    //Fever슬라이더의 값을 perFever값으로 feverIncrSpeed의 속도로 일정하게 이동한다.
                    feverSlider.value = Mathf.MoveTowards(feverSlider.value, perFever, feverIncrSpeed * Time.deltaTime);

                    if (feverSlider.value == perFever)
                    {
                        feverState = FeverState.None;
                    }
                    break;
                }
        }
    }
}
