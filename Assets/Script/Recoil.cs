using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    [Tooltip("총을 쏠 때 올라가는 x축 회전값")]
    public float recoilX = -8.1f;
    [Tooltip("폭주(피버 100) 상태일 때 사용되는 x축 회전값")]
    public float feverRecoilX = -2.0f;

    [Tooltip("반동이 적용되는 속도")]
    public float snappiness = 10f;

    [Tooltip("원래 위치로 돌아오는 속도")]
    public float returnSpeed = 5f;

    [Header("Fire Condition")]
    [Tooltip("원래 위치로 돌아왔다고 판단할 오차 각도 (값이 작을수록 완전히 멈춰야 발사됨)")]
    public float threshold = 0.5f;
    public bool isRecoil = false;

    // 기본 rotation 값
    private readonly Vector3 baseRotation = new Vector3(-90f, 0f, 0f);

    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private Vector3 jitterOffset;

    void Start()
    {
        // 시작 시 기본 rotation으로 초기화
        currentRotation = baseRotation;
        targetRotation = baseRotation;
        transform.localEulerAngles = baseRotation;
    }

    // 현재 회전이 기본값(baseRotation) 기준으로 threshold 이내인지 확인
    public bool CanFire
    {
        get
        {
            return Mathf.Abs(currentRotation.x - baseRotation.x) <= threshold;
        }
    }

    [Header("Instability")]
    public float feverRatio = 0f;
    public bool isBerserk = false;

    public void ApplyTwitch(float intensity)
    {
        // Twitch가 너무 쌓이지 않도록 targetRotation에 점진적으로 추가
        targetRotation += new Vector3(-intensity * 0.5f, Random.Range(-intensity, intensity), 0);
    }

    public void SetJitter(float intensity)
    {
        // jitterOffset은 매 프레임 랜덤하게 튀게 함
        jitterOffset = new Vector3(
            Random.Range(-intensity, intensity),
            Random.Range(-intensity, intensity),
            Random.Range(-intensity, intensity)
        );
    }

    void Update()
    {
        // 1. 목표 회전값을 항상 기본 위치(-90, 0, 0)로 부드럽게 되돌림
        // 멀어질수록 더 빨리 돌아오도록 보정
        float dist = Vector3.Distance(targetRotation, baseRotation);
        float currentReturnSpeed = isBerserk ? returnSpeed * (1f + dist * 0.1f) : returnSpeed;
        
        targetRotation = Vector3.Lerp(targetRotation, baseRotation, currentReturnSpeed * Time.deltaTime);

        // 2. 현재 회전값을 목표 회전값으로 부드럽게 이동
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);

        // 3. 실제 오브젝트의 로컬 회전값에 적용 (지터 더함)
        transform.localEulerAngles = currentRotation + jitterOffset;

        // 지터값 감쇠
        jitterOffset = Vector3.Lerp(jitterOffset, Vector3.zero, 15f * Time.deltaTime);
    }

    public void Fire()
    {
        float recoilValue = isBerserk ? feverRecoilX : (recoilX * (1f + feverRatio));
        
        // 목표 회전값에 반동 추가
        targetRotation += new Vector3(recoilValue, Random.Range(-Mathf.Abs(recoilValue) * 0.1f, Mathf.Abs(recoilValue) * 0.1f), 0);
        
        // 폭주(피버 타임) 시에만 팔이 사라지지 않게 각도 제한
        if (isBerserk)
        {
            targetRotation.x = Mathf.Clamp(targetRotation.x, baseRotation.x - 30f, baseRotation.x + 30f);
        }
    }
}