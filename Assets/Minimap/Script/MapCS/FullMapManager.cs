using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullMapManager : MonoBehaviour
{
    [Header("전체맵 UI 설정")]
    public RectTransform iconPanel; // 아이콘 생성할 FullMapImage RectTransform
    public RectTransform mapRect;   // FullMapImage RectTransform
    public Camera fullMapCamera;    // 전체맵 전용 카메라

    [Header("아이콘 프리팹")]
    public GameObject demonIconPrefab;    // 악마(몬스터) 아이콘

    [Header("플레이어 아이콘")]
    public GameObject playerIconPrefab; // 플레이어 아이콘 프리팹
    public Transform playerTransform;   // 플레이어 Transform

    private GameObject playerIcon; // 생성된 플레이어 아이콘 참조
    private RectTransform playerIconRect;

    private Dictionary<Enemy, RectTransform> monsterIcons = new Dictionary<Enemy, RectTransform>();

    void Start()
    {
        if (playerIconPrefab == null || playerTransform == null) return;
        if (mapRect == null) return; // 좌표 변환에 필수

        // 플레이어 아이콘 생성 (iconPanel 자식으로)
        RectTransform parent = iconPanel != null ? iconPanel : mapRect;
        playerIcon = Instantiate(playerIconPrefab, parent);
        playerIconRect = playerIcon.GetComponent<RectTransform>();
        if (playerIconRect == null)
            playerIconRect = playerIcon.AddComponent<RectTransform>();

        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            RegisterMonster(enemy);
    }

    void Update()
    {
        if (playerIconRect == null || playerTransform == null || fullMapCamera == null || mapRect == null)
            return;
        playerIconRect.anchoredPosition = WorldToFullMapLocal(playerTransform.position);

        foreach (var kvp in monsterIcons)
        {
            Enemy enemy = kvp.Key;
            RectTransform iconRt = kvp.Value;
            if (enemy == null || iconRt == null) continue;
            iconRt.anchoredPosition = WorldToFullMapLocal(enemy.transform.position);
        }
    }
    /// <summary>
    /// 월드 좌표 → 전체맵 UI( mapRect ) 로컬 좌표
    /// </summary>
    Vector2 WorldToFullMapLocal(Vector3 worldPos)
    {
        Vector3 viewport = fullMapCamera.WorldToViewportPoint(worldPos);
        // 카메라 뒤에 있으면 맵 밖으로 (선택)
        if (viewport.z < 0)
        {
            viewport.x = viewport.x < 0.5f ? 0 : 1;
            viewport.y = 0.5f;
        }
        Rect rect = mapRect.rect;
        float x = (viewport.x - 0.5f) * rect.width;
        float y = (viewport.y - 0.5f) * rect.height;
        return new Vector2(x, y);
    }

    /// <summary> 몬스터 등록 (스폰 시 또는 씬 로드 시 호출) </summary>
    public void RegisterMonster(Enemy enemy)
    {
        if (enemy == null || demonIconPrefab == null || mapRect == null) return;
        RectTransform parent = iconPanel != null ? iconPanel : mapRect;
        if (monsterIcons.ContainsKey(enemy)) return;

        GameObject icon = Instantiate(demonIconPrefab, parent);
        RectTransform rt = icon.GetComponent<RectTransform>();
        if (rt == null) rt = icon.AddComponent<RectTransform>();
        rt.anchoredPosition = WorldToFullMapLocal(enemy.transform.position);
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