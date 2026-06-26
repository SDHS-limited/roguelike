using System.Collections;
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
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
        isHovering = true;
        Vector2 targetPos = new Vector2(originalPos.x, 145f); 
        hoverCoroutine = StartCoroutine(MoveCard(targetPos, duration));
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverCoroutine != null) StopCoroutine(hoverCoroutine);
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
            float curve = Mathf.SmoothStep(0, 1, percent);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, curve);
            yield return null;
        }
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
            float curve = Mathf.SmoothStep(0, 1, percent);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, curve);
            image.color = Color.Lerp(startColor, endColor, curve);
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);
       
        rectTransform.anchoredPosition = originalPos;
        image.color = originalColor;
    }
}
