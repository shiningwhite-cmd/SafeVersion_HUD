using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningPositionController : MonoBehaviour
{
    public GameObject CarMarkPrefab;
    public GameObject HumanMarkPrefab;
    public Transform MarkPanel;
    // 存储现有的mark id
    private List<int> ExistingCarMark = new List<int>();
    private List<int> ExistingHumanMark = new List<int>();
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
        bool isperson = mMessage.isPerson;
        int markID = mMessage.MarkID;
        Vector2 markPosV2 = (mMessage.BboxBL + mMessage.BboxTR)/2;
        Vector3 markPos = new Vector3(markPosV2.x, markPosV2.y, 0);
        Vector2 diagonal = ( mMessage.BboxBL - mMessage.BboxTR);
        diagonal.x = -diagonal.x;
        if(isperson)
        {
            //  如果存在mark，刷新位置
            if(ExistingHumanMark.Contains(markID))
            {
                GameObject mark = GameObject.Find("HumanWarning"+markID.ToString());
                // mark.transform.position = markPos;
                
                mark.GetComponent<WarningPosChanger>().MoveWarning(markPos);
                mark.GetComponent<RectTransform>().sizeDelta = diagonal;
            }
            // 如果不存在，复制一个
            else
            {
                Debug.Log("Person");
                ExistingHumanMark.Add(markID);
                GameObject mark = Instantiate(HumanMarkPrefab,  markPos, new Quaternion(), MarkPanel);
                mark.name = "HumanWarning"+markID.ToString();
                mark.GetComponent<HumanWarningSender>().Send(markID);
                mark.GetComponent<RectTransform>().sizeDelta = diagonal;
            }
        }
        else
        {
            
            //  如果存在mark，刷新位置
            if(ExistingCarMark.Contains(markID))
            {
                GameObject mark = GameObject.Find("CarWarning"+markID.ToString());
                // mark.transform.position = markPos;
                
                mark.GetComponent<WarningPosChanger>().MoveWarning(markPos);
            }
            // 如果不存在，复制一个
            else
            {
                ExistingCarMark.Add(markID);
                GameObject mark = Instantiate(CarMarkPrefab,  markPos, new Quaternion(), MarkPanel);
                mark.name = "CarWarning"+markID.ToString();
                mark.GetComponent<CarWarningSender>().Send(markID);
            }
        }
    }

    private void DeleteMark(bool isPerson, int id)
    {
        if(isPerson)
        {
            GameObject mark = GameObject.Find("HumanWarning"+id.ToString()); // 获取GameObject的引用  
            GameObject follower = GameObject.Find("HumanFollower"+id.ToString()); // 获取GameObject的引用  

            ExistingHumanMark.Remove(id);
            
            Destroy(mark);
            Destroy(follower);

        }
        else
        {
            GameObject mark = GameObject.Find("CarWarning"+id.ToString()); // 获取GameObject的引用  
            GameObject follower = GameObject.Find("CarFollower"+id.ToString()); // 获取GameObject的引用  

            ExistingCarMark.Remove(id);
            
            Destroy(mark);
            Destroy(follower);
        }
    }

}
