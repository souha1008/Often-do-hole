using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�̑��x�G�t�F�N�g�𑁂��Ƃ�����
/// </summary>
public class TrailEffectOnOff : MonoBehaviour
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
        if(pa.isEmitting == false) //�p�[�e�B�N���Đ����Ă��Ȃ�
        {
            if (PlayerMain.instance.refState == EnumPlayerState.SWING)
            {
                if (PlayerMain.instance.swingState == SwingState.TOUCHED)
                {
                    pa.Play();
                }
            }
            else if(PlayerMain.instance.refState == EnumPlayerState.SHOT)
            {
                if (PlayerMain.instance.shotState == ShotState.GO)
                {
                    pa.Play();
                }
            }
            else if(PlayerMain.instance.refState == EnumPlayerState.MIDAIR)
            {

            }
            else if (PlayerMain.instance.refState == EnumPlayerState.ON_GROUND)
            {
                if(PlayerMain.instance.onGroundState == OnGroundState.SLIDE)
                {
                    pa.Play();
                }
            }
        }
        else
        {
            if (PlayerMain.instance.refState == EnumPlayerState.ON_GROUND)
            {
                if (PlayerMain.instance.onGroundState == OnGroundState.NORMAL)
                {
                    pa.Stop();
                }
            }
            else if(PlayerMain.instance.refState == EnumPlayerState.MIDAIR)
            {
                if (PlayerMain.instance.midairState == MidairState.NORMAL)
                {
                    pa.Stop();
                }
            }
        }
       
    }
}
