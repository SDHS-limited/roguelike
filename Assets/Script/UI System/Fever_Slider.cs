using System;
using UnityEngine;
using UnityEngine.UI;

public class Fever_Slider : MonoBehaviour
{
    [Header("Fever Slider")]
    public Slider feverSlider;

    [SerializeField] private float maxFever = 100f;
    public float currentFever;

    public event Action<float> OnFeverChanged;
    public event Action OnBerserkReached;

    public float CurrentFeverRatio => currentFever / maxFever;

    void Start()
    {
        if (feverSlider == null) feverSlider = GetComponent<Slider>();
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
        
        // '광휘 누출' 후유증 (ID: 108) 적용: 폭주 게이지 자연 증가
        if (SideEffectManager.Instance != null && SideEffectManager.Instance.HasEffect(108))
        {
            AddFever(Time.deltaTime * 0.5f); // 초당 0.5씩 증가
        }

        if (Input.GetKeyUp(KeyCode.Backspace)) 
{
            AddFever(100f);
        }


    }

    private void UpdateUI()
    {
        if (feverSlider != null)
        {
            feverSlider.value = Mathf.Lerp(feverSlider.value, CurrentFeverRatio, Time.deltaTime * 5f);
        }
    }

    public void AddFever(float amount)
    {
        float previousFever = currentFever;
        
        // '통제 불능' 후유증 (ID: 301) 적용: 폭주 게이지 증가량 +50%
        if (SideEffectManager.Instance != null && SideEffectManager.Instance.HasEffect(301))
        {
            amount *= 1.5f;
        }

        currentFever = Mathf.Clamp(currentFever + amount, 0, maxFever);

        if (previousFever != currentFever)
        {
            OnFeverChanged?.Invoke(CurrentFeverRatio);
            if (currentFever >= maxFever && previousFever < maxFever)
            {
                OnBerserkReached?.Invoke();
            }
        }
    }

    public void ResetFever(float targetPercent = 0f)
    {
        currentFever = maxFever * targetPercent;
        OnFeverChanged?.Invoke(CurrentFeverRatio);
    }
}