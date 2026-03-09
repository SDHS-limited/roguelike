using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip fire;
    
    [Header("recoil")]
    [SerializeField] GameObject GunObject;
    [SerializeField] float reloadAngle = 360f;
    [SerializeField] float reloadTime = 0.3f; 
    Quaternion startRot;

    [SerializeField] GameObject arm;
    [SerializeField] float armAngle = -7;
    [SerializeField] float armTime = 0.3f;

    [SerializeField] float currentammo = 7f;
    [SerializeField] TMP_Text ammo; 

    void Start()
    {
        startRot = GunObject.transform.localRotation;
        startRot = arm.transform.localRotation;
    }

    void Update()
    {
        ammo.text = ""+currentammo;
        if (Input.GetMouseButtonDown(0))
        {
            if(currentammo <= 0) return;
            Shoot();
            StartCoroutine(Arm());
            audioSource.PlayOneShot(fire);
        }
        

        if (Input.GetKeyDown(KeyCode.R) || currentammo <= 0)
        {
            currentammo = 7;

        }
    }
    
    IEnumerator Arm()
    {
        float rotated = 0f;
        float speed = armAngle / armTime;

        // 360도 회전
        while (rotated < reloadAngle)
        {
            float step = speed * Time.deltaTime;

            GunObject.transform.Rotate(step, 0, 0);

            rotated += step;

            yield return null;
        }
        arm.transform.localRotation = startRot;
    }

    // 총 회전 하면서 장전 모션
    IEnumerator ReloadAnim()
    {
        float rotated = 0f;
        float speed = reloadAngle / reloadTime;

        // 360도 회전
        while (rotated < reloadAngle)
        {
            float step = speed * Time.deltaTime;

            GunObject.transform.Rotate(step, 0, 0);

            rotated += step;

            yield return null;
        }

        // 원래 회전값으로 복귀
        GunObject.transform.localRotation = startRot;
    }
    void Shoot()
    {
        currentammo--;
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }   
}
