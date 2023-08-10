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
  
    private void Start()  
    {  
        // checkBox = this.gameobject.FindGameo
        isMouseOver = false;   
        checkBoxAnima = checkBox.GetComponent<Animator>();
        underLineAnima = underLine.GetComponent<Animator>();
        if(follow != null)
            followAnima = follow.GetComponent<Animator>();
    }  
  
    private void Update()  
    {  
        Vector3 mousePosition = Input.mousePosition;  
        Vector3 imagePosition = Camera.main.WorldToScreenPoint(checkBox.transform.position);  
        float distance = Vector3.Distance(mousePosition, imagePosition);  
  
        if (distance < fadeThreshold)  
        {     
            //anima.Play("ScaleDown");  
            if(followAnima != null)
                followAnima.SetBool("isFading", true);
            checkBoxAnima.SetBool("isFading", true);
            StartCoroutine(PauseUpdateCoroutine(fadeDuration));
            underLineAnima.SetBool("isAppearing", true);

        }  
        // else if (distance > scaleThreshold && !isScaling && image.transform.localScale.x < originalScale.x * maxScaleFactor)  
        // {  
        //     // 开始放大协程  
        //     StartCoroutine(ScaleUpCoroutine());  
        // }  
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
}  