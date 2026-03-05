using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class room : MonoBehaviour
{
    [SerializeField] int alliveEnemy = 0;
    [SerializeField] Text clearText;
    [SerializeField] String fulltext;    

    [Header("Enemy")]
    [SerializeField] float radius = 4f;
    [SerializeField] LayerMask targetMask;
    bool isClear = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alliveEnemy = GetComponentsInChildren<Enemy>().Length;
    }

    void Update()
    {
        if(isClear) return;

        Collider[] hit = Physics.OverlapSphere(transform.position, radius, targetMask);    

        if (hit.Length == 0 && !isClear)
        {
            StartCoroutine(RoomClear()); 
        }

    }

    IEnumerator RoomClear()
    {
        clearText.text = "";

        foreach(var c in fulltext)
        {
            clearText.text += c;
            yield return new WaitForSeconds(0.05f);
        }

        if (isClear)
        {
            yield return new WaitForSeconds(1f);
            isClear = false;
            clearText.text = "";
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
