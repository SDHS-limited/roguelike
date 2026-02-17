using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float duration = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image.fillAmount = 1f;
        StartCoroutine(FillDown());
    }

    System.Collections.IEnumerator FillDown()
    {
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            image.fillAmount = 1 - (t / duration);
            yield return null;
        }

        image.fillAmount = 0f;
    }
}
