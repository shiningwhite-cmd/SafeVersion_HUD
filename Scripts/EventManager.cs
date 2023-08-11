using UnityEngine;  
  
public class EventManager : MonoBehaviour  
{  
    private IEventManager  eyeTrackManager;  
    private IEventManager  jsonMarkManager;   
    private IEventManager  deleteMarkManager; 

    private void Start()
    {
        eyeTrackManager = new EyeTrackManager();
        jsonMarkManager = new JsonMarkManager();
        deleteMarkManager = new DeleteMarkManager();
        
    }

    private void Update()  
    {  
        eyeTrackManager.Update(); 
        jsonMarkManager.Update();  
        deleteMarkManager.Update();
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

public class JsonMarkManager : IEventManager  
{  
    public delegate void JsonMarkCreate(MarkMessage mMessage);  
    public static event JsonMarkCreate JsonMark;  
  
    public void Update()  
    {  
    }  
  
    public static void SendMessage(MarkMessage mMessage)  
    {  
        if (JsonMark != null)  
        {  
            JsonMark(mMessage);  
            //Debug.Log(target);  
        }  
    }  
}  


public class DeleteMarkManager : IEventManager  
{  
    public delegate void DeleteMarkCreate(int id);  
    public static event DeleteMarkCreate DeleteMark;  
  
    public void Update()  
    {  
    }  
  
    public static void SendMessage(int id)  
    {  
        if (DeleteMark != null)  
        {  
            DeleteMark(id);  
            //Debug.Log(target);  
        }  
    }  
}  

[System.Serializable]  
public class MarkMessage
{  
    public int MarkID;
    public Vector2 BboxBL;
    public Vector2 BboxTR;

    public MarkMessage(int markID, Vector2 bboxBL, Vector2 bboxTR)
    {
        MarkID = markID;
        BboxBL = bboxBL;
        BboxTR = bboxTR;
    }
}
