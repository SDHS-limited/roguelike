using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("응애");
        }
    }
}
