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
    [SerializeField] Gun gun;
    [SerializeField] Recoil recoil;

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
        
        int currentSideEffects = SideEffectManager.Instance != null ? SideEffectManager.Instance.SideEffectCount : 0;
        List<Experiment> availablePool = new List<Experiment>();
        foreach (var exp in allExperiments)
        {
            if (exp.requiredSideEffects <= currentSideEffects)
            {
                availablePool.Add(exp);
            }
        }

        if (availablePool.Count < 3) availablePool = new List<Experiment>(allExperiments); // Fallback

        List<int> indices = new List<int>();
        while (indices.Count < 3)
        {
            int randomIndex = Random.Range(0, availablePool.Count);
            if (!indices.Contains(randomIndex)) indices.Add(randomIndex);
        }

        currentOptions.Clear();

        for (int i = 0; i < 3; i++)
        {
            int index = i;
            Experiment selectedData = availablePool[indices[i]];
            currentOptions.Add(selectedData);
                
            nameTexts[i].text = selectedData.name;
            // 설명 제외: 버프, 너프, ID만 표시
            desTexts[i].text = $"{selectedData.Buff}\n{selectedData.Nerf}\nID: {selectedData.experimentID}";

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

        // Add a side effect when selecting an experiment (as requested)
        if (SideEffectManager.Instance != null) SideEffectManager.Instance.AddRandomSideEffect();

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
            case 0: // 날개 링 파손 테스트
                if (gun != null) gun.fireRateMultiplier += 0.2f;
                move.walkSpeed *= 0.85f;
                move.runSpeed *= 0.85f;
                if (fever_Slider != null) fever_Slider.AddFever(20f);
                break;
            case 1: // 천사의 근력 증폭
                player.attackPowerMultiplier += 0.2f;
                if (fever_Slider != null) fever_Slider.AddFever(15f);
                break;
            case 2: // 재생 인자
                player.healOnKillPercentage += 0.06f;
                move.walkSpeed *= 0.9f;
                move.runSpeed *= 0.9f;
                break;
            case 3: // 혈액 가속
                if (gun != null) gun.fireRateMultiplier += 0.05f;
                hp.maxHp -= 10f;
                hp.curHP = Mathf.Min(hp.curHP, hp.maxHp);
                break;
            case 4: // 신경 강화
                player.criticalChance += 0.1f;
                if (fever_Slider != null) fever_Slider.AddFever(5f); 
                break;
            case 6: // 반응 속도 증폭
                move.walkSpeed *= 1.15f;
                move.runSpeed *= 1.15f;
                if (recoil != null) recoil.snappiness *= 0.8f;
                break;
            case 7: // 천사의 분노 유발
                player.attackPowerMultiplier += 0.3f;
                StartCoroutine(FeverOverTime(5f, 10f)); 
                break;
            case 8: // 생존 본능 자극
                player.attackPowerMultiplier += 0.1f; 
                break;
            case 9: // 불법 약물 강제 주입
                player.criticalChance += 0.15f;
                if (SideEffectManager.Instance != null) {
                    SideEffectManager.Instance.AddRandomSideEffect();
                    SideEffectManager.Instance.AddRandomSideEffect();
                }
                break;
            case 10: // 불사의 혈청
                player.canReviveOnce = true;
                break;
            case 11: // 신성 폭발
                player.attackPowerMultiplier += 0.15f;
                AftereffectManager am11 = FindFirstObjectByType<AftereffectManager>();
                if (am11 != null) am11.hasSacredExplosion = true;
                break;
            case 13: // 통제 해제: 즉시 폭주
                if (fever_Slider != null) fever_Slider.AddFever(100f);
                AftereffectManager am13 = FindFirstObjectByType<AftereffectManager>();
                if (am13 != null) am13.extraSideEffectsOnEnd = 3;
                break;
}

        if (experiment2 != null) experiment2.SetActive(false);
        if (cameraRot != null) cameraRot.isUIOpen = false;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        ShowThreeRandomExperiments(); 

        isSelete = false;
        activeRoom = null;
        yield return null;
        }

        private IEnumerator FeverOverTime(float duration, float totalAmount)
        {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float step = (totalAmount / duration) * Time.deltaTime;
            if (fever_Slider != null) fever_Slider.AddFever(step);
            elapsed += Time.deltaTime;
            yield return null;
        }
        }
        }
