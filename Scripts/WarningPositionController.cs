using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningPositionController : MonoBehaviour
{
    public GameObject CarMarkPrefab;
    public Transform MarkPanel;
    private List<int> ExistingMark = new List<int>();
    private bool TestMode = false;
  
    void Start()
    {
        TestMode = TestModeManager.returnTestMode();
    }

    private void OnEnable()  
    {  
        JsonMarkManager.JsonMark += ReceiveMassage;  
        DeleteMarkManager.DeleteMark += DeleteMark;
    }  
  
    private void OnDisable()  
    {  
        JsonMarkManager.JsonMark -= ReceiveMassage; 
        DeleteMarkManager.DeleteMark -= DeleteMark; 
    } 

    
    private void ReceiveMassage(MarkMessage mMessage)
    {
        int markID = mMessage.MarkID;
        Vector2 markPosV2 = (mMessage.BboxBL + mMessage.BboxTR)/2;
        Vector3 markPos = new Vector3(markPosV2.x, markPosV2.y, 0);
        if(markPos != Vector3.zero)
        {
            if(ExistingMark.Contains(markID))
            {
                GameObject mark = GameObject.Find("CarWarning"+markID.ToString());
                mark.transform.position = markPos;
            }
            else
            {
                ExistingMark.Add(markID);
                GameObject mark = Instantiate(CarMarkPrefab,  markPos, new Quaternion(), MarkPanel);
                mark.name = "CarWarning"+markID.ToString();
            }
        }
    }

    private void DeleteMark(int id)
    {
        GameObject mark = GameObject.Find("CarWarning"+id.ToString()); // 获取GameObject的引用  
  
        
        Destroy(mark);
    }

}
