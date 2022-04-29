using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���S���A�j���[�V�������̐���N���X
/// </summary>
public class PlayerStateDeath : PlayerState
{
    public PlayerStateDeath()
    {
        PlayerScript.refState = EnumPlayerState.DEATH;
        PlayerScript.canShotState = false;
        PlayerScript.vel = Vector3.zero;
        PlayerScript.addVel = Vector3.zero;

        BulletScript.rb.velocity = Vector3.zero;
        BulletScript.vel = Vector3.zero;
        BulletScript.StopVelChange = true;

        PlayerScript.ResetAnimation();
    }

    public override void UpdateState()
    {
        // �t�F�[�h����
        FadeManager.Instance.SetNextFade(FADE_STATE.FADE_OUT, FADE_KIND.FADE_GAMOVER);
    }

    public override void Move()
    {
        //�ړ��Ȃ�
    }

    public override void StateTransition()
    {
        //��������h�����邱�Ƃ͂Ȃ�
        //�V�[���ύX���ăN�C�b�N���g���C�ʒu�Ƀ��|�b�v
    }

}