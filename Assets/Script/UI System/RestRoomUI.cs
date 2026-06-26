using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RestRoomUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Button prayerButton;
    [SerializeField] private Button purificationButton;
    [SerializeField] private Button stabilizationButton;
    [SerializeField] private Button meditationButton;

    [Header("References")]
    [SerializeField] private HP_Slider hpSlider;
    [SerializeField] private Fever_Slider feverSlider;
    [SerializeField] private CameraRot cameraRot;

    void Start()
    {
        prayerButton.onClick.AddListener(OnPrayer);
        purificationButton.onClick.AddListener(OnPurification);
        stabilizationButton.onClick.AddListener(OnStabilization);
        meditationButton.onClick.AddListener(OnMeditation);
        
        if (uiPanel != null) uiPanel.SetActive(false);
    }

    public void Show()
    {
        if (uiPanel != null) uiPanel.SetActive(true);
        if (cameraRot != null) cameraRot.isUIOpen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Hide()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
        if (cameraRot != null) cameraRot.isUIOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnPrayer()
    {
        hpSlider.curHP = hpSlider.maxHp;
        Debug.Log("Rest Room: Full Heal applied.");
        Hide();
    }

    private void OnPurification()
    {
        if (SideEffectManager.Instance != null) SideEffectManager.Instance.DecrementBerserkCount();
        Debug.Log("Rest Room: Berserk count decreased.");
        Hide();
    }

    private void OnStabilization()
    {
        if (SideEffectManager.Instance != null) SideEffectManager.Instance.RemoveRandomSideEffect();
        Debug.Log("Rest Room: 1 Side Effect removed.");
        Hide();
    }

    private void OnMeditation()
    {
        if (feverSlider != null) feverSlider.ResetFever(0f);
        Debug.Log("Rest Room: Instability (Fever) reduced.");
        Hide();
    }
}
