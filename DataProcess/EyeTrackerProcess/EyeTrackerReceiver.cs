using UnityEngine;  
using System;  
using System.Net.Sockets;  
using System.Text;  
using System.Net;
using System.Net.NetworkInformation; 
  
public class EyeTrackerReceiver : MonoBehaviour  
{  
    // 获取屏幕宽度  
    int screenWidth = Screen.width;  
    
    // 获取屏幕高度  
    int screenHeight = Screen.height;  


    public static string serverAddress = "localhost";  
    public static int serverPort = 12345;  
  
    private TcpClient client;  
    private NetworkStream stream;  
    private byte[] buffer = new byte[1024];  
  
    private void Start()  
    {  
        // 连接到Python服务器  
        if(CheckPortInUse(serverPort))
        {
            client = new TcpClient(serverAddress, serverPort);  
            Debug.Assert(client!=null, "do not connect to serverAddress");
            stream = client.GetStream();  
    
            // 开始异步读取数据  
            stream.BeginRead(buffer, 0, buffer.Length, OnReceiveData, null); 
        }
        else
            Debug.Log("Port has been used!!"); 
    }  
  
    private void OnReceiveData(IAsyncResult ar)  
    {  
        try  
        {  
            // 结束异步读取  
            int bytesRead = stream.EndRead(ar);  
  
            if (bytesRead > 0)  
            {  
                // 解析接收到的数据  
                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);  
                string[] coordinates = data.Split(',');  
  
                if (coordinates.Length == 2)  
                {  
                    // 获取眼动注视点的坐标  
                    float x = float.Parse(coordinates[0]);  
                    float y = float.Parse(coordinates[1]);  
  
                    // 在这里可以根据需要处理坐标数据，例如控制对象的移动或视角等  
                    // ...  
                    //Debug.Log("Received coordinates - X: " + (960 - x/2) + ", Y: " + (490-y/2));         
                    EyeTrackManager.SendMessage(new Vector2((x/1920.0f)*screenWidth, screenHeight - (y/1080.0f)*screenHeight));
                    // 继续异步读取数据  
                    stream.BeginRead(buffer, 0, buffer.Length, OnReceiveData, null);  
                }  
            }  
        }  
        catch (Exception e)  
        {  
            Debug.LogError("Error receiving data: " + e.Message);  
        }  
    }  
  
    private void OnDestroy()  
    {  
        // 关闭连接  
        stream.Close();  
        client.Close();  
    }  

    private bool CheckPortInUse(int port)  
    {  
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();  
        IPEndPoint[] activeListeners = ipGlobalProperties.GetActiveTcpListeners();  
  
        foreach (IPEndPoint listener in activeListeners)  
        {  
            if (listener.Port == port)  
            {  
                return true;  
            }  
        }  
  
        return false;  
    } 
}  