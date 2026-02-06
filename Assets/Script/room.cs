using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class room : MonoBehaviour
{
    [SerializeField] int alliveEnemy = 0;
    [SerializeField] Text clearText;
    [SerializeField] String fulltext;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alliveEnemy = GetComponentsInChildren<Enemy>().Length;
    }

    public void EnemyDies()
    {
        alliveEnemy--;

        if(alliveEnemy <= 0)
        {
            RoomClear();
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
    }
}
