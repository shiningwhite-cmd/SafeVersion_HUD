using System.IO;  
using UnityEngine;  
using System.Collections.Generic;  

public class JsonRead : MonoBehaviour
{  
    [SerializeField] private string jsonFilePath = "./Assets/JsonProcess/JsonFiles/Json_Test.json"; // JSON文件的路径  
  
    private void Start()  
    {  
        if (File.Exists(jsonFilePath))  
        {  
            string jsonContent = File.ReadAllText(jsonFilePath);  
  
            MyData data = JsonUtility.FromJson<MyData>(jsonContent);  
  
            // 现在您可以访问解析后的数据  
            Debug.Log("Numbers: " + string.Join(", ", data.numbers));  
            Debug.Log("Checked: " + data.checkeds);  
            Debug.Log("ID: " + data.id);  
            Debug.Log("Object - T: " + data.objectData.t);  
            Debug.Log("Object - W: " + data.objectData.w);  
            Debug.Log("Host: " + data.host);  
        }  
        else  
        {  
            Debug.LogError("JSON file not found at path: " + jsonFilePath);  
        }  
    }  
  
    // JSON数据的数据结构  
    [System.Serializable]  
    public class MyData  
    {  
        public int[] numbers;  
        public bool checkeds;  
        public int id;  
        public MyObject objectData;  
        public string host;  
    }  
    
    [System.Serializable]  
    public class MyObject  
    {  
        public string t;  
        public string w;  
    }  

}
