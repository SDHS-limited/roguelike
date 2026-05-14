using System.Collections;
using UnityEngine;

public class AftereffectManager : MonoBehaviour
{
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

    private IEnumerator BerserkRoutine()
    {
        Debug.Log("BERSERK MODE ACTIVATED!");
        
        // 1. 능력치 대폭 강화 (플레이어 데미지 등)
        float originalPlayerDamage = player.damage;
        player.damage *= 2f;
        
        // 2. 무한 탄약 및 자동 발사 설정
        gun.SendMessage("SetBerserk", true, SendMessageOptions.DontRequireReceiver);

        float elapsed = 0f;
        float duration = 10f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            // 시점 강제 흔들림
            ApplyTwitch(2f);
            
            // 체력 조금씩 감소 (폭주의 대가)
            if (elapsed % 1.0f < Time.deltaTime)
            {
                // Player.cs에 TakeDamage가 없을 수 있으므로 HP_Slider 등을 직접 참조하는 것이 안전할 수 있음
                // 하지만 기존 Player.cs를 보면 hp.TakeDamage(1)를 호출하는 부분이 있음.
                player.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
            }

            yield return null;
        }
        
        // 원상 복구
        player.damage = originalPlayerDamage;
        gun.SendMessage("SetBerserk", false, SendMessageOptions.DontRequireReceiver);
        
        isBerserk = false;
        feverSlider.ResetFever(0.5f); // 50%로 리셋
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
        player.damage = 20;
        yield return new WaitForSeconds(10f);
        player.damage = 10;
    }
    #endregion
}
