using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//using static trackingCtrlUDP;

public class Handle_canvas : MonoBehaviour
{
    public TMP_InputField pinInputField;  // 用于用户输入引脚号码的输入框
    public TMP_InputField Sensor_reader;  // 用于传感器输入
    public Button pinModeButton_LOW;      // 用于切换引脚模式的按钮
    public Button pinModeButton_HIGH;      // 用于切换引脚模式的按钮
    public Toggle[] optionToggles;     // 用于切换引脚模式的按钮
    public Slider analogOutputSlider; // 用于控制引脚模拟输出值的滑块

    private int pinNumber = 13;            // 存储引脚号码
    private bool isOutputMode = true;        // 存储引脚模式（输入或输出）
    private int analogOutputValue;    // 存储引脚模拟输出值

    public TrackingCtrlUDP trackingCtrlUDP;

    public Toggle arduinoToggles;     // 用于切换 arduino和esp32 的开关

    // Start is called before the first frame update
    void Start()
    {
        pinModeButton_LOW.onClick.AddListener(OnPinModeButtonClick_low);
        pinModeButton_HIGH.onClick.AddListener(OnPinModeButtonClick_high);
        analogOutputSlider.onValueChanged.AddListener(OnAnalogOutputSliderValueChanged);

        //pinInputField_text = GetComponent<InputField>();
        pinInputField.onValueChanged.AddListener(OnPinNumberChanged);

        // 添加选项Toggle的状态变化监听器
        foreach (Toggle toggle in optionToggles)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        // 激活pin
       // trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "s 13 0");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isOutputMode)
        {
            //UduinoManager.Instance.analogWrite(pinNumber, analogOutputValue);
            
        }
        else
        {
            //int read_data = UduinoManager.Instance.analogRead(pinNumber);
            trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "r " + pinNumber.ToString());
            string read_data = trackingCtrlUDP.Read_data();
            Sensor_reader.text = read_data;
        }
    }

    void OnPinNumberChanged(string pin)
    {
        pinNumber = int.Parse(pinInputField.text);
        // Initialize
        //UduinoManager.Instance.pinMode(pinNumber, PinMode.Output);
        trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "s " + pinNumber.ToString() + " 0");
        analogOutputValue = 0;
    }


    void OnPinModeButtonClick_high()
    {
        pinNumber = int.Parse(pinInputField.text);

        if (isOutputMode)
        {
            analogOutputValue = 255;
            send_ESP32();
        }

        Debug.Log("OnPinModeButtonClick_high ");
    }

    void OnPinModeButtonClick_low()
    {
        pinNumber = int.Parse(pinInputField.text);

        if (isOutputMode)
        {
            analogOutputValue = 0;
            send_ESP32();
        }

    }

    void OnAnalogOutputSliderValueChanged(float value)
    {
        pinNumber = int.Parse(pinInputField.text);
        analogOutputValue = Mathf.RoundToInt(value * 255);
        send_ESP32();
    }

    private void OnToggleValueChanged(bool value)
    {
        // 遍历所有选项Toggle
        for (int i = 0; i < optionToggles.Length; i++)
        {
            // 检查选中的Toggle
            if (optionToggles[i].isOn)
            {
                // 执行相应的操作
                switch (i)
                {
                    case 0:
                        // 第一个选项被选中，执行操作1
                        isOutputMode = true;
                        //UduinoManager.Instance.pinMode(pinNumber, PinMode.Output);
                        trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "s 13 0");
                        Debug.Log("Option 1 is selected.");
                        break;
                    case 1:
                        // 第二个选项被选中，执行操作2
                        isOutputMode = false;
                        //UduinoManager.Instance.pinMode(pinNumber, PinMode.Input);
                        trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "s 13 1");
                        Debug.Log("Option 2 is selected.");
                        break;
                        // 可以继续添加其他选项的操作
                }
            }
        }
    }

    public void send_ESP32()
    {
        if (arduinoToggles.isOn)
        {

            trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "ma " + pinNumber.ToString() + " " +
                    analogOutputValue.ToString());
        }
        else
        {
            trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "a " + pinNumber.ToString() + " " +
                    analogOutputValue.ToString());
        }
    }
}
