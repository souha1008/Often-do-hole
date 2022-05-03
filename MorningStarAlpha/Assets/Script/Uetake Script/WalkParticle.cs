using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkParticle : MonoBehaviour
{
    private ParticleSystem pa;
    private bool isPlaying;
    private Transform[] childlen;

    // Start is called before the first frame update
    void Start()
    {
        pa = GetComponent<ParticleSystem>();
        isPlaying = false;
        pa.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        childlen = new Transform[transform.childCount];

        for (int i = 0; i < childlen.Length; i++)
        {
            childlen[i] = this.transform.GetChild(i);
            Debug.LogWarning(childlen[i].name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying) //パーティクル再生していない
        {
            if (PlayerMain.instance.refState == EnumPlayerState.ON_GROUND && Mathf.Abs(PlayerMain.instance.vel.x) > 20.0f)
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
            else
            {
                if(Mathf.Abs(PlayerMain.instance.vel.x) < 20.0f)
                {
                    Switch();
                }
            }
        }
    }


    //private void Rotate()
    //{
    //    Vector3 scale = Vector3.one;
    //    if (PlayerMain.instance.dir == PlayerMoveDir.RIGHT)
    //    {
    //        for (int i = 0; i < childlen.Length; i++)
    //        {
    //            scale.x = -1;

    //            childlen[i].localScale = scale;
    //        }                                                        
    //    }                                                            
    //    else                                                         
    //    {                                                            
    //        for (int i = 0; i < childlen.Length; i++)                
    //        {
               
    //            scale.x = 1;

    //            childlen[i].localScale = scale;
    //        }
    //    }
    //}

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
