using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanted_ShakeCallback : MonoBehaviour
{
    void ShakeEnd()
    {
        //ResultManager.instance.Wanted_animator.SetBool(ResultManager.instance.Shake_Start, false);
        ResultManager.instance.Wanted_animator.SetBool(ResultManager.instance.Shake_End, true);
        bool end = ResultManager.instance.Wanted_animator.GetBool(ResultManager.instance.Shake_End);
        //Debug.Log("Shake_End:" + end);
    }
}
