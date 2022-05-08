using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stump_Callback : MonoBehaviour
{
    void Stump_End()
    {
        ResultManager.instance.Stump_end = true;
    }
}
