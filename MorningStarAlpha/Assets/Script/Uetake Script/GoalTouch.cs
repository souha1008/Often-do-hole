using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTouch : MonoBehaviour
{ 
    private void OnTriggerEnter(Collider other)
    {
        if (PlayerMain.instance.refState != EnumPlayerState.CLEAR)
        {
            PlayerMain.instance.mode = new PlayerState_Clear();
        }
    }
}
