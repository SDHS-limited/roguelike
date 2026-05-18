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
    [SerializeField] Effect effect;

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
            HandleBerserkFiring();
        }
        else
        {
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
    }

    private int burstCount = 0;
    private void HandleBerserkFiring()
    {
        berserkFireTimer -= Time.deltaTime;
        
        if (berserkFireTimer <= 0)
        {
            // 폭주 상태의 불안정한 발사 로직
            float randomRoll = UnityEngine.Random.value;

            if (burstCount > 0)
            {
                // 버스트(멋대로 연사) 중일 때는 매우 빠르게 발사
                Shoot();
                berserkFireTimer = 0.05f; 
                burstCount--;
                
                // 버스트 중에는 반동 지터를 더 강하게 줌
                if (recoil != null) recoil.SetJitter(UnityEngine.Random.Range(2f, 4f));
            }
            else if (randomRoll < 0.20f) 
            {
                // 20% 확률로 2~3발 멋대로 폭발적 발사 (버스트 시작)
                burstCount = UnityEngine.Random.Range(2, 4);
                Shoot();
                berserkFireTimer = 0.05f;
            }
            else if (randomRoll < 0.40f)
            {
                // 20% 확률로 총이 일시적으로 잘 안나감 (스타터/딜레이)
                // 발사하지 않고 타이머만 길게 설정
                berserkFireTimer = UnityEngine.Random.Range(0.5f, 1.2f);
                
                // 틱틱 거리는 소리나 효과를 주면 더 좋음
                if (audioSource != null && reloadSound != null) audioSource.PlayOneShot(reloadSound, 0.5f);
            }
            else
            {
                // 일반적인 폭주 발사 (약간의 랜덤성)
                Shoot();
                berserkFireTimer = UnityEngine.Random.Range(0.1f, 0.2f);
            }
        }
    }
    


    // 총 회전 하면서 장전 모션
    IEnumerator ReloadAnim()
    {
        if (audioSource != null && reloadSound != null) audioSource.PlayOneShot(reloadSound);
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
    [Header("Juice")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip fireSound;
    [SerializeField] AudioClip reloadSound;

    void Shoot()
    {
        currentammo--;
        if (muzzleFlash != null)
        {
            muzzleFlash.Stop();
            muzzleFlash.Play();
        }
        if (audioSource != null && fireSound != null) audioSource.PlayOneShot(fireSound);
        
        Effect effects = FindFirstObjectByType<Effect>();
        
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (effects != null) effects.TriggerCameraShake(0.1f, 0.15f);
        recoil.Fire();
    }
}


