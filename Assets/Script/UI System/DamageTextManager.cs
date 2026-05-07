using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageTextManager : MonoBehaviour
{
     public static DamageTextManager Instance { get; private set; }

    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        // 풀 미리 생성
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(damageTextPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public void ShowDamage(Vector3 worldPosition, float damage, bool isCritical = false)
    {
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(damageTextPrefab);
        }

        obj.transform.position = worldPosition + Vector3.up * 1.5f; // 적 머리 위
        var effect = obj.GetComponent<DamageTextEffect>();
        effect.Init(damage, isCritical);

        // 일정 시간 후 풀로 반환
        StartCoroutine(ReturnToPool(obj, 1f));
    }

    private IEnumerator ReturnToPool(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        pool.Enqueue(obj);

        //         DamageTextManager.Instance.ShowDamage(
        //     transform.position,
        //     damage,
        //     isCritical
        // );
    }
}
