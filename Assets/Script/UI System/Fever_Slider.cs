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