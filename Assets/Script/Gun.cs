using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip fire;
    
    [Header("recoil")]
    [SerializeField] Transform arm;     // 팔 본 또는 총 오브젝트
    [SerializeField] float recoilAngle = 10f; //반동 세기
    [SerializeField] float recoilSpeed = 20f; //튕기는 소리
    [SerializeField] float returnSpeed = 10f; //복귀 소리
    Quaternion originalRotation;
    bool isRecoiling = false;

    void Start()
    {
        originalRotation = arm.localRotation;
    }

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
    public void Fire()
    {
        if (!isRecoiling)
            StartCoroutine(Recoil());
    }

    IEnumerator Recoil()
    {
        isRecoiling = true;

        // 1️⃣ 위로 빠르게 튕김
        Quaternion recoilRotation =
            originalRotation * Quaternion.Euler(-recoilAngle, 0f, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            arm.localRotation = Quaternion.Slerp(originalRotation, recoilRotation, t);
            yield return null;
        }

        // 2️⃣ 다시 복구
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * returnSpeed;
            arm.localRotation = Quaternion.Slerp(recoilRotation, originalRotation, t);
            yield return null;
        }

        arm.localRotation = originalRotation;
        isRecoiling = false;
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }   
}
