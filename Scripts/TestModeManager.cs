using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModeManager : MonoBehaviour
{
    public static bool TestMode = false;

    public static bool returnTestMode()
    {
        return TestMode;
    }
}
