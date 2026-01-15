using UnityEngine;
using UnityEngine.UI;

public class ExperimentManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image experimentImage;
    [SerializeField] Text name;
    [SerializeField] Text des;


    [Header("Experiments")]
    [SerializeField] Experiment[] experiments;
    Experiment currentExperiment;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowRandomExperiment();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ShowRandomExperiment()
    {
        if (experiments.Length == 0)
        {
            Debug.LogWarning("Experiment가 비어있음!");
            return;
        }

        int randomIndex = Random.Range(0, experiments.Length);
        currentExperiment = experiments[randomIndex];

        UpdateUI();
    }

    void UpdateUI()
    {
        name.text = currentExperiment.name;
        des.text = currentExperiment.Des;

        // 이미지 나중에 추가할 때
        // experimentImage.sprite = currentExperiment.image;
    }
}
