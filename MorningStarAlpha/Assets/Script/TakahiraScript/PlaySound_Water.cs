using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound_Water : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.PlaySound("sound_49", 0.5f, this.gameObject);
    }
}
