using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video; 

public class VideoPlayControler : MonoBehaviour
{
    public float videoWaitforSecond;
    public VideoPlayer videoPlayer;
    // Start is called before the first frame update
    public void PlayVideo()
    {
        StartCoroutine(Play());
    }

    private IEnumerator Play() 
    {
        yield return new WaitForSeconds(videoWaitforSecond);
        videoPlayer.Play();
    }
}
