using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    [Header("UI Overlays")]
    [SerializeField] private Image damageImage;
    [SerializeField] private Image crackedOverlay;
    [SerializeField] private Image glitchOverlay;

    [Header("Components")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private Fever_Slider feverSlider;

    private Coroutine hitStopCoroutine;
    private float currentFeverRatio;

    void Start()
    {
        if (feverSlider == null) feverSlider = Object.FindFirstObjectByType<Fever_Slider>();
        
        if (feverSlider != null)
        {
            feverSlider.OnFeverChanged += UpdateFeverVisuals;
        }

        // 초기화
        if (crackedOverlay != null) crackedOverlay.color = new Color(1, 0.3f, 0.3f, 0);
        if (glitchOverlay != null) glitchOverlay.color = new Color(1, 0, 0, 0);
    }

    void OnDestroy()
    {
        if (feverSlider != null)
        {
            feverSlider.OnFeverChanged -= UpdateFeverVisuals;
        }
    }

    void Update()
    {
        // 폭주 상태에서 미세한 카메라 흔들림 및 글리치 연출
        if (currentFeverRatio > 0.7f)
        {
            float intensity = (currentFeverRatio - 0.7f) / 0.3f; // 0 to 1
            
            if (glitchOverlay != null)
            {
                float glitchAlpha = Mathf.PingPong(Time.time * 10f * intensity, 0.15f * intensity);
                glitchOverlay.color = new Color(1, 0, 0, glitchAlpha);
            }

            // 지속적인 미세 흔들림
            if (Random.value < 0.1f * intensity)
            {
                TriggerCameraShake(0.1f, 0.05f * intensity);
            }
        }
        else
        {
            if (glitchOverlay != null && glitchOverlay.color.a > 0)
            {
                glitchOverlay.color = new Color(1, 0, 0, 0);
            }
        }
    }

    public void UpdateFeverVisuals(float ratio)
    {
        currentFeverRatio = ratio;

        // 1. 화면 균열 효과 (Fever 50% 이상부터 서서히 등장)
        if (crackedOverlay != null)
        {
            float crackAlpha = Mathf.Clamp01((ratio - 0.5f) / 0.5f);
            crackedOverlay.color = new Color(1, 0.3f, 0.3f, crackAlpha * 0.7f);
        }
    }

    public void TriggerHitStop(float duration = 0.05f)
    {
        if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
        hitStopCoroutine = StartCoroutine(HitStopRoutine(duration));
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        hitStopCoroutine = null;
    }

    public void TriggerCameraShake(float duration, float magnitude)
    {
        if (cameraShake != null) StartCoroutine(cameraShake.Shake(duration, magnitude));
    }

    public IEnumerator Damage()
    {
        if (damageImage == null) yield break;
        
        Color c = Color.red;
        c.a = 0.3f;
        damageImage.color = c;

        yield return new WaitForSeconds(0.4f);

        c.a = 0f;
        damageImage.color = c;
    }

    public IEnumerator Heal()
    {
        if (damageImage == null) yield break;

        Color c = Color.green;
        c.a = 0.3f;
        damageImage.color = c;

        yield return new WaitForSeconds(0.4f);

        c.a = 0f;
        damageImage.color = c;
    }

    public void TriggerPurification()
    {
        StartCoroutine(PurificationRoutine());
    }

    private IEnumerator PurificationRoutine()
    {
        if (damageImage == null) yield break;

        Color c = Color.red; 
        c.a = 0.4f;
        damageImage.color = c;

        float elapsed = 0f;
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0.4f, 0f, elapsed / 0.2f);
            damageImage.color = c;
            yield return null;
        }
    }
}

