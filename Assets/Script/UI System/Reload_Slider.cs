using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reload_Slider : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float duration = 1.5f;

    public IEnumerator FillRoutine()
    {
        float time = 0;
        

        while (time < duration)
        {
            time += Time.deltaTime;
            float angle = Mathf.Lerp(0, 360, time / duration);
            image.rectTransform.localRotation = Quaternion.Euler(0, 0, -angle);
            yield return null;
        }

        image.rectTransform.localRotation = Quaternion.identity; // 회전 초기화
    }

}

