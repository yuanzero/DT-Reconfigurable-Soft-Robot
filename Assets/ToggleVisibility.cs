using UnityEngine;
using UnityEngine.UI;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject[] targetObject; // 需要隐藏/显示的物体

    private void Start()
    {
        // 获取Toggle组件
        Toggle toggle = GetComponent<Toggle>();
        // 添加勾选状态改变的监听事件
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool value)
    {
        for (int i = 0; i < targetObject.Length; i++)
        {
            SetRenderersVisibility(targetObject[i], value);
        }
    }

    private void SetRenderersVisibility(GameObject obj, bool value)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.enabled = value;
        }

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            GameObject child = obj.transform.GetChild(i).gameObject;
            SetRenderersVisibility(child, value);
        }
    }
}