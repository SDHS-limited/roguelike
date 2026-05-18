using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Effect : MonoBehaviour
{
    [SerializeField] Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    [SerializeField] CameraShake cameraShake;

    private Coroutine hitStopCoroutine;

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
        Color c = image.color;
        c = Color.red;
        c.a = 0.3f;
        image.color = c;

        yield return new WaitForSeconds(0.1f);

        c.a = 0f;
        image.color = c;
    }
    public IEnumerator Heal()
    {
        Color c = image.color;
        c = Color.green;
        c.a = 0.3f;
        image.color = c;

        yield return new WaitForSeconds(0.1f);

        c.a = 0f;
        image.color = c;
    }
}
