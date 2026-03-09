using UnityEngine;

/// <summary>
/// 미니맵 카메라가 플레이어를 따라가고, 플레이어의 방향에 맞춰 회전하는 기능
/// </summary>
public class MinimapCamera : MonoBehaviour
{
    public Transform target;                      // 추적할 대상(플레이어)
    public Vector3 offset = new Vector3(0, 21.8f, 0); // 플레이어 기준 카메라 오프셋

    void LateUpdate()
    {
        if (target == null) return; // 타겟이 null 이면, 반환

        transform.position = target.position + offset; // 위치 갱신
        transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f); // Y축 방향에 맞춰 회전
    }
}