using UnityEngine;

public class ExperimentObj : MonoBehaviour
{
    [SerializeField] float range = 6f;
    [SerializeField] LayerMask playerMask;
    [SerializeField] GameObject experiment; // 실험창
    [SerializeField] ExperimentManager experimentManager;
    [SerializeField] CameraRot cameraRot;
    [SerializeField] bool isvisit = false;

    void Start()
    {
        if (cameraRot == null) cameraRot = FindFirstObjectByType<CameraRot>();
        if (experiment != null) experiment.SetActive(false);
    }

    void Update()
    {
        if (isvisit) return;

        Collider[] hit = Physics.OverlapSphere(transform.position, range, playerMask);
        bool isPlayerInRange = hit.Length > 0;

        // UI 활성화: 플레이어가 근처에 있고, 현재 선택 중이 아닐 때
        if (isPlayerInRange && !experimentManager.isSelete)
        {
            if (!experiment.activeSelf)
            {
                OpenExperimentUI();
            }
        }
        // UI 비활성화: 범위를 벗어났고, 선택 중이 아닐 때만 닫음
        else if (!isPlayerInRange && experimentManager.isSelete)
        {
            if (experiment.activeSelf)
            {
                CloseExperimentUI();
            }
        }

        // 방문 처리: 이 방에서 선택이 발생하면 다시 열리지 않게 함
        if (experimentManager.isSelete && experiment.activeSelf && isPlayerInRange)
        {
            if (experimentManager.activeRoom == this)
            {
                isvisit = true;
            }
        }
    }

    private void OpenExperimentUI()
    {
        if (experiment == null) return;
        experiment.SetActive(true);
        if (cameraRot != null) cameraRot.isUIOpen = true;
        
        // 현재 이 방을 Manager에 등록
        if (experimentManager != null) experimentManager.activeRoom = this;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseExperimentUI()
    {
        if (experiment == null) return;
        experiment.SetActive(false);
        if (cameraRot != null) cameraRot.isUIOpen = false;
        
        if (experimentManager != null && experimentManager.activeRoom == this)
            experimentManager.activeRoom = null;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MarkAsVisited()
    {
        isvisit = true;
        // 시각적 연출 등을 위해 필요하다면 여기서 처리
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
