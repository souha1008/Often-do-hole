using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stump_Callback : MonoBehaviour
{
    void Stump_End()
    {
        //ResultManager.instance.stump_animator.SetBool(ResultManager.instance.Stump_Start, false);
        ResultManager.instance.stump_animator.SetBool(ResultManager.instance.Stump_end, true);
    }

    public void PlayStumpSound()
    {
        SoundManager.Instance.PlaySound("sound_45");
    }
}
