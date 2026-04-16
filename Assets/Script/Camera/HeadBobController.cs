using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [Header("설정")]
    public float walkingSpeed = 14f;  // 걷기 속도 (주파수)
    public float bobAmount = 0.05f;   // 흔들림 강도 (진폭)

    [Header("참조")]
    public Transform cameraTransform; // 흔들릴 카메라 오브젝트

    private float defaultPosY = 0;
    private float timer = 0;

    void Start()
    {
        // 카메라의 초기 로컬 Y 위치 저장
        defaultPosY = cameraTransform.localPosition.y;
    }

    void Update()
    {
        // 1. 캐릭터가 움직이고 있는지 확인 (예시: Input 사용 시)
        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;

        if (isMoving)
        {
            // 2. 타이머 계산 (HTML의 t 변수와 동일한 역할)
            timer += Time.deltaTime * walkingSpeed;

            // 3. 삼각함수를 이용한 오프셋 계산 (제공해주신 소스의 bobY 로직)
            float newY = defaultPosY + Mathf.Sin(timer) * bobAmount;

            // 4. 카메라 위치 적용
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                newY,
                cameraTransform.localPosition.z
            );
        }
        else
        {
            // 5. 멈췄을 때 즉시 제자리로 (Instant Return)
            timer = 0;
            cameraTransform.localPosition = new Vector3(
                cameraTransform.localPosition.x,
                0.289f,
                cameraTransform.localPosition.z
            );
        }
    }
}
