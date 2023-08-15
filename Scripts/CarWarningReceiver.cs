using System.Collections.Generic;  
using UnityEngine; 
// using EventSystem; 

  
public class CarWarningReceiver : MonoBehaviour  
{  
    public GameObject CarPrefab;  
  
    private void OnEnable()  
    {  
        CarWarningManager.CarWarning += ReceiveMassage;  
    }  
  
    private void OnDisable()  
    {  
        CarWarningManager.CarWarning -= ReceiveMassage;  
    } 

    private void ReceiveMassage(Transform target, int id)
    {
        GameObject follower = Instantiate(CarPrefab, this.gameObject.transform);
        follower.name = "CarFollower"+id.ToString();
        follower.GetComponent<EyeMarkFollower>().SetTarge(target);
        target.gameObject.GetComponent<approach>().SetFollower(follower.transform.GetChild(1).gameObject);
    }
}  