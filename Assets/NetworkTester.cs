using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkTester : MonoBehaviour
{
    public string arduinoIpAddress = "192.168.1.11"; // Arduino的IP地址
    public int arduinoPort = 80; // Arduino的端口号

    private TcpClient client;
    private NetworkStream stream;
    private bool isReceivingMessages = true; // 控制接收消息的循环

    void Start()
    {
        // 创建TcpClient连接
        client = new TcpClient();
        client.Connect(arduinoIpAddress, arduinoPort);
        Debug.Log("Connected to Arduino");

        // 获取网络流
        stream = client.GetStream();
    }

    void OnDestroy()
    {
        // 关闭连接
        stream.Close();
        client.Close();
    }

    void Update()
    {
        // 在适当的时候启动和停止接收消息的循环
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
        // 接收消息的循环
        if (isReceivingMessages)
        {
            // 接收来自Arduino的消息
            byte[] receiveBuffer = new byte[1024];
            int bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length);
            string receivedMessage = System.Text.Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);
            Debug.Log("Received message from Arduino: " + receivedMessage);

            // 在这里可以对接收到的消息进行处理

            // 清空缓冲区
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
