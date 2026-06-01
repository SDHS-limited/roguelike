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

    public void SetGaugeEffects(float ratio)
    {
        if (ratio >= 0.8f && ratio < 1.0f)
        {
            // 80%~90%: Edge Glow (Vignette increase)
            if (vignette != null)
            {
                vignette.intensity.overrideState = true;
                float v = Mathf.InverseLerp(0.8f, 1.0f, ratio) * 0.4f;
                vignette.intensity.value = v;
                vignette.color.value = new Color(0.8f, 0.7f, 0.2f); // Goldish
            }
        }
        
        if (ratio >= 0.9f && ratio < 1.0f)
        {
            // 90%: Chromatic Aberration start & Heartbeat
            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.overrideState = true;
                chromaticAberration.intensity.value = Mathf.InverseLerp(0.9f, 1.0f, ratio) * 0.5f;
            }
            
            // Heartbeat sound logic could be here, but we'll trigger it once in manager
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

        // 1. Pre-flash Golden Halo / Particle hint (Optional UI pulse)
        if (redOverlay != null)
        {
            redOverlay.color = new Color(1, 0.9f, 0.5f, 0.5f); // Golden flash hint
        }

        // 2. Entry Flash (0.3s as recommended)
        if (flashOverlay != null)
        {
            flashOverlay.gameObject.SetActive(true);
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

        // Character Portrait state
        Color originalPortraitColor = portraitImage != null ? portraitImage.color : Color.white;
        Vector3 originalPortraitScale = portraitImage != null ? portraitImage.transform.localScale : Vector3.one;

        // Flash Fade Out (Fast 0.3s)
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

        // 3. Gradual Lens Distortion, FOV Increase, and Exposure Brightening
        float elapsed = 0f;
        float transitionDuration = 1.0f; // Faster transition to feel the "power"
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            if (colorGrading != null)
            {
                colorGrading.saturation.overrideState = true;
                colorGrading.contrast.overrideState = true;
                colorGrading.colorFilter.overrideState = true;
                colorGrading.postExposure.overrideState = true;

                colorGrading.saturation.value = Mathf.Lerp(0f, 25f, t);
                colorGrading.contrast.value = Mathf.Lerp(0f, 35f, t);
                colorGrading.postExposure.value = Mathf.Lerp(0f, 0.8f, t); // Brighten as recommended
                colorGrading.colorFilter.value = Color.Lerp(Color.white, new Color(1, 0.95f, 0.8f), t); 
            }
            if (vignette != null)
            {
                vignette.intensity.overrideState = true;
                vignette.intensity.value = Mathf.Lerp(0.4f, 0.55f, t);
                vignette.color.value = new Color(0.1f, 0.05f, 0f); // Dark gold edge
            }
            if (chromaticAberration != null)
            {
                chromaticAberration.intensity.overrideState = true;
                chromaticAberration.intensity.value = Mathf.Lerp(0.5f, 0.8f, t);
            }
            
            if (mainCamera != null) mainCamera.fieldOfView = Mathf.Lerp(originalFOV, targetFOV, t);

            yield return null;
        }

        // 4. Continuous Pulse
        while (true)
        {
            float pulse = Mathf.PingPong(Time.time * 6f, 1f);
            
            if (warningText != null) warningText.alpha = 0.6f + pulse * 0.4f;
            
            if (redOverlay != null) redOverlay.color = new Color(1, 0.9f, 0.4f, 0.05f + pulse * 0.1f);
            
            if (portraitImage != null)
            {
                portraitImage.color = Color.Lerp(originalPortraitColor, new Color(1, 1, 0.7f), pulse); // Goldish pulse
                portraitImage.transform.localScale = originalPortraitScale * (1f + pulse * 0.08f);
            }

            if (lensDistortion != null)
            {
                lensDistortion.intensity.overrideState = true;
                lensDistortion.intensity.value = -15f - (Mathf.Sin(Time.time * 2.5f) * 10f);
            }

            yield return null;
        }
    }

    private IEnumerator EndEffectsRoutine()
    {
        Debug.Log("[Berserk] Power Normalized - Side Effects Active");
        
        if (portraitImage != null)
        {
            portraitImage.color = new Color(0.7f, 0.7f, 0.7f); // Drained look
            portraitImage.transform.localScale = Vector3.one;
        }

        if (audioSource != null && breathingClip != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(breathingClip);
        }

        float elapsed = 0f;
        float duration = 3.5f; 
        
        float startSaturation = colorGrading != null ? colorGrading.saturation.value : 0f;
        float startExposure = colorGrading != null ? colorGrading.postExposure.value : 0f;
        float startFOV = mainCamera != null ? mainCamera.fieldOfView : originalFOV;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (colorGrading != null)
            {
                colorGrading.saturation.value = Mathf.Lerp(startSaturation, 0f, t);
                colorGrading.postExposure.value = Mathf.Lerp(startExposure, 0f, t);
                colorGrading.colorFilter.value = Color.Lerp(colorGrading.colorFilter.value, Color.white, t);
            }
            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(0.55f, 0f, t);
            }
            if (chromaticAberration != null) chromaticAberration.intensity.value = Mathf.Lerp(0.8f, 0f, t);
            if (lensDistortion != null) lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, 0f, t);

            if (mainCamera != null) mainCamera.fieldOfView = Mathf.Lerp(startFOV, originalFOV, t);
            
            if (redOverlay != null) redOverlay.color = Color.Lerp(redOverlay.color, new Color(1, 1, 1, 0), t);
            if (warningText != null) warningText.alpha = 1f - t;
            if (timerText != null) timerText.alpha = 1f - t;

            yield return null;
        }

        if (warningUI != null) warningUI.SetActive(false);
        if (portraitImage != null) portraitImage.color = Color.white;
    }
}
