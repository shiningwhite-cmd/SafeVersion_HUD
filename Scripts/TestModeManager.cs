using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModeManager : MonoBehaviour
{
    public static bool TestMode = true;

    public static bool returnTestMode()
    {
        return TestMode;
    }
}
