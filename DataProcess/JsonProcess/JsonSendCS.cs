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
    public VideoPlayControler videoPlayControler;

    // 获取屏幕宽度  
    int screenWidth = Screen.width;  
    
    // 获取屏幕高度  
    int screenHeight = Screen.height;

    List<int> PersonLastID = new List<int>(); 
    List<int> PersonThisID = new List<int>();
    
    List<int> CarLastID = new List<int>(); 
    List<int> CarThisID = new List<int>();

    string collectedData;

    List<FrameInfo> personFrameInfo = new List<FrameInfo>();
    List<FrameInfo> carFrameInfo = new List<FrameInfo>();

    private int frameIndex = 0;
    public float ratio = 0.033f;
    public Vector2 resolution = new Vector2(1280.0f, 720.0f);
    string[] dataStrings;
    RiskList riskList = new RiskList();
    public bool withPerson;
    public bool noMark;
    public float gameDuration;
    public GameObject mask;

    void Start()  
    {  
        // Application.targetFrameRate = 30;

        videoPlayControler.PlayVideo();
        
        ReceiveFromPython();  

        processData();

        Invoke("EndGame", gameDuration);
        // StartCoroutine(sendMessage());
    }  

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        sendMessage();
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
        dataStrings = collectedData.Split(new string[] { "|$|" }, StringSplitOptions.None);
        
        // Debug.Log("Received Data: " + collectedData); 
    } 

    private void EndGame()  
    {  
        Time.timeScale = 0f;
        mask.SetActive(true);
    }  

    private void processData()
    {
        // 处理人
        string[] personStrings = dataStrings[0].Split(new string[] { "|*|" }, StringSplitOptions.None);
        // string riskString = "{\"risk\": " + personStrings[1] + "}";
        Debug.Log(personStrings[1]);
        riskList = JsonUtility.FromJson<RiskList>(personStrings[1]);
        
        for(int i = 2; i < int.Parse(personStrings[0]); i++)
        {  
            if(personStrings[i] == "")
            {
                break;
            }

            if(personStrings[i] == "null")
            {
                break;
            }

            FrameInfo singleFrame = new FrameInfo();
            singleFrame.riskList = new List<ListWrapper>();
            string[] riskStringsinFrame = personStrings[i].Split(new string[] { "|#|" }, StringSplitOptions.None);
            // Debug.Log(riskStringsinFrame);

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

            personFrameInfo.Add(singleFrame);
        }


        // 处理车
        string[] carStrings = dataStrings[1].Split(new string[] { "|*|" }, StringSplitOptions.None);

        
        
        for(int i = 0; i < int.Parse(personStrings[0]); i++)
        {  
            if(carStrings[i] == "")
            {
                break;
            }

            if(carStrings[i] == "null")
            {
                break;
            }

            FrameInfo singleFrame = new FrameInfo();
            singleFrame.riskList = new List<ListWrapper>();
            string[] riskStringsinFrame = carStrings[i].Split(new string[] { "|#|" }, StringSplitOptions.None);
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

            carFrameInfo.Add(singleFrame);
        }

    }  

    void sendMessage()
    {
        if(!noMark)
        {
            if(withPerson)
            {
                if(frameIndex < personFrameInfo.Count)
                {
                    foreach(ListWrapper risk in personFrameInfo[frameIndex].riskList)
                    {
                        if(risk == null)
                            continue;
                        Vector2 BboxBL = new Vector2((risk.list[0]/resolution.x)*screenWidth, 
                            screenHeight - (risk.list[1]/resolution.y)*screenHeight);
                        Vector2 BboxTR = new Vector2((risk.list[2]/resolution.x)*screenWidth, 
                            screenHeight - (risk.list[3]/resolution.y)*screenHeight);
                        int ID = risk.riskID;
                        Vector2 BboxCenter = (BboxBL + BboxTR)/2;
                        if(riskList.riskList.Contains(ID))
                        {
                            if(riskList.ending_frame[riskList.riskList.IndexOf(ID)] != -1 && riskList.starting_frame[riskList.riskList.IndexOf(ID)] != -1)
                            {
                                if(frameIndex < riskList.ending_frame[riskList.riskList.IndexOf(ID)] && frameIndex > riskList.starting_frame[riskList.riskList.IndexOf(ID)] )
                                {
                                    JsonMarkManager.SendMessage(new MarkMessage(true, ID, BboxBL, BboxTR));
                                    PersonThisID.Add(ID);
                                }
                            }
                            else
                            {
                                JsonMarkManager.SendMessage(new MarkMessage(true, ID, BboxBL, BboxTR));
                                    PersonThisID.Add(ID);
                            }
                        }
                    }

                    if(PersonLastID != null && PersonThisID != null)
                    {
                        foreach(int id in PersonLastID)
                        {
                            if(!PersonThisID.Contains(id))
                            {
                                DeleteMarkManager.SendMessage(true, id);
                            }
                        }
                    }

                    PersonLastID.Clear();
                    PersonLastID.AddRange(PersonThisID);
                    PersonThisID.Clear();
                }
            }
            
            if(frameIndex < carFrameInfo.Count)
            {
                foreach(ListWrapper risk in carFrameInfo[frameIndex].riskList)
                {
                    if(risk == null)
                        continue;
                    Vector2 BboxBL = new Vector2((risk.list[0]/resolution.x)*screenWidth, 
                        screenHeight - (risk.list[1]/resolution.y)*screenHeight);
                    Vector2 BboxTR = new Vector2((risk.list[2]/resolution.x)*screenWidth, 
                        screenHeight - (risk.list[3]/resolution.y)*screenHeight);
                    int ID = risk.riskID;
                    Vector2 BboxCenter = (BboxBL + BboxTR)/2;
                    Vector2 BboxScale = new Vector2(- BboxBL.x + BboxTR.x, BboxBL.y - BboxTR.y);
                    if(BboxCenter.y > screenHeight / 4 && BboxCenter.y < screenHeight * 3 / 4 && BboxScale.x > 360.0f && BboxScale.y > 180.0f)
                    {
                        JsonMarkManager.SendMessage(new MarkMessage(false, ID, BboxBL, BboxTR));
                        CarThisID.Add(ID);
                    }
                }


                if(CarLastID != null && CarThisID != null)
                {
                    foreach(int id in CarLastID)
                    {
                        if(!CarThisID.Contains(id))
                        {
                            DeleteMarkManager.SendMessage(false, id);
                        }
                    }
                }

                CarLastID.Clear();
                CarLastID.AddRange(CarThisID);
                CarThisID.Clear();
            }
        }
        frameIndex ++;
        // yield return new WaitForSeconds(ratio);

        Debug.Log("This Frame:"+frameIndex);
        
        // if(frameIndex % 3 == 0)
        // {
        //     // yield return new WaitForSeconds(0.01f);
        // }
        
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
    
    [System.Serializable]
    public class RiskList
    {
        public List<int> riskList;
        public List<int> starting_frame;
        
        public List<int> ending_frame;
    }
}  