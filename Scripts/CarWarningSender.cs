using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using EventSystem;

public class CarWarningSender : MonoBehaviour
{
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    public void Send(int id)
    {
        CarWarningManager.SendMessage(this.gameObject.transform, id);
    }
}
