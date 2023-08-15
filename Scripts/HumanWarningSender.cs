using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using EventSystem;

public class HumanWarningSender : MonoBehaviour
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    public void Send(int id)
    {
        HumanWarningManager.SendMessage(this.gameObject.transform, id);
    }
}
