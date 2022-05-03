using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkParticle : MonoBehaviour
{
    private ParticleSystem pa;
    private bool isPlaying;
    // Start is called before the first frame update
    void Start()
    {
        pa = GetComponent<ParticleSystem>();
        isPlaying = false;
        pa.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }


    // Update is called once per frame
    void Update()
    {
        Rotate();
        if (isPlaying) //パーティクル再生していない
        {
            if (PlayerMain.instance.refState == EnumPlayerState.ON_GROUND)
            {
                Switch();
            }
        }
        else
        {
            if (PlayerMain.instance.refState != EnumPlayerState.ON_GROUND)
            {
                Switch();
            }
        }
    }


    private void Rotate()
    {
        if(PlayerMain.instance.dir == PlayerMoveDir.RIGHT)
        {
            transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {

            transform.localRotation = Quaternion.Euler(0, -90, 0);
        }
    }

    private void Switch()
    {
        if (isPlaying)
        {
            pa.Play(true);
        }
        else
        {
            pa.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        isPlaying = !isPlaying;
    }
}
