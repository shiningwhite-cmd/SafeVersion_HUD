using UnityEngine;  
using System.Diagnostics;  
  
public class PythonRunning : MonoBehaviour  
{  
    private void Awake()  
    {  
        string pythonPath = "./UnityEnv310/Scripts/python.exe"; // 如果Python已经添加到系统路径中，可以直接使用"python"作为路径  
        string scriptPath = "./JsonProcess/JsonReadPy.py"; // 替换为你的Python脚本路径  
  
        ProcessStartInfo startInfo = new ProcessStartInfo(pythonPath);  
        startInfo.Arguments = scriptPath;  
  
        Process process = new Process();  
        process.StartInfo = startInfo;  
        process.EnableRaisingEvents = true; // 启用Exited事件  
        process.Exited += ProcessExited; // 注册Exited事件处理程序  
        process.Start();  
    }  
  
    private void ProcessExited(object sender, System.EventArgs e)  
    {  
        UnityEngine.Debug.Log("Python script execution completed.");  
    }
}  