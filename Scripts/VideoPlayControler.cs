using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video; 

public class VideoPlayControler : MonoBehaviour
{
    public float videoWaitforSecond = 0.2f;
    public VideoPlayer videoPlayer;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Play());
    }

    private IEnumerator Play() 
    {
        yield return new WaitForSeconds(videoWaitforSecond);
        videoPlayer.Play();
    }
}
