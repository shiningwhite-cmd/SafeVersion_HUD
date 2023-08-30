using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPlay : MonoBehaviour
{
    void Update()  
    {  
        // 检测玩家是否按下了"Escape"键  
        if (Input.GetKeyDown(KeyCode.Escape))  
        {  
            // 退出游戏  
            Quit();  
        }  
    }  
  
    void Quit()  
    {  
        // 在这里调用退出游戏的方法  
        #if UNITY_EDITOR  
            UnityEditor.EditorApplication.isPlaying = false;  
        #else  
            Application.Quit();  
        #endif  
    }  
}
