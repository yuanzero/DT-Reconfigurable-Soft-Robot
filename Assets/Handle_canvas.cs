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

    private string pinNumber = "13";            // 存储引脚号码
    private bool isOutputMode = true;        // 存储引脚模式（输入或输出）
    private int analogOutputValue;    // 存储引脚模拟输出值

    public TrackingCtrlUDP trackingCtrlUDP;

    public Toggle arduinoToggles;     // 用于切换 arduino和esp32 的开关

    public TextMeshProUGUI[] Sensor_Data; //用于显示sensor输入

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
            if (arduinoToggles.isOn)
            {
                //trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "mr " + pinNumber);
               

            }
            else
            {

                trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "r " + pinNumber);
            }

            string read_data = trackingCtrlUDP.Read_data();
            Sensor_reader.text = read_data;
            if (read_data != null)
            {
                Recieve_data_process(read_data);
            }

        }
    }

    IEnumerator SendUDPMessagesWithDelay()
    {
        while (!isOutputMode)
        {
            trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "mr " + "A0");
            yield return new WaitForSeconds(0.2f);
            trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "mr " + "A1");
            yield return new WaitForSeconds(0.2f);
            trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "mr " + "A2");
            yield return new WaitForSeconds(0.2f);
        }
    }

    void OnPinNumberChanged(string pin)
    {
        pinNumber = pinInputField.text;
        // Initialize
        //UduinoManager.Instance.pinMode(pinNumber, PinMode.Output);
        trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "s " + pinNumber + " 0");
        analogOutputValue = 0;
    }


    void OnPinModeButtonClick_high()
    {
        pinNumber = pinInputField.text;

        if (isOutputMode)
        {
            analogOutputValue = 255;
            send_ESP32();
        }

        Debug.Log("OnPinModeButtonClick_high ");
    }

    void OnPinModeButtonClick_low()
    {
        pinNumber = pinInputField.text;

        if (isOutputMode)
        {
            analogOutputValue = 0;
            send_ESP32();
        }

    }

    void OnAnalogOutputSliderValueChanged(float value)
    {
        pinNumber = pinInputField.text;
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

                        StartCoroutine(SendUDPMessagesWithDelay()); //激活循环读取

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

            trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "ma " + pinNumber + " " +
                    analogOutputValue.ToString());
        }
        else
        {
            trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "a " + pinNumber + " " +
                    analogOutputValue.ToString());
        }
    }

    public void Recieve_data_process(string read_data)
    {
        string message = read_data;
        string[] words = message.Split(' ', '\n'); // 以空格分割字符串
        int index = -1;
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i] == "readpin")
            {
                index = i;
                break;
            }
        }
        if (index != -1 && index + 2 < words.Length) // 确保找到了readpin并且后面有足够的数据
        {
            string Pin = words[index + 1]; // 获取第一个数据
            string PinData = words[index + 2]; // 获取第二个数据

            if (Pin == "A0\r")
            {
                Sensor_Data[0].text = PinData;
            }
            else if (Pin == "A1\r")
            {
                Sensor_Data[1].text = PinData;
            }
            else if (Pin == "A2\r")
            {
                Sensor_Data[2].text = PinData;
            }
            else
            {
                //Debug.Log("Pin: " + Pin + " " + PinData);
            }
            
        }

        /*
        string searchString = "read is: ";
        int startIndex = receivedMessage.IndexOf(searchString);
        if (startIndex != -1)
        {
            startIndex += searchString.Length;
            string numberString = receivedMessage.Substring(startIndex);
            int number;
            Sensor_Data.text = numberString;
            if (int.TryParse(numberString, out number))
            {
                //Debug.Log("Extracted number: " + number);
                // 在这里可以将number保存下来或进行其他处理
                number = int.Parse(numberString);
            }
            else
            {
                Debug.Log("Failed to parse the number.");
            }
        }
        else
        {
            Debug.Log("The message does not contain the desired string.");
        }
        */
    }
}
