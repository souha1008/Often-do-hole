using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkParticle : MonoBehaviour
{
    private ParticleSystem pa;

    // Start is called before the first frame update
    void Start()
    {
        pa = GetComponent<ParticleSystem>();
    }


    // Update is called once per frame
    void Update()
    {

        if (pa.isEmitting == false) //パーティクル再生していない
        {
            if (PlayerMain.instance.refState == EnumPlayerState.ON_GROUND)
            {
                pa.Play();
            }
        }
        else
        {
            if (PlayerMain.instance.refState != EnumPlayerState.ON_GROUND)
            {
                pa.Stop();
            }
        }

    }
}
