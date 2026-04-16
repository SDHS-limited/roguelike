using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float lifeTime = 1f;

    public TextMeshProUGUI text;
    private Color color;

    void Start()
    {
        color = text.color;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 위로 이동
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // 점점 투명하게
        color.a -= Time.deltaTime / lifeTime;
        text.color = color;
    }

    public void SetDamage(int damage)
    {
        text.text = damage.ToString();
    }
}
