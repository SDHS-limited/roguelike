using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Berserk Visuals")]
    [SerializeField] private Image berserkOverlay; // Assign a red/glitchy UI Image
    [SerializeField] private float berserkDuration = 25f;

    private IEnumerator BerserkRoutine()
    {
        Debug.Log("!!! BERSERK MODE ACTIVATED !!!");
        
        // 1. 긍정적 효과: 체력 회복 (시작 시 30% 회복)
        if (player.GetComponentInChildren<HP_Slider>() != null)
        {
            HP_Slider hp = player.GetComponentInChildren<HP_Slider>();
            hp.curHP = Mathf.Min(hp.maxHp, hp.curHP + (hp.maxHp * 0.3f));
        }
        
        // 2. 능력치 강화 및 무기 설정
        float originalPlayerDamage = player.damage;
        player.damage *= 2.5f; // 공격력 대폭 증가
        gun.SendMessage("SetBerserk", true, SendMessageOptions.DontRequireReceiver);

        // 3. 부정적 효과: 입력 과민 반응 (속도 및 가속도 대폭 증가)
        float originalWalkSpeed = move.walkSpeed;
        float originalAccel = move.acceleration;
        move.walkSpeed *= 1.8f;
        move.acceleration *= 3.0f; // 제어가 힘들 정도로 가속됨

        // 4. 시각 왜곡 (오버레이 활성화)
        if (berserkOverlay != null) berserkOverlay.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < berserkDuration)
        {
            elapsed += Time.deltaTime;
            
            // 부정적 효과: 조준 불안정 (심한 경련이 아닌 '진동' 느낌으로 변경)
            if (recoil != null)
            {
                recoil.SetJitter(Random.Range(1f, 3f));
                // Twitch는 간헐적으로만 발생 (팔이 하늘로 솟구치는 것 방지)
                if (elapsed % 0.5f < Time.deltaTime) 
                {
                    recoil.ApplyTwitch(Random.Range(1f, 2f));
                }
            }
            
            // 시각 왜곡 연출 (심장 박동처럼 오버레이 투명도 조절)
            if (berserkOverlay != null)
            {
                float alpha = 0.2f + Mathf.PingPong(Time.time * 2f, 0.3f);
                Color c = berserkOverlay.color;
                c.a = alpha;
                berserkOverlay.color = c;
            }

            // 폭주의 대가: 체력 지속 감소
            if (elapsed % 0.5f < Time.deltaTime)
            {
                player.TakeDamage(1f);
            }

            yield return null;
        }
        
        // 원상 복구
        player.damage = originalPlayerDamage;
        move.walkSpeed = originalWalkSpeed;
        move.acceleration = originalAccel;
        gun.SendMessage("SetBerserk", false, SendMessageOptions.DontRequireReceiver);
        if (recoil != null) recoil.isBerserk = false;
        
        if (berserkOverlay != null) berserkOverlay.gameObject.SetActive(false);
        
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
        player.damage = 20;
        yield return new WaitForSeconds(10f);
        player.damage = 10;
    }
    #endregion
}
