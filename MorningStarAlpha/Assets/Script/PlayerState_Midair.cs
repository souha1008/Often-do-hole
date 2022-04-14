using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �v���C���[���󒆂ɂ�����
/// �N�[���^�C�����o�āA�e�̔��˂��ł���
/// </summary>
public class PlayerStateMidair : PlayerState
{
    private bool shotButton;
    private void Init()
    {
        PlayerScript.refState = EnumPlayerState.MIDAIR;
        PlayerScript.midairState = MidairState.NORMAL;
        shotButton = false;
        PlayerScript.canShotState = false;

        BulletScript.InvisibleBullet();
    }

    public PlayerStateMidair(bool can_shot)//�R���X�g���N�^
    {
        Init();
        PlayerScript.canShotState = can_shot;
    }


    public override void UpdateState()
    {
        BulletAdjust();

        if (PlayerScript.adjustLeftStick.x > 0.01f)
        {
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (PlayerScript.adjustLeftStick.x < -0.01f)
        {
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
        }

        if (PlayerScript.ReleaseMode)
        {
            if (Input.GetButtonUp("Button_R"))
            {
                if (PlayerScript.canShot)
                {
                    shotButton = true;
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("Button_R"))
            {
                if (PlayerScript.canShot)
                {
                    shotButton = true;
                }
            }
        }
        

        //�}�~�����͉��H
        if (PlayerScript.sourceLeftStick.y < -0.7f && Mathf.Abs(PlayerScript.sourceLeftStick.x) < 0.3f)
        {
            //��x�ł����͂��ꂽ��i�v��
            PlayerScript.midairState = MidairState.FALL;
        }

    }

    public override void Move()
    {
        //����S
        PlayerScript.vel.x *= PlayerScript.MIDAIR_FRICTION;
        if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * (fixedAdjust);
        }
        else if (PlayerScript.adjustLeftStick.x < -PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * -1 * (fixedAdjust);
        }

        //�}�~����
        if (PlayerScript.midairState == MidairState.FALL)
        {
            //�v���C���[����Ɍ������Ă���Ƃ��͑���
            if (PlayerScript.vel.y > 0.0f)
            {
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 2.0f * (fixedAdjust);
            }
            else�@//���̂Ƃ�����������
            {
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 1.5f * (fixedAdjust);
            }

            PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1 * 1.3f);
        }
        //���R����
        else if (PlayerScript.midairState == MidairState.NORMAL)
        {
            PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * (fixedAdjust);
            PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
        }
    }


    public override void StateTransition()
    {
        if (shotButton)
        {
            PlayerScript.midairState = MidairState.NONE;
            PlayerScript.mode = new PlayerStateShot();
        }

        //���n�����痧���Ă����ԂɈڍs
        if (PlayerScript.isOnGround)
        {
            PlayerScript.midairState = MidairState.NONE;
            PlayerScript.mode = new PlayerStateOnGround();
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:MidAir");
    }
}
