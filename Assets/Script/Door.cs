using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("설정")]
    public float openAngle = 90f;   // 열리는 각도
    public float openSpeed = 2f;    // 열리는 속도

    private bool isLeftOpen;        // 내부적으로 방향을 판단할 변수
    private bool isOpening = false;
    private Quaternion targetRotation;
    private Quaternion closedRotation;

    void Start()
    {
        // 처음 닫힌 상태 저장
        closedRotation = transform.rotation;
    }

    void Update()
    {
        if (isOpening)
        {
            // 부드럽게 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 핵심 로직: 플레이어의 x값과 문의 x값을 비교
            // 플레이어가 문보다 왼쪽에 있으면(x값이 작으면) true, 아니면 false
            isLeftOpen = other.transform.position.x < transform.position.x;

            // 방향(isLeftOpen)에 따라 목표 회전값 계산
            // 플레이어가 왼쪽(isLeftOpen=true)에 있으면 오른쪽(양수 각도)으로 개방
            float angle = isLeftOpen ? openAngle : -openAngle;
            targetRotation = Quaternion.Euler(0, angle, 0) * closedRotation;

            isOpening = true;

            Debug.Log($"플레이어 위치: {other.transform.position.x}, 문 위치: {transform.position.x}");
            Debug.Log(isLeftOpen ? "플레이어가 왼쪽에서 진입하여 오른쪽으로 엽니다." : "플레이어가 오른쪽에서 진입하여 왼쪽으로 엽니다.");
        }
    }
}
