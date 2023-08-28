using UnityEngine;
using System.Collections;    
using UnityEngine.UI;  
  
public class blink : MonoBehaviour  
{    
    public GameObject checkBox; 
    public GameObject underLine;
    public GameObject follow;
    public float fadeThreshold = 160f;  
    public float fadeDuration = 1.0f;
  
    private bool isMouseOver;  
    private bool isFading;  
    
    private Vector3 originalScale; 
    private Animator checkBoxAnima;
    private Animator underLineAnima;
    private Animator followAnima;
    private Vector3 EyePos;
    private Vector3 imagePosition;
  
    private void Start()  
    {  
        isMouseOver = false;   
        checkBoxAnima = checkBox.GetComponent<Animator>();
        underLineAnima = underLine.GetComponent<Animator>();
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
        // if(EyePos == null)
        //     EyePos = Vector3.zero;
        Vector3 eyeTrackPos = Vector3.zero;
        if(TestModeManager.TestMode)
            eyeTrackPos = Input.mousePosition;
        else
        {
            if(EyePos != null)
                eyeTrackPos = EyePos;
        }
        imagePosition = this.gameObject.transform.position;  
        float distance = Vector3.Distance(eyeTrackPos, imagePosition);  
  
        if (distance < fadeThreshold)  
        {     
            //anima.Play("ScaleDown");  
            if(followAnima != null)
                followAnima.SetBool("isFading", true);
            checkBoxAnima.SetBool("isFading", true);
            StartCoroutine(PauseUpdateCoroutine(fadeDuration));
            underLineAnima.SetBool("isAppearing", true);

        }  
    } 

    private IEnumerator PauseUpdateCoroutine(float fadeDuration)  
    {  
        yield return new WaitForSeconds(fadeDuration);  
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