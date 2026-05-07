using UnityEngine;
using System.Collections;
using TMPro;

public class DamageTextEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private float spreadRadius = 0.3f;

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Init(float damage, bool isCritical = false)
    {
        // 크리티컬 여부에 따라 색상/크기 변경
        if (isCritical)
        {
            damageText.text = $"<color=#FF4444><size=130%>{Mathf.RoundToInt(damage)}!</size></color>";
        }
        else
        {
            damageText.text = $"<color=#FFFFFF>{Mathf.RoundToInt(damage)}</color>";
        }

        // 약간 랜덤하게 퍼지게 (겹침 방지)
        Vector3 randomOffset = new Vector3(
            Random.Range(-spreadRadius, spreadRadius),
            0f,
            Random.Range(-spreadRadius, spreadRadius)
        );
        transform.position += randomOffset;

        StartCoroutine(AnimateAndDestroy());
    }

    void LateUpdate()
    {
        // 항상 카메라를 바라보게
        if (mainCamera != null)
            transform.LookAt(mainCamera.transform);
    }

    private IEnumerator AnimateAndDestroy()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Color startColor = damageText.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // 위로 떠오르기
            transform.position = startPos + Vector3.up * (floatSpeed * t);

            // 페이드 아웃
            damageText.alpha = 1f - t;

            yield return null;
        }

        Destroy(gameObject);
    }
}
