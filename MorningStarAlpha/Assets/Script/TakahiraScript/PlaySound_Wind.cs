using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound_Wind : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.PlaySound("sound_50");
    }
}
