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

    private Dictionary<Enemy, RectTransform> monsterIcons = new Dictionary<Enemy, RectTransform>();

    void Start()
    {
        // 중앙
        playerIcon = Instantiate(playerIconPrefab, iconPanel);
        playerIcon.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; 

        //// 기존 몬스터들 등록
        //foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        //{
        //    RegisterMonster(enemy);
        //}
    }

    void Update()
    {
        if (playerIcon == null || playerTransform == null) return;

        // 방향 표시 (선택): 아이콘이 플레이어가 보는 방향을 가리키게
        RectTransform rt = playerIcon.GetComponent<RectTransform>();
        rt.localEulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);

        //foreach (var kvp in monsterIcons)
        //{
        //    Enemy enemy = kvp.Key;
        //    RectTransform iconRt = kvp.Value;
        //    if (enemy == null || iconRt == null)
        //        continue;
        //    Vector2 localPos = WorldToMinimapLocal(enemy.transform.position);
        //    iconRt.anchoredPosition = localPos;
        //}
    }

    ///// <summary>
    ///// 월드 좌표 → 미니맵 iconPanel 로컬 좌표
    ///// </summary>
    //Vector2 WorldToMinimapLocal(Vector3 worldPos)
    //{
    //    Vector3 viewportPos = minimapCamera.WorldToViewportPoint(worldPos);
    //    // 카메라 뒤에 있으면 화면 밖으로 보냄 (또는 숨김)
    //    if (viewportPos.z < 0)
    //    {
    //        viewportPos.x = viewportPos.x < 0.5f ? 0 : 1;
    //        viewportPos.y = 0.5f;
    //    }
    //    // 뷰포트(0~1) → RectTransform 로컬 좌표
    //    Rect rect = iconPanel.rect;
    //    float x = (viewportPos.x - 0.5f) * rect.width;
    //    float y = (viewportPos.y - 0.5f) * rect.height;
    //    return new Vector2(x, y);
    //}
    //
    ///// <summary>
    ///// 새 몬스터 등록 (스폰 시 호출)
    ///// </summary>
    //public void RegisterMonster(Enemy enemy)
    //{
    //    if (enemy == null || demonIconPrefab == null || monsterIcons.ContainsKey(enemy))
    //        return;
    //    GameObject icon = Instantiate(demonIconPrefab, iconPanel);
    //    RectTransform rt = icon.GetComponent<RectTransform>();
    //    rt.anchoredPosition = WorldToMinimapLocal(enemy.transform.position);
    //    monsterIcons.Add(enemy, rt);
    //}
    //
    ///// <summary>
    ///// 몬스터 제거 (사망 시 호출)
    ///// </summary>
    //public void UnregisterMonster(Enemy enemy)
    //{
    //    if (!monsterIcons.TryGetValue(enemy, out RectTransform icon))
    //        return;
    //    monsterIcons.Remove(enemy);
    //    if (icon != null)
    //        Destroy(icon.gameObject);
    //}
}