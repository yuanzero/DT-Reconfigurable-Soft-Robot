using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class SliderController : MonoBehaviour
{
    public GameObject[] parentObjects; // �û�ָ���ĸ���������
    //public PinchSlider sliderPrefab; // ʹ��PinchSlider�ؼ�
    //public GameObject closeButtonPrefab; // �رհ�ťԤ����

    private PinchSlider[] sliderUIs; // ���ڴ洢��̬���ɵ�PinchSlider UI
    private PressableButton[] closeButtonUIs; // ���ڴ洢��̬���ɵĹرհ�ť UI


    void Start()
    {
        sliderUIs = new PinchSlider[parentObjects.Length]; // ��ʼ��PinchSlider UI ����
        closeButtonUIs = new PressableButton[parentObjects.Length]; // ��ʼ���رհ�ť UI ����



        for (int i = 0; i < parentObjects.Length; i++)
        {
            /*
            // ����һ���µ�PinchSlider UI �͹رհ�ť UI ��Ϊ������
            sliderUIs[i] = Instantiate(sliderPrefab, parentObjects[i].transform);
            closeButtonUIs[i] = Instantiate(closeButtonPrefab, parentObjects[i].transform);

            // ��������������ţ�ʹ�����븸���������Ӱ��
            sliderUIs[i].transform.SetParent(null); // �� Slider UI �Ӹ������н��
            closeButtonUIs[i].transform.SetParent(null); // ���رհ�ť UI �Ӹ������н��

            // ���ֲ���������Ϊ10��
            sliderUIs[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            closeButtonUIs[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            // �� Slider �� Button ����Ϊ�������������
            sliderUIs[i].transform.SetParent(parentObjects[i].transform);
            closeButtonUIs[i].transform.SetParent(parentObjects[i].transform);
            */

            // ���������в��� PinchSlider �͹رհ�ť UI
           
            sliderUIs[i] = parentObjects[i].GetComponentInChildren<PinchSlider>();
            closeButtonUIs[i] = parentObjects[i].GetComponentInChildren<PressableButton>();

            // �رհ�ť���¼�
            if (i < parentObjects.Length && i < closeButtonUIs.Length)
            {
                var closeButton = closeButtonUIs[i].GetComponent<Interactable>();
                int index = i; // ���浱ǰ����ֵ
                closeButton.OnClick.AddListener(() => OnCloseButtonClick(sliderUIs[index]));
            }
        }
    }

    void Update()
    {
        
    }

    private void OnCloseButtonClick(PinchSlider sliderUI)
    {
        sliderUI.gameObject.SetActive(!sliderUI.gameObject.activeSelf); // �л���Ӧ�� PinchSlider UI ����ʾ״̬
    }
}