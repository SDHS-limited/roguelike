using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UI;
using static UITest_Player;

public class Fever_Slider : MonoBehaviour
{
    /*
     * 피버 시스템 설명
     * -> 특정 조건을 만족하면 피버 게이지가 증가하고,
     *    최대값에 도달하면 일정 속도로 차오르는 구조
     * 
     * 현재 임시 기준
     * 조건 - 마우스 클릭 횟수
     * 
     * 이후 기획에 따라 변경 가능
     * 예시 - 적 처치 시 특정 수치만큼 증가
     */

    [Header("Fever Slider")]
    public Slider feverSlider; // 피버 슬라이더 UI

    public float feverValue;              // 피버 수치
    [SerializeField] private float maxFever;     // 최대 피버 수치
    [SerializeField] private float minFever;     // 최소 피버 수치
    [SerializeField] private float currentFever; // 현재 피버 수치
    private float perFever;                       // 목표 피버 수치

    public enum FeverState
    {
        None,
        feverIncr    // 피버 증가 상태
    }

    public FeverState feverState = FeverState.None;

    public float feverIncrSpeed; // 피버 증가 속도
    // public float feverDesrSpeed; // 피버 감소 속도 (미사용)

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
    }

    void PlayerFever()
    {
        switch (feverState)
        {
            case FeverState.feverIncr:
            {
                // 피버 슬라이더를 perFever 값까지
                // feverIncrSpeed 속도로 부드럽게 증가시킨다
                feverSlider.value = Mathf.MoveTowards(
                    feverSlider.value,
                    perFever,
                    feverIncrSpeed * Time.deltaTime
                );

                // 목표 수치에 도달하면 상태 초기화
                if (feverSlider.value == perFever)
                {
                    feverState = FeverState.None;
                }
                break;
            }
        }
    }
}