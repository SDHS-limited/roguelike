using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Reload_Slider : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float duration = 1.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public IEnumerator FillRoutine()
    {
        image.gameObject.SetActive(true);
        float time = 0;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(0, 1, time / duration);
            yield return null;
        }

        image.fillAmount = 0; // 마지막 보정
        image.gameObject.SetActive(false);
    }
}
