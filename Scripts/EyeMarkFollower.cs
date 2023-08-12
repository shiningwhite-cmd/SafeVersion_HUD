using UnityEngine;  
using System.Collections;
  
public class EyeMarkFollower : MonoBehaviour  
{  
    public Transform target; 
    public Transform follower;
    public Transform mark; 
    public float speed = 0.1f;
    public float rotationSpeed = 2.5f; 
    private float Radius = 94.0f;
    private Vector3 EyePos;
    private Vector3 eyePos;
    private bool TestMode = false;
  
    void Start()
    {
        TestMode = TestModeManager.returnTestMode();
        // StartCoroutine(FollowTarget()); 
    }

    private void OnEnable()  
    {  
        EyeTrackManager.EyeTrack += ReceiveMassage;  
    }  
  
    private void OnDisable()  
    {  
        EyeTrackManager.EyeTrack -= ReceiveMassage;  
    } 

    private void Update()  
    {  
        // 获取鼠标在世界坐标系中的位置  
        //Vector3 eyePos = RectTransformUtility.WorldToScreenPoint(Camera.main, GetEyeTrackPosition());
        // Vector3 eyePos = GetEyeTrackPosition();
        eyePos = GetEyeTrackPosition();
        if(TestMode)
            eyePos = GetMousePosition();
        //Debug.Log(eyePos); 
        eyePos.z = 0f;  
        // 将UI图片的位置设置为鼠标位置  
        this.gameObject.transform.position = eyePos;
        // Debug.Log("eyePos=");
        // Debug.Log(eyePos);
        // Debug.Log("position=");
        // Debug.Log(transform.position);
        //Debug.Log(transform.position);  
  
        // 计算物体A与鼠标之间的方向  
        Vector3 direction = target.position - transform.position; 
        Vector3 normalizedDirection = new Vector3(direction.x, direction.y, 0).normalized; 
  
        // 计算旋转角度  
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;  
  
        // 使用插值方法逐渐旋转UI图片  
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);  
        follower.rotation = Quaternion.Slerp(follower.rotation, targetRotation, rotationSpeed * Time.deltaTime); 

        //Vector3 markPos = new Vector3(normalizedDirection.x * Radius, normalizedDirection.y * Radius, 0);
        //mark.position = Vector3.Lerp(mark.position, markPos, rotationSpeed * Time.deltaTime);
        //Debug.Log(new Vector3(normalizedDirection.x * Radius, normalizedDirection.y * Radius, 0));
    }  

    public void SetTarge(Transform newtarget)
    {
        target = newtarget;
    }

    private Vector3 GetEyeTrackPosition()
    {
        if(EyePos != null)
        {
            return EyePos;
        }
        
        return new Vector3(0, 0, 0);
        
    }

    private Vector3 GetMousePosition()
    {
        return Input.mousePosition; 
    }

    private void ReceiveMassage(Vector2 Pos)
    {
        EyePos = new Vector3(Pos.x, Pos.y, 0);
    }

    // private IEnumerator FollowTarget()  
    // {  
    //     while (true)  
    //     {  
    //         Vector3 nowPos = Input.mousePosition;
    //         Debug.Log(this.gameObject.transform.position);
    //         float journeyLength = Vector3.Distance(this.gameObject.transform.position, nowPos);  
    //         float startTime = Time.time;  
    //         float distCovered = 0f;  
  
    //         while (distCovered < journeyLength)  
    //         {  
    //             distCovered = (Time.time - startTime) * speed;  
    //             float fracJourney = distCovered / journeyLength;  
    //             this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, nowPos, fracJourney);  
    //             yield return null;  
    //         }  
  
    //         yield return null;  
    //     }  
    // }  
}  