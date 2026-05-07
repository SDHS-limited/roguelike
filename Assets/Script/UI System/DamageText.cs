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
    private Vector3 startPos;

    private float timer;
    private bool isPlaying = false;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        color = text.color;
    }

    void Update()
    {
        if (!isPlaying)
            return;

        // 위로 이동
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // 시간 증가
        timer += Time.deltaTime;

        // 점점 투명하게
        color.a = Mathf.Lerp(1f, 0f, timer / lifeTime);
        text.color = color;
        if (timer >= lifeTime)
        {
            isPlaying = false;

            timer = 0f;

            transform.position = startPos;
        }
    }

    public void SetDamage(int damage)
    {
        text.text = damage.ToString();

        // 현재 위치를 시작 위치로 저장
        startPos = transform.position;

        // 초기화
        timer = 0f;

        color.a = 1f;
        text.color = color;

        // 실행 시작
        isPlaying = true;
    }
}
