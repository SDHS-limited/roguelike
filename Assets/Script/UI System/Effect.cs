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
