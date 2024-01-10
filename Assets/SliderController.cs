using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

public class SliderController : MonoBehaviour
{
    public GameObject[] parentObjects; // 用户指定的父物体数组
    //public PinchSlider sliderPrefab; // 使用PinchSlider控件
    //public GameObject closeButtonPrefab; // 关闭按钮预制体

    private PinchSlider[] sliderUIs; // 用于存储动态生成的PinchSlider UI
    private PressableButton[] closeButtonUIs; // 用于存储动态生成的关闭按钮 UI


    void Start()
    {
        sliderUIs = new PinchSlider[parentObjects.Length]; // 初始化PinchSlider UI 数组
        closeButtonUIs = new PressableButton[parentObjects.Length]; // 初始化关闭按钮 UI 数组



        for (int i = 0; i < parentObjects.Length; i++)
        {
            /*
            // 创建一个新的PinchSlider UI 和关闭按钮 UI 作为子物体
            sliderUIs[i] = Instantiate(sliderPrefab, parentObjects[i].transform);
            closeButtonUIs[i] = Instantiate(closeButtonPrefab, parentObjects[i].transform);

            // 重置子物体的缩放，使其脱离父物体的缩放影响
            sliderUIs[i].transform.SetParent(null); // 将 Slider UI 从父物体中解除
            closeButtonUIs[i].transform.SetParent(null); // 将关闭按钮 UI 从父物体中解除

            // 将局部缩放设置为10倍
            sliderUIs[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            closeButtonUIs[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            // 将 Slider 和 Button 设置为父物体的子物体
            sliderUIs[i].transform.SetParent(parentObjects[i].transform);
            closeButtonUIs[i].transform.SetParent(parentObjects[i].transform);
            */

            // 从子物体中查找 PinchSlider 和关闭按钮 UI
           
            sliderUIs[i] = parentObjects[i].GetComponentInChildren<PinchSlider>();
            closeButtonUIs[i] = parentObjects[i].GetComponentInChildren<PressableButton>();

            // 关闭按钮绑定事件
            if (i < parentObjects.Length && i < closeButtonUIs.Length)
            {
                var closeButton = closeButtonUIs[i].GetComponent<Interactable>();
                int index = i; // 保存当前索引值
                closeButton.OnClick.AddListener(() => OnCloseButtonClick(sliderUIs[index]));
            }
        }
    }

    void Update()
    {
        
    }

    private void OnCloseButtonClick(PinchSlider sliderUI)
    {
        sliderUI.gameObject.SetActive(!sliderUI.gameObject.activeSelf); // 切换对应的 PinchSlider UI 的显示状态
    }
}