using UnityEngine;

/// <summary>
/// M 키 입력으로 전체맵 열기/닫기, 미니맵 표시 토글
/// </summary>
public class FullMapToggle : MonoBehaviour
{
    public GameObject fullMapCanvas;   // 전체맵 Canvas

    public MonoBehaviour[] controlScripts; // CharacterMove, MouseMove를 담을 배열

    private bool isMapOpen = false; // 전체맵 열림 여부

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isMapOpen = !isMapOpen;               // 상태 반전
            fullMapCanvas.SetActive(isMapOpen);       // 전체맵 Canvas 활성/비활성
        }
    }
}