using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Recoil recoil;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!recoil.CanFire) return;
            StartCoroutine(Shake(0.15f, 0.15f));
        }
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float y = Random.Range(-0.2f, 0.2f) * magnitude;
            transform.localPosition = new Vector3(originalPosition.x + y, originalPosition.y + y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = originalPosition;
    }
}
