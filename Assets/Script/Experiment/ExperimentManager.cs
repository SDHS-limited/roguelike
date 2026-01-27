using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ExperimentManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Text[] nameTexts;
    [SerializeField] Text[] desTexts;
    [SerializeField] Image[] ImageText;
   // [SerializeField] Image[] experimentImages;
    [SerializeField] Button[] selectButtons;
    [SerializeField] GameObject experiment; // 실험창

    [Header("Data Pool")]
    [SerializeField] Experiment[] allExperiments; // 모든 능력 리스트
    [DoNotSerialize] public bool isSelete = false;

    [Header("Effect")]

    private List<Experiment> currentOptions = new List<Experiment>();

    void Start()
    {
        ShowThreeRandomExperiments();
    }
    void Awake() {
        Application.targetFrameRate = 60;
    }

    void ShowThreeRandomExperiments()
    {
        if (allExperiments.Length < 3) return;

        // 1. 중복 없는 인덱스 3개 뽑기
        List<int> indices = new List<int>();
        while (indices.Count < 3)
        {
            int randomIndex = Random.Range(0, allExperiments.Length);
            if (!indices.Contains(randomIndex)) indices.Add(randomIndex);
        }

        currentOptions.Clear();

        // 2. UI 업데이트 및 버튼 이벤트 할당
        for (int i = 0; i < 3; i++)
        {
            int index = i; // 클로저(Closure) 문제 방지용 변수
            Experiment selectedData = allExperiments[indices[i]];
            currentOptions.Add(selectedData);

            nameTexts[i].text = selectedData.name;
            desTexts[i].text = selectedData.Des;

            // ImageText[i].sprite = selectedData.image;
            // 버튼 클릭 리스너 설정
            selectButtons[i].onClick.AddListener(() =>
            {

                OnSelectExperiment(index);
            });
        }
    }

    // 능력을 선택했을 때 실행되는 함수
    public void OnSelectExperiment(int index)
    {
        isSelete = true;
        Experiment chosen = currentOptions[index];
        Debug.Log($"{chosen.name} 선택됨!");

        ApplyEffect(chosen); 
    }

    // 실제 게임 데이터에 능력을 반영하는 곳
    void ApplyEffect(Experiment data)
    {
        switch (data.experimentID)
        {
            case 1:
                Debug.Log("공격력이 10 증가합니다.");
                break;
            case 2:
                Debug.Log("이동속도가 3 감소합니다.");
                break;
            case 3:
                //치명타 높음, 적 데미지 증가
                break;
            case 4:
                //적 데미지 너프, 총 데미지 너프
                break;
            case 5:
                break;
            case 6:
                // 체력 20 추가, 휴우증 1개 추가
                break;
            case 7:
                //받은 페널티 제거 및 폭주 게이지 4분의 1 증가
                break;
            // 추가적인 ID에 따른 효과들...
        }
    }
}
