using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System;

public class ESP32Communication : MonoBehaviour
{
    private TcpClient client;
    private StreamReader reader;
    private StreamWriter writer;

    private string serverIP = "192.168.1.11"; // ESP32的IP地址
    private int serverPort = 80; // ESP32的端口号

    void Start()
    {
        ConnectToESP32();
        SendMessageToESP32("hallow");
        StartCoroutine(ReceiveMessageFromESP32());
    }

    void ConnectToESP32()
    {
        try
        {
            client = new TcpClient(serverIP, serverPort);
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            Debug.Log("Connected to ESP32");
        }
        catch (Exception e)
        {
            Debug.Log("Failed to connect to ESP32: " + e.Message);
        }
    }

    void SendMessageToESP32(string message)
    {
        if (writer != null)
        {
            writer.WriteLine(message);
            writer.Flush();
        }
    }

    IEnumerator ReceiveMessageFromESP32()
    {
        while (true)
        {
            if (reader != null && reader.BaseStream.CanRead)
            {
                string message = reader.ReadLine();
                if (!string.IsNullOrEmpty(message))
                {
                    Debug.Log("Received message from ESP32: " + message);
                    // 在这里处理接收到的消息
                }
            }
            yield return null;
        }
    }

    void OnApplicationQuit()
    {
        if (client != null && client.Connected)
        {
            reader.Close();
            writer.Close();
            client.Close();
        }
    }
}
