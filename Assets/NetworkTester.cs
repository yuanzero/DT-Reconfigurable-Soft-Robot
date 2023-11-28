using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkTester : MonoBehaviour
{
    public string arduinoIpAddress = "192.168.1.11"; // Arduino��IP��ַ
    public int arduinoPort = 80; // Arduino�Ķ˿ں�

    private TcpClient client;
    private NetworkStream stream;
    private bool isReceivingMessages = true; // ���ƽ�����Ϣ��ѭ��

    void Start()
    {
        // ����TcpClient����
        client = new TcpClient();
        client.Connect(arduinoIpAddress, arduinoPort);
        Debug.Log("Connected to Arduino");

        // ��ȡ������
        stream = client.GetStream();
    }

    void OnDestroy()
    {
        // �ر�����
        stream.Close();
        client.Close();
    }

    void Update()
    {
        // ���ʵ���ʱ��������ֹͣ������Ϣ��ѭ��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isReceivingMessages = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            isReceivingMessages = false;
        }

        else if (Input.GetKeyDown(KeyCode.Return))
        {
            SendToArduino("Hello Arduino!");
        }
    }

    void FixedUpdate()
    {
        // ������Ϣ��ѭ��
        if (isReceivingMessages)
        {
            // ��������Arduino����Ϣ
            byte[] receiveBuffer = new byte[1024];
            int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
            string receivedMessage = System.Text.Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);
            Debug.Log("Received message from Arduino: " + receivedMessage);

            // ��������ԶԽ��յ�����Ϣ���д���

            // ��ջ�����
            Array.Clear(receiveBuffer, 0, receiveBuffer.Length);
        }
    }

    public void SendToArduino(string message)
    {
        if (client != null && client.Connected)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Debug.Log("Message sent to Arduino: " + message);
            }
            catch (Exception e)
            {
                Debug.Log("Failed to send message to Arduino: " + e.Message);
            }
        }
        else
        {
            Debug.Log("Not connected to Arduino");
        }
    }
}
