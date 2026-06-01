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

        // Higher severity side effects appear as count increases
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
            Debug.Log($"Acquired Side Effect: {selected.effectName}");
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
            Debug.Log($"Removed Side Effect: {removed.effectName}");
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
                if (move != null)
                {
                    move.walkSpeed *= 0.8f;
                    move.runSpeed *= 0.8f;
                }
                break;
            case 202: // 과부하: 재장전 속도 감소
                if (gun != null) gun.reloadSpeedMultiplier *= 0.7f;
                break;
            case 203: // 신체 붕괴: 최대 체력 -20%
                if (hpSlider != null)
                {
                    hpSlider.maxHp *= 0.8f;
                    hpSlider.curHP = Mathf.Min(hpSlider.curHP, hpSlider.maxHp);
                }
                break;
            case 302: // 타락 징후: 지속 체력 감소
                StartCoroutine(CorruptionRoutine());
                break;
        }
    }

    private IEnumerator CorruptionRoutine()
    {
        while (HasEffect(302))
        {
            if (hpSlider != null) hpSlider.TakeDamage(0.5f);
            yield return new WaitForSeconds(3f);
        }
    }

    private void RemoveSideEffect(SideEffect se)
    {
        Debug.Log($"[SIDE EFFECT] Purified: {se.effectName}");
        switch (se.effectID)
        {
            case 101:
                if (recoil != null) recoil.snappiness /= 0.6f;
                break;
            case 102:
                if (move != null)
                {
                    move.walkSpeed /= 0.8f;
                    move.runSpeed /= 0.8f;
                }
                break;
            case 202:
                if (gun != null) gun.reloadSpeedMultiplier /= 0.7f;
                break;
            case 203:
                if (hpSlider != null) hpSlider.maxHp /= 0.8f;
                break;
        }
    }

    public bool HasEffect(int id)
    {
        return activeSideEffects.Exists(se => se.effectID == id);
    }

    private void Update()
    {
        // Testing shortcuts
        if (Input.GetKeyDown(KeyCode.F1)) AddRandomSideEffect();
        if (Input.GetKeyDown(KeyCode.F2)) RemoveRandomSideEffect();
        if (Input.GetKeyDown(KeyCode.F3)) { berserkCount++; Debug.Log($"Berserk Count: {berserkCount}"); }
        if (Input.GetKeyDown(KeyCode.F4)) { DecrementBerserkCount(); Debug.Log($"Berserk Count: {berserkCount}"); }
        
        // Manual specific effect tests
        if (Input.GetKeyDown(KeyCode.F5)) AddSpecificEffect(101);
        if (Input.GetKeyDown(KeyCode.F6)) AddSpecificEffect(102);
        if (Input.GetKeyDown(KeyCode.F7)) AddSpecificEffect(203);
        if (Input.GetKeyDown(KeyCode.F8)) AddSpecificEffect(302);
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
