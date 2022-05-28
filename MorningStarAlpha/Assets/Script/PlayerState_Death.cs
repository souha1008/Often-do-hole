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

    private static float KnockbackPowerX = 40.0f;    // �m�b�N�o�b�N�� X
    private static float KnockbackPowerY = 20.0f;    // �m�b�N�o�b�N�� Y
    private Vector3 HitPos;

    public PlayerStateDeath_Thorn(Vector3 hit_pos)
    {
        PlayerScript.refState = EnumPlayerState.DEATH;
        PlayerScript.canShotState = false;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel = Vector3.zero;
        PlayerScript.floorVel = Vector3.zero;
        PlayerScript.addVel = Vector3.zero;
        Timer = 0.0f;
        HitPos = hit_pos;

        BulletScript.ReturnBullet();
        RotationStand();
        PlayerScript.ResetAnimation();

        //�A�j���p
        PlayerScript.animator.SetFloat("NockbackSpeed", 2.0f);
        PlayerScript.animator.Play("NockBack");
        PlayerScript.animator.SetBool(PlayerScript.animHash.IsDead, true);

        deathSE();

        Knockback();
    }

    void deathSE()
    {
        int seNum = Random.Range(0, 2);

        switch (seNum)
        {
            case 0:
                //SE
                SoundManager.Instance.PlaySound("CVoice_ (3)", 0.8f);
                break;

            case 1:
                SoundManager.Instance.PlaySound("CVoice_ (4)", 0.8f);
                break;

            default:
                Debug.LogWarning("random :Out OfRange");
                break;
        }

    }


    // �m�b�N�o�b�N����
    private void Knockback()
    {
        // �m�b�N�o�b�N�����w��
        Vector3 Vec = Player.transform.position - HitPos;

        // �L�����̉�]
        if (Vec.x < 0)
        {
            PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 90, 0));
        }
        else
        {
            PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 270, 0));
        }

        // �m�b�N�o�b�N�ړ�
        if (Vec.x < 0)
        {
            PlayerScript.vel = new Vector3(-KnockbackPowerX, KnockbackPowerY, 0);
        }
        else
        {
            PlayerScript.vel = new Vector3(KnockbackPowerX, KnockbackPowerY, 0);
        }
    }

    public override void UpdateState()
    {
        // �t�F�[�h����
        Timer += Time.deltaTime;


        if (Timer > 0.8)
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


/// <summary>
/// ���S���A�j���[�V�������̐���N���X
/// </summary>
/// 
public class PlayerStateDeath_Kujira : PlayerState
{

    float Timer;

    public PlayerStateDeath_Kujira()
    {
        PlayerScript.refState = EnumPlayerState.DEATH;
        PlayerScript.canShotState = false;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel = Vector3.zero;
        PlayerScript.floorVel = Vector3.zero;
        PlayerScript.addVel = Vector3.zero;
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
        if (PlayerScript.refState != EnumPlayerState.DEATH)
        {
            deathVoidSE();
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
    }

    void deathVoidSE()
    {
        int seNum = Random.Range(0, 2);

        switch (seNum)
        {
            case 0:
                //SE
                SoundManager.Instance.PlaySound("CVoice_ (1)", 0.8f);
                break;

            case 1:
                SoundManager.Instance.PlaySound("CVoice_ (2)", 0.8f);
                break;

            default:
                Debug.LogWarning("random :Out OfRange");
                break;
        }

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