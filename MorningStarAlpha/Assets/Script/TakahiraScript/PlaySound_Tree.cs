using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound_Tree : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.PlaySound("sound_51", 0.5f, this.gameObject);
        MeshRenderer Mr = null;
        if ((Mr = this.GetComponent<MeshRenderer>()) != null)
        {
            Mr.enabled = false;
        }
    }
}
