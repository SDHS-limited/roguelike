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
            targetFOV = originalFOV + 15f; 
        }

        if (warningUI != null) warningUI.SetActive(false);
        if (flashOverlay != null) flashOverlay.gameObject.SetActive(false);
    }

    public void SetGaugeEffects(float ratio)
    {
        // 80%~100% Preview sequence
        if (ratio >= 0.8f)
        {
            if (vignette != null)
            {
                vignette.intensity.overrideState = true;
                vignette.color.overrideState = true;
                float v = Mathf.InverseLerp(0.8f, 1.0f, ratio) * 0.45f;
                vignette.intensity.value = v;
                vignette.color.value = new Color(1f, 0.8f, 0.2f); // Goldish edge
            }
        }

        if (ratio >= 0.9f)
        {
            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.overrideState = true;
                chromaticAberration.intensity.value = Mathf.InverseLerp(0.9f, 1.0f, ratio) * 0.6f;
            }
            
            // Faint heartbeat pulse in vignette
            if (vignette != null)
            {
                float pulse = Mathf.PingPong(Time.time * 2f, 0.1f);
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
            timerText.text = $"STABILITY: {remaining:F1}s";
        }
    }

    private IEnumerator StartEffectsRoutine()
    {
        Debug.Log("[Berserk] Starting Enhanced Presentation");

        // 1. Golden Hint (0.1s)
        if (flashOverlay != null)
        {
            flashOverlay.gameObject.SetActive(true);
            flashOverlay.color = new Color(1, 0.9f, 0.5f, 0.6f); // Golden glow
            yield return new WaitForSeconds(0.1f);
        }

        // 2. Entry White Flash (0.3s)
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
                warningText.text = "LIMITER REMOVED\n<size=80%>SACRED OVERDRIVE</size>";
            }
        }

        // Fade Flash Out
        float flashDuration = 0.3f;
        float elapsedFlash = 0f;
        while (elapsedFlash < flashDuration)
        {
            elapsedFlash += Time.deltaTime;
            if (flashOverlay != null) flashOverlay.color = new Color(1, 1, 1, 1 - (elapsedFlash / flashDuration));
            yield return null;
        }
        if (flashOverlay != null) flashOverlay.gameObject.SetActive(false);

        // Heartbeat start
        if (audioSource != null && heartbeatClip != null)
        {
            audioSource.clip = heartbeatClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 3. Active State (Bright & Strong)
        float elapsed = 0f;
        float transitionDuration = 0.5f; // Fast transition
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            if (colorGrading != null)
            {
                colorGrading.postExposure.overrideState = true;
                colorGrading.saturation.overrideState = true;
                colorGrading.colorFilter.overrideState = true;

                colorGrading.postExposure.value = Mathf.Lerp(0f, 1.0f, t); // Brighter exposure
                colorGrading.saturation.value = Mathf.Lerp(0f, 20f, t);
                colorGrading.colorFilter.value = Color.Lerp(Color.white, new Color(1, 0.95f, 0.85f), t); // Warm gold tint
            }
            
            if (mainCamera != null) mainCamera.fieldOfView = Mathf.Lerp(originalFOV, targetFOV, t);

            yield return null;
        }

        // 4. Constant Effects while Active
        while (true)
        {
            float pulse = Mathf.PingPong(Time.time * 6f, 1f);
            
            if (warningText != null) warningText.alpha = 0.7f + pulse * 0.3f;
            if (redOverlay != null) redOverlay.color = new Color(1, 0.85f, 0.3f, 0.05f + pulse * 0.1f);
            
            if (portraitImage != null)
            {
                portraitImage.color = Color.Lerp(Color.white, new Color(1, 1, 0.8f), pulse);
                portraitImage.transform.localScale = Vector3.one * (1f + pulse * 0.05f);
            }

            if (lensDistortion != null)
            {
                lensDistortion.intensity.overrideState = true;
                lensDistortion.intensity.value = -10f - (Mathf.Sin(Time.time * 3f) * 10f);
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
        float duration = 2.5f; 
        
        float startExposure = colorGrading != null ? colorGrading.postExposure.value : 1.0f;
        float startFOV = mainCamera != null ? mainCamera.fieldOfView : targetFOV;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (colorGrading != null)
            {
                colorGrading.postExposure.value = Mathf.Lerp(startExposure, 0f, t);
                colorGrading.saturation.value = Mathf.Lerp(20f, 0f, t);
                colorGrading.colorFilter.value = Color.Lerp(colorGrading.colorFilter.value, Color.white, t);
            }
            if (vignette != null) vignette.intensity.value = Mathf.Lerp(0.5f, 0f, t);
            if (chromaticAberration != null) chromaticAberration.intensity.value = Mathf.Lerp(0.8f, 0f, t);
            if (lensDistortion != null) lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, 0f, t);

            if (mainCamera != null) mainCamera.fieldOfView = Mathf.Lerp(startFOV, originalFOV, t);
            
            if (warningText != null) warningText.alpha = 1f - t;
            if (timerText != null) timerText.alpha = 1f - t;

            yield return null;
        }

        if (warningUI != null) warningUI.SetActive(false);
        }
        }
