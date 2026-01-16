using Unity.VisualScripting;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UI;
using static UITest_Player;

public class Fever_Slider : MonoBehaviour
{
    /*
     * �ǹ� �����̴� ���
     * -> Ư�� ���� �޼��� �����̴��� ���� �����ϴ� �ִ밪�� �����ϸ� ������ �ӵ��� ����
     * 
     * ���� �ӽ� ����
     * ���� - ���콺 Ŭ�� Ƚ��
     * 
     * �Ŀ� �������� ä���� ����
     * ���� - ���� Ư�� ȹ�� ��ŭ ���Ͻ�
     */

    [Header("Fever Slider")]
    public Slider feverSlider; //�ǹ� �����̴�

    public float feverValue; //�ǹ� ��
    [SerializeField] private float maxFever; //�ִ� �ǹ� ��
    [SerializeField] private float minFever; //�ּ� �ǹ� ��
    [SerializeField] private float currentFever; // ���� �ǹ� ��
    private float perFever; //����� �ǹ� ��

    public enum FeverState
    {
        None, /*feverDesr,*/ feverIncr
    }
    public FeverState feverState = FeverState.None;

    public float feverIncrSpeed; //�ǹ� ���� �ӵ�
    //public float feverDesrSpeed; //�ǹ� ���� �ӵ�

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        minFever = feverValue;
        feverSlider.value = currentFever / maxFever;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerFever();

        feverSlider.value = currentFever / maxFever;

        if (Input.GetMouseButtonDown(0)) {
            currentFever += 10;
        }

    }

    void PlayerFever()
    {
        switch (feverState)
        {
            case FeverState.feverIncr:
                {
                    //Fever�����̴��� ���� perFever������ feverIncrSpeed�� �ӵ��� �����ϰ� �̵��Ѵ�.
                    feverSlider.value = Mathf.MoveTowards(feverSlider.value, perFever, feverIncrSpeed * Time.deltaTime);

                    if (feverSlider.value == perFever)
                    {
                        feverState = FeverState.None;
                    }
                    break;
                }
        }
    }
}
