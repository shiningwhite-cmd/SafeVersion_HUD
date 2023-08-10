using System.Collections.Generic;  
using UnityEngine; 
// using EventSystem; 

  
public class HumanWarningReceiver : MonoBehaviour  
{  
    public GameObject humanPrefab;  
  
    private void OnEnable()  
    {  
        HumanWarningManager.HumanWarning += ReceiveMassage;  
    }  
  
    private void OnDisable()  
    {  
        HumanWarningManager.HumanWarning -= ReceiveMassage;  
    } 

    private void ReceiveMassage(Transform target)
    {
        GameObject follower = Instantiate(humanPrefab, this.gameObject.transform);
        follower.GetComponent<EyeMarkFollower>().SetTarge(target);
        target.gameObject.GetComponent<blink>().SetFollower(follower.transform.GetChild(1).gameObject);
    }
}  