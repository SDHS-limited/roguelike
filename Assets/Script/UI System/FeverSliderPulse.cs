using UnityEngine;
using UnityEngine.UI;

public class FeverSliderPulse : MonoBehaviour
{
    private Image fillImage;
    private Vector3 originalScale;
    private bool isPulsing = false;
    private Color originalColor;

    void Awake()
    {
        Slider slider = GetComponent<Slider>();
        if (slider != null && slider.fillRect != null)
        {
            fillImage = slider.fillRect.GetComponent<Image>();
            if (fillImage != null) originalColor = fillImage.color;
        }
        originalScale = transform.localScale;
    }

    public void SetPulse(bool active)
    {
        isPulsing = active;
        if (!active)
        {
            transform.localScale = originalScale;
            if (fillImage != null) fillImage.color = originalColor;
        }
    }

    void Update()
    {
        if (isPulsing)
        {
            float pulse = Mathf.PingPong(Time.time * 6f, 1f);
            transform.localScale = originalScale * (1f + pulse * 0.1f);
            if (fillImage != null)
            {
                fillImage.color = Color.Lerp(originalColor, Color.white, pulse * 0.5f);
            }
        }
    }
}
