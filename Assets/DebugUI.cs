using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Profiling;
using System.IO;

public class DebugUI : MonoBehaviour
{
    public TextMeshProUGUI debugText; // 用于显示调试信息的文本框

    private bool isDebugVisible = true; // 调试界面是否可见

    private void Start()
    {
        debugText.gameObject.SetActive(true); // 初始时隐藏调试文本框
    }

    private void Update()
    {
        // 检测用户输入或其他触发条件来显示/隐藏调试界面
        if (Input.GetKeyDown(KeyCode.D))
        {
            isDebugVisible = !isDebugVisible; // 切换调试界面的可见性

            debugText.gameObject.SetActive(isDebugVisible);
        }
    }

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 在这里处理Debug.Log的命令，例如打印到控制台或写入日志文件

        debugText.text += logString + "\n";
    }
}
