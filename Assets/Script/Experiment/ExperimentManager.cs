using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class ExperimentManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text[] nameTexts;
    [SerializeField] TMP_Text[] desTexts;
    [SerializeField] Image[] ImageText;
    [SerializeField] Button[] selectButtons;
    [SerializeField] SeleteCardAnim[] cardAnims;
    [SerializeField] GameObject experiment2; // 실험창

    [Header("Data Pool")]
    [SerializeField] Experiment[] allExperiments; // 모든 능력 리스트
    [DoNotSerialize] public bool isSelete = false;

    [Header("ID")]
    [SerializeField] Bullet bullet;
    [SerializeField] Player player;
    [SerializeField] Move move;
    
    [Header("Effect")]
    [SerializeField] Effect effect;
    [SerializeField] Fever_Slider fever_Slider;
    [SerializeField] HP_Slider hp;

    private List<Experiment> currentOptions = new List<Experiment>();
    
    [Header("Camera")]
    [SerializeField] CameraRot cameraRot;

    [HideInInspector] public ExperimentObj activeRoom; // 현재 UI를 열고 있는 방

    void Start()
    {
        if (cameraRot == null) cameraRot = FindFirstObjectByType<CameraRot>();
        ShowThreeRandomExperiments();
        if (experiment2 != null) experiment2.SetActive(false);
        isSelete = false;
    }

    void Awake() {
        Application.targetFrameRate = 60;
    }

    void ShowThreeRandomExperiments()
    {
        if (allExperiments.Length < 3) return;
        
        List<int> indices = new List<int>();
        while (indices.Count < 3)
        {
            int randomIndex = Random.Range(0, allExperiments.Length);
            if (!indices.Contains(randomIndex)) indices.Add(randomIndex);
        }

        currentOptions.Clear();

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

    public void OnSelectExperiment(int index)
    {
        if (isSelete) return; 

        Experiment chosen = currentOptions[index];
        Debug.Log($"{chosen.name} 선택됨!");

        isSelete = true; 
        StartCoroutine(cardAnims[index].Anim(1f));
        StartCoroutine(ApplyEffect(chosen)); 
    }

    IEnumerator ApplyEffect(Experiment data)
    {
        //yield return new WaitForSeconds(1.2f); // 애니메이션 대기

        if (activeRoom != null)
        {
            activeRoom.MarkAsVisited();
        }

        switch (data.experimentID)
        {
            case 1:
                player.attackPowerMultiplier += 0.5f;
                fever_Slider.AddFever(5f);
                break;
            case 2:
                yield return new WaitForSeconds(0.1f);
                move.walkSpeed -= 2f;
                player.attackPowerMultiplier -= 0.4f;
                fever_Slider.AddFever(10f);
                StartCoroutine(effect.Damage()); 
                break;
            case 3:
                fever_Slider.AddFever(5f);
                break;
            case 4:
                fever_Slider.AddFever(5f);
                break;
            case 5:
                break;
            case 6:
                hp.curHP += 20;
                fever_Slider.AddFever(5f);
                break;
            case 7:
                fever_Slider.AddFever(25f);
                break;
            case 8:
                player.attackPowerMultiplier += 2.0f;
                fever_Slider.AddFever(30f);
                break;
        }

        if (experiment2 != null) experiment2.SetActive(false);
        if (cameraRot != null) cameraRot.isUIOpen = false;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        ShowThreeRandomExperiments(); 

        isSelete = false;
        activeRoom = null;
    }
}
