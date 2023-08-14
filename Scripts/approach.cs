using System.Collections;
using UnityEngine;  
using UnityEngine.UI;  
  
public class approach : MonoBehaviour  
{  
    public Image image;  
    public GameObject follow;
    public float scaleDuration = 1f;  
    public float minScaleFactor = 0.7f;  
    public float maxScaleFactor = 1f;  
    public float scaleThreshold = 110f;  
  
    private bool isScaling;  
    private bool isMouseOver;  
    private Vector3 originalScale; 
    private Animator anima;
    private Animator followAnima;
    private Vector3 EyePos;
    private Vector3 imagePosition;
  
    private void Start()  
    {  
        isScaling = false;  
        isMouseOver = false;  
        originalScale = image.transform.localScale;  
        anima = GetComponent<Animator>();
        if(follow != null)
            followAnima = follow.GetComponent<Animator>();
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
        if(EyePos == null)
            EyePos = Vector3.zero;
        imagePosition = this.gameObject.transform.position;  
        float distance = Vector3.Distance(EyePos, imagePosition);  
        // Debug.Log(imagePosition);
  
        if (distance < scaleThreshold)  
        {     
            //anima.Play("ScaleDown");  
            if(followAnima != null)
                followAnima.SetBool("isFading", true);
            anima.SetBool("isAnimaPlay", true);
        }  
        // else if (distance > scaleThreshold && !isScaling && image.transform.localScale.x < originalScale.x * maxScaleFactor)  
        // {  
        //     // 开始放大协程  
        //     StartCoroutine(ScaleUpCoroutine());  
        // }  
    }
    
    public void SetFollower(GameObject follower)
    {
        follow = follower;
        followAnima = follow.GetComponent<Animator>();
    } 

    private void ReceiveMassage(Vector2 Pos)
    {
        EyePos = new Vector3(Pos.x, Pos.y, 0);
    }
}  