using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AftereffectManager : MonoBehaviour
{
    //폭주 게이지
    [SerializeField] private Fever_Slider feverSlider;
    [SerializeField] private Player player;
    [SerializeField] private Move move;
    [SerializeField] private Gun gun;
    [SerializeField] private Recoil recoil;

    [Header("Aftereffect Settings")]
    [SerializeField] private float minInterval = 1f;
    [SerializeField] private float maxInterval = 15f;
    
    private float nextTriggerTime;

    private bool isBerserk = false;

    void Start()
    {
        if (feverSlider != null)
        {
            feverSlider.OnBerserkReached += EnterBerserk;
        }
        SetNextTrigger();
    }

    void Update()
    {
        float ratio = feverSlider != null ? feverSlider.CurrentFeverRatio : 0;
        
        // Recoil 등에 수치 전달
        if (recoil != null) recoil.feverRatio = ratio;

        // --- 폭주 예고 연출 (80% / 90% 단계) ---
        if (!isBerserk && berserkEffectController != null)
        {
            berserkEffectController.SetGaugeEffects(ratio);
        }

        if (isBerserk) return;

        if (ratio < 0.2f) return; // 20% 이하는 안전

        if (Time.time >= nextTriggerTime)
        {
            TriggerRandomAftereffect(ratio);
            SetNextTrigger();
        }
    }

    private void SetNextTrigger()
    {
        float ratio = feverSlider != null ? feverSlider.CurrentFeverRatio : 0;
        // 게이지가 높을수록 자주 발생
        float currentMax = Mathf.Lerp(maxInterval, minInterval, ratio);
        nextTriggerTime = Time.time + Random.Range(minInterval, currentMax);
    }

    private void TriggerRandomAftereffect(float ratio)
    {
        int effectType = Random.Range(0, 3); // 0: Movement, 1: Combat, 2: Visual/Misc

        switch (effectType)
        {
            case 0: // Movement
                if (Random.value > 0.5f)
                {
                    StartCoroutine(MovementAftereffect(ratio));
                }
                else
                {
                    Vector3 force = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * (ratio * 10f);
                    move.ApplyStagger(force);
                }
                break;
            case 1: // Combat
                StartCoroutine(CombatAftereffect(ratio));
                break;
            case 2: // Twitch / Cam
                ApplyTwitch(ratio);
                break;
        }
    }

    private IEnumerator MovementAftereffect(float ratio)
    {
        // Stumble: 잠시 속도 저하
        float originalSpeed = move.walkSpeed;
        move.walkSpeed *= 0.3f;
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f) * ratio);
        move.walkSpeed = originalSpeed;
    }

    private IEnumerator CombatAftereffect(float ratio)
    {
        // Spasm: 강제 발사 또는 장전 지연
        if (Random.value > 0.5f)
        {
            // 강제 발사 시도 (Gun 클래스에 ForceFire 추가 필요)
            gun.SendMessage("ForceFire", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            // Jamming: 잠시 발사 불가 (Gun 클래스에 Jamming 추가 필요)
            gun.SendMessage("SetJamming", true, SendMessageOptions.DontRequireReceiver);
            yield return new WaitForSeconds(Random.Range(1f, 2f) * ratio);
            gun.SendMessage("SetJamming", false, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void ApplyTwitch(float ratio)
    {
        // 시점 흔들림 (Recoil 클래스에 Twitch 추가 필요)
        recoil.SendMessage("ApplyTwitch", ratio * 10f, SendMessageOptions.DontRequireReceiver);
    }

    public void EnterBerserk()
    {
        if (isBerserk) return;
        isBerserk = true;
        StartCoroutine(BerserkRoutine());
    }

    [Header("Berserk Visuals")]
    [SerializeField] private BerserkEffectController berserkEffectController;
    [SerializeField] private float berserkDuration = 25f;

    private IEnumerator BerserkRoutine()
    {
        Debug.Log("!!! BERSERK MODE ACTIVATED !!!");
        
        // Fever Slider Pulse
        FeverSliderPulse fPulse = feverSlider.GetComponent<FeverSliderPulse>();
        if (fPulse == null) fPulse = feverSlider.gameObject.AddComponent<FeverSliderPulse>();
        fPulse.SetPulse(true);

        // --- 🎬 1. 시작 단계 (Start Phase) ---
        if (berserkEffectController != null) berserkEffectController.StartBerserkEffects(berserkDuration);
        
        // 긍정적 효과: 체력 회복 (시작 시 30% 회복)
        if (player.GetComponentInChildren<HP_Slider>() != null)
        {
            HP_Slider hp = player.GetComponentInChildren<HP_Slider>();
            hp.curHP = Mathf.Min(hp.maxHp, hp.curHP + (hp.maxHp * 0.3f));
        }

        // --- ⚔ 2. 진행 중 단계 (Active Phase) ---
        // 능력치 강화 및 무기 설정
        float originalPlayerDamageMultiplier = player.attackPowerMultiplier;
        player.attackPowerMultiplier *= 3.0f; // 공격력 3배로 상향
        gun.SendMessage("SetBerserk", true, SendMessageOptions.DontRequireReceiver);
        
        if (recoil != null) recoil.isBerserk = true; // 반동 감소 활성화

        // 부정적 효과: 입력 과민 반응 (속도 및 가속도 대폭 증가)
        float originalWalkSpeed = move.walkSpeed;
        float originalRunSpeed = move.runSpeed;
        float originalAccel = move.acceleration;
        float originalPlayerSpeed = player.speed;

        // 속도 대폭 상향 (기본의 3.5배 수준으로 강화)
        move.walkSpeed = originalWalkSpeed * 3.5f; 
        move.runSpeed = originalRunSpeed * 2.8f;
        move.acceleration *= 5.0f;
        player.speed *= 3.5f; 

        Debug.Log($"[Berserk] Power Overload: AttackMult={player.attackPowerMultiplier}, Speed={move.walkSpeed}");

        float elapsed = 0f;
        Effect effects = FindFirstObjectByType<Effect>();
        
        while (elapsed < berserkDuration)
        {
            elapsed += Time.deltaTime;
            
            // UI Timer Update
            if (berserkEffectController != null) berserkEffectController.UpdateTimer(berserkDuration - elapsed);

            // 카메라 및 조준 흔들림
            if (recoil != null)
            {
                // Active Phase 연출: 카메라 흔들림과 조준 흔들림
                recoil.SetJitter(Random.Range(2.5f, 5.5f));
                if (elapsed % 0.15f < Time.deltaTime) 
                {
                    recoil.ApplyTwitch(Random.Range(2.5f, 4.5f));
                    if (effects != null) effects.TriggerCameraShake(0.2f, 0.3f);
                }
            }

            // 폭주의 대가: 체력 지속 감소
            if (elapsed % 0.5f < Time.deltaTime)
            {
                player.TakeDamage(1.5f);
            }

            yield return null;
        }
        
        // --- ☠ 3. 종료 단계 (End Phase) ---
        if (berserkEffectController != null) berserkEffectController.EndBerserkEffects();

        // 원상 복구
        player.attackPowerMultiplier = originalPlayerDamageMultiplier;
move.walkSpeed = originalWalkSpeed;
        move.runSpeed = originalRunSpeed;
        move.acceleration = originalAccel;
        player.speed = originalPlayerSpeed;
        
        gun.SendMessage("SetBerserk", false, SendMessageOptions.DontRequireReceiver);
        if (recoil != null) recoil.isBerserk = false;
        
        if (fPulse != null) fPulse.SetPulse(false);

        isBerserk = false;
        feverSlider.ResetFever(0.5f); // 50%로 리셋
        Debug.Log("Berserk Mode Ended. Fever reset to 50%.");
    }

    #region 기존 코드 호환용
    public IEnumerator SpeedDown()
    {
        player.speed = 2f;
        yield return new WaitForSeconds(10f);
        player.speed = 5f;
    }

    public IEnumerator PainDamage()
    {
        player.attackPowerMultiplier = 2.0f;
        yield return new WaitForSeconds(10f);
        player.attackPowerMultiplier = 1.0f;
    }
#endregion
}
