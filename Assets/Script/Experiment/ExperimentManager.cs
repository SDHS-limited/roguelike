using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class ExperimentManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Text[] nameTexts;
    [SerializeField] Text[] desTexts;
    [SerializeField] Image[] ImageText;
   // [SerializeField] Image[] experimentImages;
    [SerializeField] Button[] selectButtons;
    [SerializeField] GameObject experiment2; // 실험창

    [Header("Data Pool")]
    [SerializeField] Experiment[] allExperiments; // 모든 능력 리스트
    [DoNotSerialize] public bool isSelete = false;

    [Header("ID")]
    [SerializeField] Bullet bullet;
    [SerializeField] Player player;
    
    [Header("Effect")]
    [SerializeField] Effect effect;
    [SerializeField] Fever_Slider fever_Slider;

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
        isSelete = true;
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
            int index = i;
            Experiment selectedData = allExperiments[indices[i]];
            currentOptions.Add(selectedData);

            nameTexts[i].text = selectedData.name;
            desTexts[i].text = selectedData.Des;

            selectButtons[i].onClick.RemoveAllListeners();

            selectButtons[i].onClick.AddListener(() =>
            {
                OnSelectExperiment(index);
            });
        }
    }

    // 능력을 선택했을 때 실행되는 함수
    public void OnSelectExperiment(int index)
    {
        Experiment chosen = currentOptions[index];
        Debug.Log($"{chosen.name} 선택됨!");

        StartCoroutine(ApplyEffect(chosen)); 
    }

    // 실제 게임 데이터에 능력을 반영하는 곳
    IEnumerator ApplyEffect(Experiment data)
    {
        switch (data.experimentID)
        {
            case 1:
                bullet.Damage += 10;
                fever_Slider.currentFever += 5;
                ShowThreeRandomExperiments();
                isSelete = false;
                break;
            case 2:
                yield return new WaitForSeconds(0.3f);

                player.speed -= 2f;
                player.damage -= 4f;
                fever_Slider.currentFever += 10;
                StartCoroutine(effect.Damage());

                ShowThreeRandomExperiments();
                isSelete = false;
                break;
            case 3:
                //치명타 높음, 적 데미지 증가

                fever_Slider.currentFever += 5;

                ShowThreeRandomExperiments();
                isSelete = false;
                break;
            case 4:
                //적 데미지 너프, 총 데미지 너프

                fever_Slider.currentFever += 5;

                ShowThreeRandomExperiments();
                isSelete = false;
                break;
            case 5:
                fever_Slider.currentFever += 5;

                ShowThreeRandomExperiments();
                isSelete = false;
                break;
            case 6:
                // 체력 20 추가, 휴우증 1개 추가
                fever_Slider.currentFever += 5;

                ShowThreeRandomExperiments();
                isSelete = false;
                break;
            case 7:
                //받은 페널티 제거 및 폭주 게이지 4분의 1 증가
                fever_Slider.currentFever += 5;

                ShowThreeRandomExperiments();
                isSelete = false;
                break;
            // 추가적인 ID에 따른 효과들...
        }
    }
}
