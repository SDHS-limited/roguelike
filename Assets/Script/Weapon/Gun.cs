using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;

    [Header("Gun")]
    [SerializeField] Transform GunObject;   // 회전시킬 총 오브젝트
    [SerializeField] float reloadAngle = 360f; // 총이 회전할 각도
    [SerializeField] float reloadTime = 1f; // 장전 시간

    private Quaternion startRot; // 시작 회전값 저장
    
    [SerializeField] float currentammo = 7f;
    [SerializeField] TMP_Text ammo;
    [SerializeField] Reload_Slider reload_Slider;
    [SerializeField] Recoil recoil;

    void Start()
    {
        startRot = GunObject.transform.localRotation;
        // startRot = arm.transform.localRotation;
        reload_Slider.gameObject.SetActive(true);
    }

    private bool isJamming = false;

    public void SetJamming(bool value)
    {
        isJamming = value;
    }

    public void ForceFire()
    {
        if (currentammo > 0)
        {
            Shoot();
        }
    }

    private bool isBerserk = false;
    private float berserkFireTimer = 0f;

    public void SetBerserk(bool value)
    {
        isBerserk = value;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Map_Build_test") return;
        ammo.text = ""+currentammo;
        
        if (isBerserk)
        {
            currentammo = 7; // 무한 탄약
            berserkFireTimer -= Time.deltaTime;
            if (berserkFireTimer <= 0)
            {
                Shoot();
                berserkFireTimer = 0.1f; // 폭주 시 연사 속도
            }
            return;
        }

        if (Input.GetMouseButton(0))
        {
            if(currentammo <= 0 || isJamming) return;
            if (!recoil.CanFire) return;

            Shoot();
            // audioSource.PlayOneShot(fire);
        }
        

        if (Input.GetKeyDown(KeyCode.R) || currentammo <= 0)
        {
            if (!recoil.CanFire || isJamming) return;
            currentammo = 7;
            StartCoroutine(reload_Slider.FillRoutine());
            StartCoroutine(ReloadAnim());
        }


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
        recoil.Fire();
    }   
}


