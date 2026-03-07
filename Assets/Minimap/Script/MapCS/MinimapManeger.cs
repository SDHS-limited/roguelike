using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 미니맵 상의 오브젝트 아이콘, 플레이어 아이콘 관리
/// </summary>
public class MinimapManeger : MonoBehaviour
{
    [Header("미니맵 설정")]
    public RectTransform iconPanel;          // 아이콘 생성될 미니맵 패널
    public RectTransform minimapRect;        // 미니맵 RectTransform
    public Camera minimapCamera;             // 미니맵 카메라

    [Header("아이콘 프리팹")]
    public GameObject playerIconPrefab;      // 플레이어 아이콘
    public GameObject demonIconPrefab;         // 몬스터(악마) 아이콘

    [Header("플레이어")]
    public Transform playerTransform;        // 플레이어 Transform
    private GameObject playerIcon;           // 플레이어 아이콘 인스턴스
    private RectTransform playerIconRect;   // 캐시 (매 프레임 GetComponent 방지)

    private Dictionary<Enemy, RectTransform> monsterIcons = new Dictionary<Enemy, RectTransform>();

    void Start()
    {
        if (playerIconPrefab != null && iconPanel != null)
        {
            playerIcon = Instantiate(playerIconPrefab, iconPanel);
            playerIconRect = playerIcon.GetComponent<RectTransform>();
            if (playerIconRect != null)
                playerIconRect.anchoredPosition = Vector2.zero;
        }

        // 씬에 이미 존재하는 몬스터 일괄 등록
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            RegisterMonster(enemy);
    }

    void Update()
    {
        if (playerIconRect != null && playerTransform != null)
            playerIconRect.localEulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);

        // 몬스터 아이콘 위치 갱신 (월드 → 미니맵 로컬 좌표)
        if (minimapCamera == null || iconPanel == null) return;
        foreach (var kvp in monsterIcons)
        {
            Enemy enemy = kvp.Key;
            RectTransform iconRt = kvp.Value;
            if (enemy == null || iconRt == null) continue;
            iconRt.anchoredPosition = WorldToMinimapLocal(enemy.transform.position);
        }
    }

    /// <summary> 월드 좌표 → 미니맵 iconPanel 로컬 좌표 </summary>
    public Vector2 WorldToMinimapLocal(Vector3 worldPos)
    {
        Vector3 viewportPos = minimapCamera.WorldToViewportPoint(worldPos);
        if (viewportPos.z < 0)
        {
            viewportPos.x = viewportPos.x < 0.5f ? 0 : 1;
            viewportPos.y = 0.5f;
        }
        Rect rect = iconPanel.rect;
        float x = (viewportPos.x - 0.5f) * rect.width;
        float y = (viewportPos.y - 0.5f) * rect.height;
        return new Vector2(x, y);
    }

    /// <summary> 몬스터 등록 (스폰 시 또는 씬 로드 시 호출) </summary>
    public void RegisterMonster(Enemy enemy)
    {
        if (enemy == null || demonIconPrefab == null || iconPanel == null) return;
        if (monsterIcons.ContainsKey(enemy)) return;

        GameObject icon = Instantiate(demonIconPrefab, iconPanel);
        RectTransform rt = icon.GetComponent<RectTransform>();
        if (rt == null) rt = icon.AddComponent<RectTransform>();
        rt.anchoredPosition = WorldToMinimapLocal(enemy.transform.position);
        monsterIcons.Add(enemy, rt);
    }

    /// <summary> 몬스터 해제 (사망 시 호출) </summary>
    public void UnregisterMonster(Enemy enemy)
    {
        if (enemy == null || !monsterIcons.TryGetValue(enemy, out RectTransform icon))
            return;
        monsterIcons.Remove(enemy);
        if (icon != null && icon.gameObject != null)
            Destroy(icon.gameObject);
    }
}