using System.Collections;
using UnityEngine;

public class AftereffectManager : MonoBehaviour
{
    [SerializeField] Player player;


    public IEnumerator SpeedDown()
    {
        player.speed = 2f;
        yield return new WaitForSeconds(10f);
        player.speed = 5f;
    }

    public IEnumerator PainDamage()
    {
        player.damage = 20;
        yield return new WaitForSeconds(10f);
        player.damage = 10;
    }

}
