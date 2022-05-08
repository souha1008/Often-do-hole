using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanted_ShakeCallback : MonoBehaviour
{
    void ShakeEnd()
    {
        ResultManager.instance.Wanted_animator.SetBool(ResultManager.instance.Shake, false);
    }
}
