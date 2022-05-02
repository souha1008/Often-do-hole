using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���S���A�j���[�V�������̐���N���X
/// </summary>
public class PlayerStateDeath : PlayerState
{

    float Timer;

            
    public PlayerStateDeath()
    {
        PlayerScript.refState = EnumPlayerState.DEATH;
        PlayerScript.canShotState = false;



        Timer = 0.0f;
    }

    public override void UpdateState()
    {
        // �t�F�[�h����
        Timer += Time.deltaTime;


        if (Timer > 0.6)
        {
            FadeManager.Instance.SetNextFade(FADE_STATE.FADE_OUT, FADE_KIND.FADE_GAMOVER);
        }
    }

    public override void Move()
    {
        PlayerScript.vel.x *= PlayerScript.MIDAIR_FRICTION;
        PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * (fixedAdjust);
        PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);

    }

    public override void StateTransition()
    {
        //��������h�����邱�Ƃ͂Ȃ�
        //�V�[���ύX���ăN�C�b�N���g���C�ʒu�Ƀ��|�b�v
    }

}