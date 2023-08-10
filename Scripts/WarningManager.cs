using UnityEngine;  
  
public class WarningManager : MonoBehaviour  
{  
    private IWarningManager humanWarningManager;  
    private IWarningManager carWarningManager;

    private void Start()
    {
        humanWarningManager = new HumanWarningManager();  
        carWarningManager = new CarWarningManager();
        
    }

    private void Update()  
    {  
        humanWarningManager.Update();  
        carWarningManager.Update();  
    }  
}  
  
public interface IWarningManager  
{  
    void Update();  
}  
  
public class HumanWarningManager : IWarningManager  
{  
    public delegate void WarningCreate(Transform target);  
    public static event WarningCreate HumanWarning;  
  
    public void Update()  
    {  
        // 处理人类警告逻辑  
    }  
  
    public static void SendMessage(Transform target)  
    {  
        if (HumanWarning != null)  
        {  
            HumanWarning(target);  
            Debug.Log(target);  
        }  
    }  
}  
  
public class CarWarningManager : IWarningManager  
{  
    public delegate void WarningCreate(Transform target);  
    public static event WarningCreate CarWarning;  
  
    public void Update()  
    {  
        // 处理汽车警告逻辑  
    }  
  
    public static void SendMessage(Transform target)  
    {  
        if (CarWarning != null)  
        {  
            CarWarning(target);  
            Debug.Log(target);  
        }  
    }  
}  