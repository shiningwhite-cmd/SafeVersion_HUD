using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using UnityEngine;  
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Text.RegularExpressions; 

public class JsonMessageGet : MonoBehaviour  
{  
    public string pythonHost = "localhost"; // Python文件的主机名  
    public int pythonPort = 1111; // Python文件的端口号  
  
    // 获取屏幕宽度  
    int screenWidth = Screen.width;  
    
    // 获取屏幕高度  
    int screenHeight = Screen.height; 

    private Socket clientSocket;  
  
    private TcpClient client;  
    private NetworkStream stream; 
    int maxCarIndex = 0;
    int maxPersonIndex = 0;
    public float ratio = 0.04f;
    public Vector2 resolution = new Vector2(1280.0f, 720.0f);

    IEnumerator Start()  
    {   
        while (true)  
        {  
            SendToPython("true");  
            
            StartCoroutine(ReceiveFromPython()); 

            yield return new WaitForSeconds(0.5f);
            // Debug.Log("frame:" + frame);
            // frame++;
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
        // 解析接收到的JSON字符串   
        List<ListWrapper> receivedData = new List<ListWrapper>();       
        byte[] buffer = new byte[1024];  
        int bytesRead = stream.Read(buffer, 0, buffer.Length);  
        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead); 
        int carIndex = 0;
        int personIndex = 0;

        string[] jsonStrings = data.Split(new string[] { "|*|" }, StringSplitOptions.None); 
        string[] personStrings = jsonStrings[1].Split(new string[] { "|#|" }, StringSplitOptions.None);
        string[] carStrings = jsonStrings[0].Split(new string[] { "|#|" }, StringSplitOptions.None);  
        // Debug.Log(jsonStrings[0]); 
        Debug.Log(jsonStrings[1]);

        
        foreach (string carString in carStrings)  
        {  
            if(carString == "")
            {
                // Debug.Log("CarBreak");
                break;
            }

            if(carString == "null")
            {
                // Debug.Log("CarNone");
                break;
            }

            // 解析接收到的JSON字符串  
            // ListWrapper listWrapper = JsonUtility.FromJson<ListWrapper>(carString); 
            List<float> listWrapper = HandleData(carString);
            // Debug.Log(listWrapper);
            Vector2 BboxBL = new Vector2((listWrapper[0]/resolution.x)*screenWidth, screenHeight - (listWrapper[1]/resolution.y)*screenHeight);
            
            Vector2 BboxTR = new Vector2((listWrapper[2]/resolution.x)*screenWidth, screenHeight - (listWrapper[3]/resolution.y)*screenHeight);
            
            JsonMarkManager.SendMessage(new MarkMessage(false, carIndex, BboxBL, BboxTR));
            carIndex++;
            
            if(maxCarIndex < carIndex)
                maxCarIndex = carIndex;
        }

        if(carIndex < maxCarIndex)
        {
            for(int i = carIndex; i < maxCarIndex; i++)
            {
                // Debug.Log(maxCarIndex);
                DeleteMarkManager.SendMessage(false, i);
            }
            maxCarIndex = carIndex;
        }
        
        foreach (string personString in personStrings)  
        {  
            Debug.Log(personIndex);
            if(personString == "")
            {
                Debug.Log("Personbreak");
                break;
            }

            if(personString == "null")
            {
                Debug.Log("PersonNone");
                break;
            }

            // 解析接收到的JSON字符串  
            // ListWrapper listWrapper = JsonUtility.FromJson<ListWrapper>(personString); 
            List<float> listWrapper = HandleData(personString);
            // Debug.Log(personString); 
            Vector2 BboxBL = new Vector2((listWrapper[0]/resolution.x)*screenWidth, screenHeight - (listWrapper[1]/resolution.y)*screenHeight);
            Vector2 BboxTR = new Vector2((listWrapper[2]/resolution.x)*screenWidth, screenHeight - (listWrapper[3]/resolution.y)*screenHeight);
            // Debug.Log("Send2Person");
            Vector2 diagonal = (BboxBL - BboxTR);
            diagonal.x = -diagonal.x;
            if(diagonal.x > screenWidth * ratio && diagonal.y > screenHeight * ratio )
            {
                JsonMarkManager.SendMessage(new MarkMessage(true, personIndex, BboxBL, BboxTR));
                personIndex++;
            }
            if(maxPersonIndex < personIndex)
                maxPersonIndex = personIndex;
        }
        if(personIndex < maxPersonIndex)
        {
            for(int i = personIndex; i < maxPersonIndex; i++)
            {
                DeleteMarkManager.SendMessage(true, i);
            }
            maxPersonIndex = personIndex;
        }

  
        yield return null;  
    }   
  
    void OnApplicationQuit()  
    {  
        if (client != null)  
        {  
            Debug.Log("Close");
            client.Close();  
        }  
    }  

    private List<float> HandleData(string jsonString)
    {
        List<float> dataList = new List<float>();  
  
        MatchCollection matches = Regex.Matches(jsonString, @"[-+]?\d*\.?\d+");  
        
        foreach (Match match in matches)  
        {  
            float value;  
            if (float.TryParse(match.Value, out value))  
            {  
                dataList.Add(value);  
            }  
        }  

        return dataList;
    }

    [System.Serializable]  
    private class ListWrapper  
    {  
        public float[] list;  
    }

      
}  