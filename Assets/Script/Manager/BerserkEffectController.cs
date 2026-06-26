using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class BerserkEffectController : MonoBehaviour
{
    [Header("Post Processing")]
    [SerializeField] private PostProcessVolume volume;
    private ColorGrading colorGrading;
    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private LensDistortion lensDistortion;

    [Header("UI Elements")]
    [SerializeField] private GameObject warningUI;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private Image redOverlay;
    [SerializeField] private Image flashOverlay;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text timerText;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip entryClip;
    [SerializeField] private AudioClip heartbeatClip;
    [SerializeField] private AudioClip breathingClip;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    private float originalFOV;
    private float targetFOV; 

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) mainCamera = Object.FindFirstObjectByType<Camera>();
        }

        if (volume == null) volume = GetComponent<PostProcessVolume>();
        if (volume == null) volume = Object.FindFirstObjectByType<PostProcessVolume>();

        if (volume != null && volume.profile != null)
        {
            volume.profile.TryGetSettings(out colorGrading);
            volume.profile.TryGetSettings(out vignette);
            volume.profile.TryGetSettings(out chromaticAberration);
            volume.profile.TryGetSettings(out lensDistortion);
        }

        if (mainCamera != null)
        {
            originalFOV = mainCamera.fieldOfView;
            targetFOV = originalFOV + 20f; // 속도감을 위해 FOV를 조금 더 과감하게 넓힘 (+15 -> +20)
        }

        if (warningUI != null) warningUI.SetActive(false);
        if (flashOverlay != null) flashOverlay.gameObject.SetActive(false);
    }

    public void SetGaugeEffects(float ratio)
    {
        // 80%~100% 게이지 차오를 때의 전조 증상
        if (ratio >= 0.8f)
        {
            if (vignette != null)
            {
                vignette.intensity.overrideState = true;
                vignette.color.overrideState = true;
                float v = Mathf.InverseLerp(0.8f, 1.0f, ratio) * 0.45f;
                vignette.intensity.value = v;
                vignette.color.value = new Color(0.6f, 0f, 0f); // 피가 쏠리는 듯한 검붉은색 (수정됨)
            }
        }

        if (ratio >= 0.9f)
        {
            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.overrideState = true;
                chromaticAberration.intensity.value = Mathf.InverseLerp(0.9f, 1.0f, ratio) * 0.8f; // 어지러움 강화
            }
            
            if (vignette != null)
            {
                float pulse = Mathf.PingPong(Time.time * 3f, 0.15f); // 심장 박동을 조금 더 빠르고 거칠게
                vignette.intensity.value += pulse;
            }
        }
    }

    public void StartBerserkEffects(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(StartEffectsRoutine());
    }

    public void EndBerserkEffects()
    {
        StopAllCoroutines();
        StartCoroutine(EndEffectsRoutine());
    }

    public void UpdateTimer(float remaining)
    {
        if (timerText != null)
        {
            timerText.text = $"TIME: {remaining:F1}s"; // 스크린샷과 동일하게 텍스트 수정
        }
    }

    private IEnumerator StartEffectsRoutine()
    {
        Debug.Log("[Berserk] Starting Blood Overdrive Presentation");

        // 1. 강렬한 붉은 번쩍임 (0.1s)
        if (flashOverlay != null)
        {
            flashOverlay.gameObject.SetActive(true);
            flashOverlay.color = new Color(0.8f, 0f, 0f, 0.8f); // 진한 피색 번쩍임
            yield return new WaitForSeconds(0.1f);
        }

        // 2. 진입 시 하얀 섬광 (시야 마비 효과)
        if (flashOverlay != null)
        {
            flashOverlay.color = Color.white;
        }

        if (audioSource != null && entryClip != null)
        {
            audioSource.PlayOneShot(entryClip);
        }

        if (warningUI != null)
        {
            warningUI.SetActive(true);
            if (warningText != null)
            {
                // 군사적이고 미친 컨셉에 맞는 문구로 수정
                //warningText.text = "WARNING\n<size=80%>ADRENALINE OVERLOAD</size>"; 
            }
        }

        // Flash 페이드 아웃
        float flashDuration = 0.3f;
        float elapsedFlash = 0f;
        while (elapsedFlash < flashDuration)
        {
            elapsedFlash += Time.deltaTime;
            if (flashOverlay != null) flashOverlay.color = new Color(1, 1, 1, 1 - (elapsedFlash / flashDuration));
            yield return null;
        }
        if (flashOverlay != null) flashOverlay.gameObject.SetActive(false);

        // 심장 박동 사운드 시작
        if (audioSource != null && heartbeatClip != null)
        {
            audioSource.clip = heartbeatClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 3. 포스트 프로세싱 활성화 (붉고 대비가 강한 화면)
        float elapsed = 0f;
        float transitionDuration = 0.4f; 
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            if (colorGrading != null)
            {
                colorGrading.postExposure.overrideState = true;
                colorGrading.saturation.overrideState = true;
                colorGrading.colorFilter.overrideState = true;
                colorGrading.contrast.overrideState = true; // 대비 추가

                colorGrading.postExposure.value = Mathf.Lerp(0f, 0.8f, t);
                colorGrading.saturation.value = Mathf.Lerp(0f, 30f, t); // 색감을 더 미친듯이 쨍하게
                colorGrading.colorFilter.value = Color.Lerp(Color.white, new Color(1f, 0.6f, 0.6f), t); // 화면 전체를 붉게 틴트
                colorGrading.contrast.value = Mathf.Lerp(0f, 25f, t); // 대비를 높여 잔혹한 느낌 강조
            }
            
            if (mainCamera != null) mainCamera.fieldOfView = Mathf.Lerp(originalFOV, targetFOV, t);

            yield return null;
        }

        // 4. 폭주 중 지속 효과
        while (true)
        {
            float pulse = Mathf.PingPong(Time.time * 8f, 1f); // 호흡/깜빡임을 더 빠르고 신경질적으로
            
            if (warningText != null) warningText.alpha = 0.5f + pulse * 0.5f;
            if (redOverlay != null) redOverlay.color = new Color(0.8f, 0f, 0f, 0.1f + pulse * 0.2f); // 피 웅덩이 같은 붉은 오버레이
            
            if (portraitImage != null)
            {
                portraitImage.color = Color.Lerp(Color.white, new Color(1f, 0.5f, 0.5f), pulse); // 얼굴이 붉게 상기됨
                portraitImage.transform.localScale = Vector3.one * (1f + pulse * 0.08f);
            }

            if (lensDistortion != null)
            {
                lensDistortion.intensity.overrideState = true;
                lensDistortion.intensity.value = -15f - (Mathf.Sin(Time.time * 5f) * 15f); // 화면 왜곡을 더 강하게
            }

            yield return null;
        }
    }

    private IEnumerator EndEffectsRoutine()
    {
        if (audioSource != null && breathingClip != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(breathingClip);
        }

        float elapsed = 0f;
        float duration = 1.5f; // 끝나는 시간은 조금 더 짧게 (현자타임 느낌)
        
        float startExposure = colorGrading != null ? colorGrading.postExposure.value : 1.0f;
        float startFOV = mainCamera != null ? mainCamera.fieldOfView : targetFOV;
        float startContrast = colorGrading != null ? colorGrading.contrast.value : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (colorGrading != null)
            {
                colorGrading.postExposure.value = Mathf.Lerp(startExposure, 0f, t);
                colorGrading.saturation.value = Mathf.Lerp(30f, 0f, t);
                colorGrading.colorFilter.value = Color.Lerp(colorGrading.colorFilter.value, Color.white, t);
                colorGrading.contrast.value = Mathf.Lerp(startContrast, 0f, t);
            }
            if (vignette != null) vignette.intensity.value = Mathf.Lerp(0.6f, 0f, t);
            if (chromaticAberration != null) chromaticAberration.intensity.value = Mathf.Lerp(0.8f, 0f, t);
            if (lensDistortion != null) lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, 0f, t);

            if (mainCamera != null) mainCamera.fieldOfView = Mathf.Lerp(startFOV, originalFOV, t);
            
            if (warningText != null) warningText.alpha = 1f - t;
            if (timerText != null) timerText.alpha = 1f - t;
            if (redOverlay != null) redOverlay.color = new Color(0.8f, 0f, 0f, Mathf.Lerp(redOverlay.color.a, 0f, t));

            yield return null;
        }

        if (warningUI != null) warningUI.SetActive(false);
    }
}