using System.Collections;
using Unity.VisualScripting;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SeleteCardAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] public Image image;

    [Header("Selete")]
    private Vector2 originalPos;
    private Color originalColor;

    [Header("Card")]
    private Coroutine hoverCoroutine;
    public bool isHovering = false;
    public float duration = 0.15f;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        originalPos = rectTransform.anchoredPosition;
        originalColor = image.color;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 기존에 이동 중이던 애니메이션이 있다면 중지 (빠른 마우스 이동 시 버벅임 방지)
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
        }

        isHovering = true;
        Vector2 targetPos = new Vector2(originalPos.x, 145f); 
        hoverCoroutine = StartCoroutine(MoveCard(targetPos, duration));
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // 기존에 이동 중이던 애니메이션이 있다면 중지
        if (hoverCoroutine != null)
        {
            StopCoroutine(hoverCoroutine);
        }

        isHovering = false;        
        hoverCoroutine = StartCoroutine(MoveCard(originalPos, duration));
    }

    private IEnumerator MoveCard(Vector2 targetPos, float duration)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;
            
            // 부드러운 움직임을 위해 SmoothStep 적용
            float curve = Mathf.SmoothStep(0, 1, percent);
            
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, curve);
            yield return null;
        }

        // 시간이 다 끝나면 정확한 목표 위치로 고정
        rectTransform.anchoredPosition = targetPos;
    }

    public IEnumerator Anim(float duration)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, 150f);
        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;

            // 부드러운 움직임을 위해 SmoothStep 적용 (선택 사항)
            float curve = Mathf.SmoothStep(0, 1, percent);

            // 위치 이동
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, curve);
            
            // 알파값 조절
            image.color = Color.Lerp(startColor, endColor, curve);

            yield return null;
        }

        yield return new WaitForSeconds(1.5f);
       
        //원래대로 되돌리기
        rectTransform.anchoredPosition = originalPos;
        image.color = originalColor;
    }
}
