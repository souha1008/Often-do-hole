using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound_Wind : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.PlaySound("sound_50", 0.5f, this.gameObject);
        MeshRenderer Mr = null;
        if ((Mr = this.GetComponent<MeshRenderer>()) != null)
        {
            Mr.enabled = false;
        }
    }
}
