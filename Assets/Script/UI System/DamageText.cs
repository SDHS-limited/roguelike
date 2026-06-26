using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public float moveSpeed = 2f;
    [SerializeField] public float lifeTime = 1f;

    private TextMeshProUGUI text;
    private Color color;
    private Vector3 initialLocalPos;
    private Camera mainCamera;

    private float timer;
    private bool isPlaying = false;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (text != null) color = text.color;
        initialLocalPos = transform.localPosition;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!isPlaying)
            return;

        // Move upward in local space to follow the parent (Enemy)
        transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;

        // Always face the camera
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }

        // Timer
        timer += Time.deltaTime;

        // Fade out
        if (text != null)
        {
            color.a = Mathf.Lerp(1f, 0f, timer / lifeTime);
            text.color = color;
        }

        if (timer >= lifeTime)
        {
            isPlaying = false;
            timer = 0f;
            transform.localPosition = initialLocalPos;
        }
    }

    public void SetDamage(int damage)
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
        if (text == null) return;

        text.text = damage.ToString();

        // Reset to initial local position to follow the enemy head
        transform.localPosition = initialLocalPos;

        // Initialize
        timer = 0f;
        if (color == default) color = text.color;
        color.a = 1f;
        text.color = color;

        // Start playing
        isPlaying = true;
    }
}
