using UnityEngine;
using UnityEngine.UI;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject targetObject; // ��Ҫ����/��ʾ������

    private void Start()
    {
        // ��ȡToggle���
        Toggle toggle = GetComponent<Toggle>();
        // ��ӹ�ѡ״̬�ı�ļ����¼�
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool value)
    {
        SetRenderersVisibility(targetObject, value);
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