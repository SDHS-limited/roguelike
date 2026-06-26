using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainTitle : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Demo2";

    private void Awake()
    {
        BindButtons();
    }

    private void OnDestroy()
    {
        UnbindButtons();
    }

    private void BindButtons()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnClickStartButton);
            startButton.onClick.AddListener(OnClickStartButton);
        }
        else
        {
            Debug.LogWarning("[MainTitle] Start Button is not assigned.");
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnClickExitButton);
            exitButton.onClick.AddListener(OnClickExitButton);
        }
        else
        {
            Debug.LogWarning("[MainTitle] Exit Button is not assigned.");
        }
    }

    private void UnbindButtons()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnClickStartButton);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnClickExitButton);
        }
    }

    public void OnClickStartButton()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("[MainTitle] gameSceneName is empty.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            Debug.LogError($"[MainTitle] Scene '{gameSceneName}' is not in Build Settings.");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickExitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
