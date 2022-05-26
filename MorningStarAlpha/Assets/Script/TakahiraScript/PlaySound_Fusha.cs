using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound_Fusha : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.PlaySound("sound_52");
    }
}
