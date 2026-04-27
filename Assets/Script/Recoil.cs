using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    [Tooltip("총을 쏠 때 올라가는 x축 회전값")]
    public float recoilX = -8.1f;

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

    void Update()
    {
        // 1. 목표 회전값을 항상 기본 위치(-90, 0, 0)로 부드럽게 되돌림
        targetRotation = Vector3.Lerp(targetRotation, baseRotation, returnSpeed * Time.deltaTime);

        // 2. 현재 회전값을 목표 회전값으로 부드럽게 이동
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);

        // 3. 실제 오브젝트의 로컬 회전값에 적용
        transform.localEulerAngles = currentRotation;
    }

    public void Fire()
    {
        // 총을 쏠 때 목표 회전값에 반동 추가
        targetRotation += new Vector3(recoilX, 0, 0);
    }
}