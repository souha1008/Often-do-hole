using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum DeathType
{
    THORN,  //�j
    VOID,   //�ޗ�
}


/// <summary>
/// ���S���A�j���[�V�������̐���N���X
/// </summary>
/// 
public class PlayerStateDeath_Thorn : PlayerState
{

    float Timer;
    
    public PlayerStateDeath_Thorn()
    {
        PlayerScript.refState = EnumPlayerState.DEATH;
        PlayerScript.canShotState = false;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel = Vector3.zero;
        PlayerScript.useVelocity = false;
        Timer = 0.0f;

        BulletScript.ReturnBullet();
        RotationStand();
        PlayerScript.ResetAnimation();

        //�A�j���p
        PlayerScript.animator.SetFloat("NockbackSpeed", 8.0f);
        PlayerScript.animator.Play("NockBack");
        PlayerScript.animator.SetBool(PlayerScript.animHash.IsDead, true);
    }

    public override void UpdateState()
    {
        // �t�F�[�h����
        Timer += Time.deltaTime;


        if (Timer > 0.6)
        {
            GameStateManager.GameOverReloadScene();
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

public class PlayerStateDeath_Void : PlayerState
{
    float Timer;

    public PlayerStateDeath_Void()
    {
        CameraMainShimokawara.instance.StopCamera();
        PlayerScript.refState = EnumPlayerState.DEATH;
        PlayerScript.canShotState = false;
        Timer = 0.0f;

        BulletScript.ReturnBullet();
        RotationStand();

        //�A�j���p
        PlayerScript.ResetAnimation();
        //�Ȃ�
    }

    public override void UpdateState()
    {
        // �t�F�[�h����
        Timer += Time.deltaTime;


        if (Timer > 0.6)
        {
            GameStateManager.GameOverReloadScene();
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