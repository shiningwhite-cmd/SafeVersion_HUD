using UnityEngine;  
  
public class EventManager : MonoBehaviour  
{  
    private IEventManager  eyeTrackManager;  

    private void Start()
    {
        eyeTrackManager = new EyeTrackManager();
        
    }

    private void Update()  
    {  
        eyeTrackManager.Update();  
    }  
}  
  
public interface IEventManager  
{  
    void Update();  
}  
  
public class EyeTrackManager : IEventManager  
{  
    public delegate void EyeTrackCreate(Vector2 Pos);  
    public static event EyeTrackCreate EyeTrack;  
  
    public void Update()  
    {  
    }  
  
    public static void SendMessage(Vector2 target)  
    {  
        if (EyeTrack != null)  
        {  
            EyeTrack(target);  
            //Debug.Log(target);  
        }  
    }  
}  