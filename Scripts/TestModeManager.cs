using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModeManager : MonoBehaviour
{
    //此处若为真，则是鼠标代替眼动仪测试，为假则是读取眼动数据
    public static bool TestMode = true;

    public static bool returnTestMode()
    {
        return TestMode;
    }
}
