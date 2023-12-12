using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
//using static trackingCtrlUDP;

public class Handle_canvas : MonoBehaviour
{
    public TMP_InputField pinInputField;  // �����û��������ź���������
    public TMP_InputField Sensor_reader;  // ���ڴ���������
    public Button pinModeButton_LOW;      // �����л�����ģʽ�İ�ť
    public Button pinModeButton_HIGH;      // �����л�����ģʽ�İ�ť
    public Toggle[] optionToggles;     // �����л�����ģʽ�İ�ť
    public Slider analogOutputSlider; // ���ڿ�������ģ�����ֵ�Ļ���

    private int pinNumber = 13;            // �洢���ź���
    private bool isOutputMode = true;        // �洢����ģʽ������������
    private int analogOutputValue;    // �洢����ģ�����ֵ

    public TrackingCtrlUDP trackingCtrlUDP;

    public Toggle arduinoToggles;     // �����л� arduino��esp32 �Ŀ���

    // Start is called before the first frame update
    void Start()
    {
        pinModeButton_LOW.onClick.AddListener(OnPinModeButtonClick_low);
        pinModeButton_HIGH.onClick.AddListener(OnPinModeButtonClick_high);
        analogOutputSlider.onValueChanged.AddListener(OnAnalogOutputSliderValueChanged);

        //pinInputField_text = GetComponent<InputField>();
        pinInputField.onValueChanged.AddListener(OnPinNumberChanged);

        // ���ѡ��Toggle��״̬�仯������
        foreach (Toggle toggle in optionToggles)
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        // ����pin
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
        // ��������ѡ��Toggle
        for (int i = 0; i < optionToggles.Length; i++)
        {
            // ���ѡ�е�Toggle
            if (optionToggles[i].isOn)
            {
                // ִ����Ӧ�Ĳ���
                switch (i)
                {
                    case 0:
                        // ��һ��ѡ�ѡ�У�ִ�в���1
                        isOutputMode = true;
                        //UduinoManager.Instance.pinMode(pinNumber, PinMode.Output);
                        trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "s 13 0");
                        Debug.Log("Option 1 is selected.");
                        break;
                    case 1:
                        // �ڶ���ѡ�ѡ�У�ִ�в���2
                        isOutputMode = false;
                        //UduinoManager.Instance.pinMode(pinNumber, PinMode.Input);
                        trackingCtrlUDP.SendUDPMessage(trackingCtrlUDP.ServerIP, trackingCtrlUDP.OutPort, "s 13 1");
                        Debug.Log("Option 2 is selected.");
                        break;
                        // ���Լ����������ѡ��Ĳ���
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
