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
    public bool isRecoil = false; // 반동 여부

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    // 외부 스크립트(예: Weapon.cs)에서도 발사 가능 여부를 알 수 있도록 만든 프로퍼티
    public bool CanFire 
    {
        get 
        { 
            // 현재 X축 회전값의 절댓값이 threshold(0.5도) 이하인지 확인
            return Mathf.Abs(currentRotation.x) <= threshold; 
        }
    }

    void Update()
    {
        // 1. 목표 회전값을 항상 원래 위치(0, 0, 0)로 부드럽게 되돌림
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);

        // 2. 현재 회전값을 목표 회전값으로 부드럽게 이동
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);

        // 3. 실제 오브젝트의 로컬 회전값에 적용
        transform.localEulerAngles = currentRotation;

        // 테스트용: 마우스 왼쪽 버튼 클릭 + 팔이 제자리로 돌아왔을 때(CanFire == true)만 발사
        if (Input.GetButtonDown("Fire1") && CanFire)
        {
            Fire();
        }
    }

    public void Fire()
    {

        // 총을 쏠 때 목표 회전값에 반동 추가
        targetRotation += new Vector3(recoilX, 0, 0);
        
        // 여기에 총알 발사, 이펙트 생성, 사운드 재생 등의 로직을 추가하시면 됩니다.
        Debug.Log("발사 완료! (반동 시작)");
    }
}
