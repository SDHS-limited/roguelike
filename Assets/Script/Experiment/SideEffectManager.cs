using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SideEffectManager : MonoBehaviour
{
    public static SideEffectManager Instance { get; private set; }

    [SerializeField] private List<SideEffect> allSideEffects;
    [SerializeField] private List<SideEffect> activeSideEffects = new List<SideEffect>();

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Move move;
    [SerializeField] private Gun gun;
    [SerializeField] private Recoil recoil;
    [SerializeField] private Fever_Slider feverSlider;
    [SerializeField] private HP_Slider hpSlider;
    [SerializeField] private GameObject hallucinationPrefab;

    public int berserkCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void IncrementBerserkCount() => berserkCount++;
    public void DecrementBerserkCount() => berserkCount = Mathf.Max(0, berserkCount - 1);

    public int SideEffectCount => activeSideEffects.Count;

    public void AddRandomSideEffect()
    {
        if (allSideEffects == null || allSideEffects.Count == 0) return;

        List<SideEffect> available = new List<SideEffect>();
        foreach (var se in allSideEffects)
        {
            if (activeSideEffects.Contains(se)) continue;

            if (se.severity == SideEffectSeverity.Minor) available.Add(se);
            else if (se.severity == SideEffectSeverity.Moderate && SideEffectCount >= 3) available.Add(se);
            else if (se.severity == SideEffectSeverity.Severe && SideEffectCount >= 6) available.Add(se);
        }

        if (available.Count > 0)
        {
            SideEffect selected = available[Random.Range(0, available.Count)];
            activeSideEffects.Add(selected);
            ApplySideEffect(selected);
        }
    }

    public void RemoveRandomSideEffect()
    {
        if (activeSideEffects.Count > 0)
        {
            int index = Random.Range(0, activeSideEffects.Count);
            SideEffect removed = activeSideEffects[index];
            RemoveSideEffect(removed);
            activeSideEffects.RemoveAt(index);
        }
    }

    private void ApplySideEffect(SideEffect se)
    {
        Debug.Log($"[SIDE EFFECT] Acquired: {se.effectName} (ID: {se.effectID})");
        switch (se.effectID)
        {
            case 101: // 손 떨림: 조준 흔들림 증가
                if (recoil != null) recoil.snappiness *= 0.6f;
                break;
            case 102: // 근육 경직: 이동속도 감소
                if (move != null) { move.walkSpeed *= 0.8f; move.runSpeed *= 0.8f; }
                break;
            case 103: // 시야 혼탁: 화면 흐림
                if (player != null) player.isVisionBlurred = true;
                break;
            case 104: // 과호흡: 연사 속도 감소
                if (gun != null) gun.fireRateMultiplier *= 0.8f;
                break;
            case 105: // 이명: 볼륨 감소
                AudioListener.volume = 0.4f;
                break;
            case 106: // 신경 과민: 피격 흔들림 증가
                if (player != null) player.hitShakeMultiplier *= 2.5f;
                break;
            case 107: // 불면: 회복 효율 감소
                if (player != null) player.healMultiplier *= 0.5f;
                break;
            case 108: // 광휘 누출: 게이지 자연 증가
                StartCoroutine(RadianceLeakRoutine());
                break;
            case 201: // 기억 혼란: 선택지 숨김
                Debug.Log("[SIDE EFFECT] Memory Confusion active: Card choices reduced.");
                break;
            case 202: // 과부하: 재장전 속도 감소
                if (gun != null) gun.reloadSpeedMultiplier *= 0.7f;
                break;
            case 203: // 신체 붕괴: 최대 체력 -20%
                if (hpSlider != null) { hpSlider.maxHp *= 0.8f; hpSlider.curHP = Mathf.Min(hpSlider.curHP, hpSlider.maxHp); }
                break;
            case 301: // 통제 불능: 게이지 획득 +50%
                break;
            case 302: // 타락 징후: 지속 체력 감소
                StartCoroutine(CorruptionRoutine());
                break;
            case 303: // 정신 분열: 가짜 적 등장
                StartCoroutine(HallucinationRoutine());
                break;
            case 304: // 실험체 한계: 랜덤 능력치 변동
                StartCoroutine(StatJitterRoutine());
                break;
        }
    }

    private IEnumerator RadianceLeakRoutine()
    {
        while (HasEffect(108))
        {
            if (feverSlider != null) feverSlider.AddFever(Time.deltaTime * 0.5f);
            yield return null;
        }
    }

    private IEnumerator CorruptionRoutine()
{
        while (HasEffect(302))
        {
            if (hpSlider != null) hpSlider.TakeDamage(1f);
            yield return new WaitForSeconds(4f);
        }
    }

    private IEnumerator HallucinationRoutine()
    {
        while (HasEffect(303))
        {
            yield return new WaitForSeconds(Random.Range(10f, 25f));
            if (hallucinationPrefab != null && player != null)
            {
                Vector3 spawnPos = player.transform.position + player.transform.forward * 8f + Random.insideUnitSphere * 4f;
                spawnPos.y = player.transform.position.y;
                GameObject fake = Instantiate(hallucinationPrefab, spawnPos, Quaternion.identity);
                Destroy(fake, 4f);
                Debug.Log("[SIDE EFFECT] Hallucination spawned!");
            }
        }
    }

    private IEnumerator StatJitterRoutine()
    {
        while (HasEffect(304))
        {
            float mult = Random.Range(0.7f, 1.3f);
            if (player != null) player.attackPowerMultiplier *= mult;
            yield return new WaitForSeconds(8f);
            if (player != null) player.attackPowerMultiplier /= mult;
            yield return new WaitForSeconds(2f);
        }
    }

    private void RemoveSideEffect(SideEffect se)
    {
        Debug.Log($"[SIDE EFFECT] Purified: {se.effectName}");
        switch (se.effectID)
        {
            case 101: if (recoil != null) recoil.snappiness /= 0.6f; break;
            case 102: if (move != null) { move.walkSpeed /= 0.8f; move.runSpeed /= 0.8f; } break;
            case 103: if (player != null) player.isVisionBlurred = false; break;
            case 104: if (gun != null) gun.fireRateMultiplier /= 0.8f; break;
            case 105: AudioListener.volume = 1.0f; break;
            case 106: if (player != null) player.hitShakeMultiplier /= 2.5f; break;
            case 107: if (player != null) player.healMultiplier /= 0.5f; break;
            case 202: if (gun != null) gun.reloadSpeedMultiplier /= 0.7f; break;
            case 203: if (hpSlider != null) hpSlider.maxHp /= 0.8f; break;
        }
    }

    public bool HasEffect(int id)
    {
        return activeSideEffects.Exists(se => se.effectID == id);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) AddRandomSideEffect();
        if (Input.GetKeyDown(KeyCode.F2)) RemoveRandomSideEffect();
        if (Input.GetKeyDown(KeyCode.F3)) { if(feverSlider) feverSlider.AddFever(100f); }
        if (Input.GetKeyDown(KeyCode.F4)) { if(feverSlider) feverSlider.ResetFever(0f); }
        
        if (Input.GetKeyDown(KeyCode.F5)) AddSpecificEffect(101);
        if (Input.GetKeyDown(KeyCode.F6)) AddSpecificEffect(102);
        if (Input.GetKeyDown(KeyCode.F7)) AddSpecificEffect(203);
        if (Input.GetKeyDown(KeyCode.F8)) AddSpecificEffect(302);
        if (Input.GetKeyDown(KeyCode.F9)) AddSpecificEffect(201);
        if (Input.GetKeyDown(KeyCode.F10)) AddSpecificEffect(303);
        if (Input.GetKeyDown(KeyCode.F11)) AddSpecificEffect(304);
        if (Input.GetKeyDown(KeyCode.F12)) {
            foreach(var s in new List<SideEffect>(activeSideEffects)) {
                RemoveSideEffect(s);
                activeSideEffects.Remove(s);
            }
        }
    }

    private void AddSpecificEffect(int id)
    {
        SideEffect se = allSideEffects.Find(x => x.effectID == id);
        if (se != null && !activeSideEffects.Contains(se))
        {
            activeSideEffects.Add(se);
            ApplySideEffect(se);
        }
    }
}
