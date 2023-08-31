using UnityEngine;  

public class FPS : MonoBehaviour  
{  
    float deltaTime = 0.0f;  

    void Update()  
    {  
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;  
    }  

    void OnGUI()  
    {  
        int fps = Mathf.RoundToInt(1.0f / deltaTime);  
        GUI.Label(new Rect(10, 10, 100, 20), "FPS: " + fps);  
    }  
}  
