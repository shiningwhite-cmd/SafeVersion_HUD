using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using UnityEngine;  
using System.Collections;
using System.Collections.Generic; 
  
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
            
            yield return StartCoroutine(ReceiveFromPython()); 

            yield return new WaitForSeconds(0.3333f);
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

    IEnumerator ReceiveFromPython()  
    {  
        List<ListWrapper> receivedData = new List<ListWrapper>();       
        byte[] buffer = new byte[1024];  
        int bytesRead = stream.Read(buffer, 0, buffer.Length);  
        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);  
        // Debug.Log(data);
  
        // 解析接收到的JSON字符串  
        // ListWrapper listWrapper = JsonUtility.FromJson<ListWrapper>(data);  
        // receivedData.Add(listWrapper);  
        string[] jsonStrings = data.Split(new string[] { "|#|" }, StringSplitOptions.None);  
        int index = 0;
        int maxindex = 0;
  
        foreach (string jsonString in jsonStrings)  
        {  
            if(maxindex < index)
                maxindex = index;
            // 解析接收到的JSON字符串  
            ListWrapper listWrapper = JsonUtility.FromJson<ListWrapper>(jsonString);  
            Vector2 BboxBL = new Vector2(listWrapper.list[0], 1080 - listWrapper.list[1]);
            Vector2 BboxTR = new Vector2(listWrapper.list[2], 1080 - listWrapper.list[3]);
            JsonMarkManager.SendMessage(new MarkMessage(index, BboxBL, BboxTR));
            index++;
            // foreach (float num in listWrapper.list)  
            // {  
            //     Debug.Log(num);  
            // }  
        }  

        if(index<maxindex)
        {
            for(int i = index; i <= maxindex; i++)
            {
                DeleteMarkManager.SendMessage(i);
            }
        }
  
        yield return null;  
    }   
  
    void OnApplicationQuit()  
    {  
        if (client != null)  
        {  
            client.Close();  
        }  
    }  

    [System.Serializable]  
    private class ListWrapper  
    {  
        public float[] list;  
    }

      
}  