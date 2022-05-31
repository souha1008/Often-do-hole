using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound_Wind : MonoBehaviour
{
    void Start()
    {
        if (!SoundManager.Instance.isNowPlaySound("sound_50"))
            SoundManager.Instance.PlaySound("sound_50", 0.3f);
        MeshRenderer Mr = null;
        if ((Mr = this.GetComponent<MeshRenderer>()) != null)
        {
            Mr.enabled = false;
        }
    }
}
