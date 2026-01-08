using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float shakeAmount = 3.0f;
    [SerializeField] float shakeTime = 1.0f;
    [SerializeField] Camera cam;
 
    void Update()
    {
        StartCoroutine(Shake(shakeAmount, shakeTime));
    }
 
    IEnumerator Shake(float ShakeAmount, float ShakeTime)
    {
        float timer = 0;
        while (timer <= ShakeTime)
        {
            cam.transform.position = 
                (Vector3)UnityEngine.Random.insideUnitCircle * ShakeAmount;
            timer += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.position = new Vector3(0f, 0f, 0f);
    }
}
