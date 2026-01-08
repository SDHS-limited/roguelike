using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip fire;

    [SerializeField] Animator anim;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
            // anim.SetTrigger("Shoot");
            audioSource.PlayOneShot(fire);

        }
        else
        {
            // anim.SetBool("IsShoot", false);
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }   
}
