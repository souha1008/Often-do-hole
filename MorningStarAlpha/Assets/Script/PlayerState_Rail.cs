using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ���[���ړ����̃N���X
/// </summary>
public class PlayerStateRail : PlayerState
{
    public PlayerStateRail()
    {
        PlayerScript.refState = EnumPlayerState.RAILING;
        PlayerScript.canShotState = false; //���ĂȂ�
        PlayerScript.vel = Vector3.zero;   //���x0
        PlayerScript.addVel = Vector3.zero;

        BulletScript.rb.velocity = Vector3.zero;
        BulletScript.vel = Vector3.zero;
        BulletScript.StopVelChange = true;
    }

    public override void UpdateState()
    {
        //�L�[���͕s��
    }

    public override void Move()
    {
        //�ړ��Ȃ�
    }

    public override void StateTransition()
    {
        //�I�������X�e�[�g
    }
}


