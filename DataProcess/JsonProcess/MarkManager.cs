using System.IO;  
using UnityEngine;  
using System.Collections.Generic;  
  
// [System.Serializable]  
// public class FrameData  
// {  
//     public float Time;  
//     public string Text;  
//     public Dictionary<string, Dictionary<string, object>> Pedestrian;  
//     public Dictionary<string, Dictionary<string, object>> Vehicle;  
// }  
  
// [System.Serializable]  
// public class RootData  
// {  
//     public Dictionary<string, FrameData> Frames;  
// }  
  

public class MarkManager : MonoBehaviour
{  
    [SerializeField] private string jsonFilePath = "./Assets/JsonProcess/JsonFiles/Bbox_Video_0194_.json"; // JSON文件的路径  
  
    private void Start()  
    {  
        if (File.Exists(jsonFilePath))  
        {  
            string jsonContent = File.ReadAllText(jsonFilePath);  
  
            // 使用JsonUtility进行反序列化  
            // DataContainer dataContainer = JsonUtility.FromJson<DataContainer>(jsonContent);
            // //Debug.Log(jsonContent);  
   
            // Debug.Log(dataContainer);
            // // 访问数据  
            // if (dataContainer != null)  
            // {  
            //     foreach (var item in dataContainer.data)  
            //     {  
            //         BoundingBoxes box = item.bbox;  
            //         float[] box0 = box.box0;
            //         float[] box1 = box.box1;
            //         float[] box2 = box.box2;
            //         float[] box3 = box.box3;
            //         float[] box4 = box.box4;
            //         float[] box5 = box.box5;
            
            //         Debug.Log(box0);
            //         Debug.Log(box1);
            //         Debug.Log(box2);
            //         Debug.Log(box3);
            //         Debug.Log(box4);
            //         Debug.Log(box5);
            //     }  
            // }  
            // else
            // {
            //     Debug.LogError("JSON file has not data: " + jsonFilePath);  
            // }

            DataContainer dataContainer =  JsonUtility.FromJson<DataContainer>(jsonContent);
            float[] b1 = dataContainer.data1.bbox.box0;
            Debug.Log(b1);

        }  
        else  
        {  
            Debug.LogError("JSON file not found at path: " + jsonFilePath);  
        }  
    }  
  
    // JSON数据的数据结构  
    [System.Serializable]  
    private class DataContainer  
    {  
        public DataItem data1;   
    }  
  
    [System.Serializable]  
    private class DataItem  
    {  
        public BoundingBoxes bbox;  
    }  
  
    [System.Serializable]  
    private class BoundingBoxes  
    {  
        public float[] box0;
        public float[] box1;
        public float[] box2;
        public float[] box3;
        public float[] box4;
        public float[] box5;

    }


    ///////////////////////////////
    // [System.Serializable]  
    // public class BoundingBox  
    // {  
    //     public float[] values;  
    // }  
    
    // [System.Serializable]  
    // public class MyData  
    // {  
    //     public Dictionary<string, BoundingBox> bbox;  
    // }  

}
