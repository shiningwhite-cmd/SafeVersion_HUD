// using System.IO;  
// using UnityEngine;  
// using System.Collections.Generic;  
  
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
  

// public class MarkManager : MonoBehaviour
// {  
//     private void Start()  
//     {  
//         string json = " "; // 读取JSON文件内容，可以使用File.ReadAllText方法或者其他方式获取文件内容  
  
//         RootData rootData = JsonUtility.FromJson<RootData>(json);  
//         if (rootData != null)  
//         {  
//             foreach (KeyValuePair<string, FrameData> frame in rootData.Frames)  
//             {  
//                 string frameKey = frame.Key;  
//                 FrameData frameData = frame.Value;  
//                 float time = frameData.Time;  
//                 string text = frameData.Text;  
//                 Dictionary<string, Dictionary<string, object>> pedestrian = frameData.Pedestrian;  
//                 Dictionary<string, Dictionary<string, object>> vehicle = frameData.Vehicle;  
  
//                 // 在这里可以根据需要处理读取到的数据  
//                 Debug.Log("Frame: " + frameKey);  
//                 Debug.Log("Time: " + time);  
//                 Debug.Log("Text: " + text);  
//                 // 处理 pedestrian 和 vehicle 数据  
//             }  
//         }  
//     }  

// }
// public class MarkManager : MonoBehaviour
// { 
//     private void SpawnPrefabs()  
//     {  
//         GameObject container = new GameObject("PrefabContainer");  
  
//         for (int i = 0; i < data.prefabCount; i++)  
//         {  
//             GameObject prefabInstance = Instantiate(Resources.Load<GameObject>("Prefab"), data.position, Quaternion.identity);  
//             prefabInstance.transform.SetParent(container.transform);  
//         }  
//     }  
// }  