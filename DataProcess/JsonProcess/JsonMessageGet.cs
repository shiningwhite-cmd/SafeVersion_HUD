using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using UnityEngine;  
using System.Collections;
  
public class JsonMessageGet : MonoBehaviour  
{  
    public string pythonHost = "localhost"; // Python文件的主机名  
    public int pythonPort = 1111; // Python文件的端口号  
  
    private Socket clientSocket;  
  
    private TcpClient client;  
    private NetworkStream stream; 
    // private void Start()  
    // {  
    //     // 创建Socket对象  
    //     clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
          
    //     // 连接服务器  
    //     IPAddress serverAddress = IPAddress.Parse(serverIP);  
    //     IPEndPoint serverEndPoint = new IPEndPoint(serverAddress, serverPort);  
    //     clientSocket.Connect(serverEndPoint);  
          
         
    // }  


    IEnumerator Start()  
    {   
        while (true)  
        {  
            SendToPython("true");  
  
            yield return new WaitForSeconds(1f); // 每隔1秒发送一次信息  
        }    
    }  

    void SendToPython(string message)  
    {  
        try  
        {  
            if (client == null || !client.Connected)  
            {  
                client = new TcpClient(pythonHost, pythonPort);  
                stream = client.GetStream();  
            }  
  
            byte[] data = Encoding.UTF8.GetBytes(message);  
            stream.Write(data, 0, data.Length);  
        }  
        catch (SocketException e)  
        {  
            Debug.LogError("发送信息失败: " + e.Message);  
        }  
    }  
  
    void OnApplicationQuit()  
    {  
        if (client != null)  
        {  
            client.Close();  
        }  
    }  
}  