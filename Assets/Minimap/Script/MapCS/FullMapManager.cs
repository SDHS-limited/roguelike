using UnityEngine;
using UnityEngine.UI;

public class FullMapManager : MonoBehaviour
{
    [Header("전체맵 UI 설정")]
    public RectTransform iconPanel; // 아이콘 생성할 FullMapImage RectTransform
    public RectTransform mapRect;   // FullMapImage RectTransform
    public Camera fullMapCamera;    // 전체맵 전용 카메라

    [Header("아이콘 프리팹")]
    public GameObject demonIconPrefab;    // 몬스터(악마) 아이콘

    [Header("플레이어 아이콘")]
    public GameObject playerIconPrefab; // 플레이어 아이콘 프리팹
    public Transform playerTransform;   // 플레이어 Transform

    private GameObject playerIcon; // 생성된 플레이어 아이콘 참조
    private RectTransform playerIconRect;

    void Start()
    {
        if (playerIconPrefab == null || playerTransform == null) return;
        if (mapRect == null) return; // 좌표 변환에 필수
        // 플레이어 아이콘 생성 (iconPanel 자식으로)
        playerIcon = Instantiate(playerIconPrefab, iconPanel != null ? iconPanel : mapRect);
        playerIconRect = playerIcon.GetComponent<RectTransform>();
        if (playerIconRect == null)
            playerIconRect = playerIcon.AddComponent<RectTransform>();
    }
    void Update()
    {
        if (playerIconRect == null || playerTransform == null || fullMapCamera == null || mapRect == null)
            return;
        Vector2 localPos = WorldToFullMapLocal(playerTransform.position);
        playerIconRect.anchoredPosition = localPos;
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
}