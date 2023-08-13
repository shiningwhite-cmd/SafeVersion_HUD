using UnityEngine;  
using UnityEngine.UI;  
using System.Collections;
  
public class WarningPosChanger : MonoBehaviour  
{  
    public Vector3 targetPosition;  
    public float duration = 0.2f;  
  
    private Vector3 startPosition;  
    private float elapsedTime = 0f;  

    private bool isCoroutineStarted = false;
  
    private void Start()  
    {   
    }  
  
    private IEnumerator MoveToTarget()  
    {  
        while (elapsedTime < duration)  
        {  
            elapsedTime += Time.deltaTime;  
            float t = Mathf.Clamp01(elapsedTime / duration);  
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);  
            yield return null;  
        }  
    }  


    public void MoveWarning(Vector3 target)
    {
        startPosition = transform.position;
        targetPosition = target;  
        StartCoroutineIfNeeded();
    }

    private void StartCoroutineIfNeeded()  
    {  
        if (!isCoroutineStarted)  
        {  
            StartCoroutine(MoveToTarget());  
            isCoroutineStarted = true;  
        }  
    } 
}  