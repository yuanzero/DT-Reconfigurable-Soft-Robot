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

    private string pinNumber = "13";            // �洢���ź���
    private bool isOutputMode = true;        // �洢����ģʽ������������
    private int analogOutputValue;    // �洢����ģ�����ֵ

    public TrackingCtrlUDP trackingCtrlUDP;

    public Toggle arduinoToggles;     // �����л� arduino��esp32 �Ŀ���

    public TextMeshProUGUI[] Sensor_Data; //������ʾsensor����

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

                        StartCoroutine(SendUDPMessagesWithDelay()); //����ѭ����ȡ

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
        string[] words = message.Split(' ', '\n'); // �Կո�ָ��ַ���
        int index = -1;
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i] == "readpin")
            {
                index = i;
                break;
            }
        }
        if (index != -1 && index + 2 < words.Length) // ȷ���ҵ���readpin���Һ������㹻������
        {
            string Pin = words[index + 1]; // ��ȡ��һ������
            string PinData = words[index + 2]; // ��ȡ�ڶ�������

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
                // ��������Խ�number���������������������
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
