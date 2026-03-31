using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SeleteCardAnim : MonoBehaviour
{
    [SerializeField] public RectTransform rectTransform;
    [SerializeField] public Image image;

    private Vector2 originalPos;
    private Color originalColor;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        originalPos = rectTransform.anchoredPosition;
        originalColor = image.color;
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
