using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModeManager : MonoBehaviour
{
    //此处若为真，则是鼠标代替眼动仪测试，为假则是读取眼动数据
    public static bool TestMode = false;
    public bool isTestMode;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if(isTestMode)
            TestMode = true;
        else
            TestMode = false;
    }

    public static bool returnTestMode()
    {
        return TestMode;
    }
}
