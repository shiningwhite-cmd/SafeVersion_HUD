using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using UnityEngine;  
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Text.RegularExpressions; 

public class JsonSendCS : MonoBehaviour  
{    

    TcpClient client;  
    NetworkStream stream;  
    StringBuilder receivedData;

    // 获取屏幕宽度  
    int screenWidth = Screen.width;  
    
    // 获取屏幕高度  
    int screenHeight = Screen.height;

    List<int> LastID = new List<int>(); 
    List<int> ThisID = new List<int>();

    string collectedData;

    List<FrameInfo> frameInfo = new List<FrameInfo>();

    private int frameIndex = 0;
    public float ratio = 0.03333f;
    public Vector2 resolution = new Vector2(1280.0f, 720.0f);

    void Start()  
    {  
        ReceiveFromPython();  

        processData();

        StartCoroutine(sendMessage());
    }  

    private void ReceiveFromPython()  
    {  


        // 创建TCP客户端  
        client = new TcpClient();  
  
        // 目标IP和端口  
        string targetIP = "localhost";  
        int targetPort = 11111;  
  
        // 连接到目标主机  
        client.Connect(targetIP, targetPort);  
  
        // 获取网络流  
        stream = client.GetStream();  
  
        // 初始化StringBuilder  
        receivedData = new StringBuilder();  
  
        // 接收数据  
        byte[] receiveBuffer = new byte[1024];  
        int bytesRead = 0;  
  
        while ((bytesRead = stream.Read(receiveBuffer, 0, receiveBuffer.Length)) > 0)  
        {  
            // 将接收到的数据块添加到StringBuilder  
            receivedData.Append(Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead));  
        }  
        
        // 关闭流和客户端  
        stream.Close();  
        client.Close(); 
  
        // 汇集的数据  
        collectedData = receivedData.ToString();  
        // Debug.Log("Received Data: " + collectedData); 
    } 


    private void processData()
    {
        string[] personStrings = collectedData.Split(new string[] { "|*|" }, StringSplitOptions.None);
        
        
        for(int i = 1; i < int.Parse(personStrings[0]); i++)
        {  
            if(personStrings[i] == "")
            {
                Debug.Log("Personbreak");
                break;
            }

            if(personStrings[i] == "null")
            {
                Debug.Log("PersonNone");
                break;
            }

            Debug.Log("personStrings[i]="+personStrings[i]);
            FrameInfo singleFrame = new FrameInfo();
            singleFrame.riskList = new List<ListWrapper>();
            string[] riskStringsinFrame = personStrings[i].Split(new string[] { "|#|" }, StringSplitOptions.None);
            Debug.Log(riskStringsinFrame);

            foreach(string riskStringinFrame in riskStringsinFrame)
            {
                if(riskStringinFrame == "")
                {
                    break;
                }

                if(riskStringinFrame == "null")
                {
                    break;
                }

                ListWrapper Info = new ListWrapper();
                Info = JsonUtility.FromJson<ListWrapper>(riskStringinFrame);
                

                singleFrame.riskList.Add(Info);

            }

            frameInfo.Add(singleFrame);
        }

    }  

    IEnumerator sendMessage()
    {
        while(true)
        {
            foreach(ListWrapper risk in frameInfo[frameIndex].riskList)
            {
                if(risk == null)
                    continue;
                Vector2 BboxBL = new Vector2((risk.list[0]/resolution.x)*screenWidth, 
                    screenHeight - (risk.list[1]/resolution.y)*screenHeight);
                Vector2 BboxTR = new Vector2((risk.list[2]/resolution.x)*screenWidth, 
                    screenHeight - (risk.list[3]/resolution.y)*screenHeight);
                int ID = risk.riskID;
                JsonMarkManager.SendMessage(new MarkMessage(true, ID, BboxBL, BboxTR));
                ThisID.Add(ID);
            }

            if(LastID != null && ThisID != null)
            {
                foreach(int id in LastID)
                {
                    if(!ThisID.Contains(id))
                    {
                        DeleteMarkManager.SendMessage(true, id);
                    }
                }
            }

            LastID.Clear();
            LastID.AddRange(ThisID);
            ThisID.Clear();
            
            frameIndex ++;
            yield return new WaitForSeconds(ratio);
        }
    }

    private void endReceive()
    {
        
        // 关闭流和客户端  
        stream.Close();  
        client.Close();  
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
    public class ListWrapper  
    {  
        public int riskID;
        public float[] list;  
    }

    [System.Serializable]
    public class FrameInfo
    {
        public List<ListWrapper> riskList;
    }
}  