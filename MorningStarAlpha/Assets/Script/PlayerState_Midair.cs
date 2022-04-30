using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// プレイヤーが空中にいる状態
/// クールタイムを経て、弾の発射ができる
/// </summary>
public class PlayerStateMidair : PlayerState
{
    private bool shotButton;
    private float boostTimer;

    private void Init()
    {
        PlayerScript.refState = EnumPlayerState.MIDAIR;
        shotButton = false;
        boostTimer = 0.0f;

        RotationStand();
        //アニメ用
        PlayerScript.ResetAnimation();
        PlayerScript.animator.SetBool(PlayerScript.animHash.onGround, false);
    }

    public PlayerStateMidair(bool can_shot, MidairState first_state)//コンストラクタ
    {
        Init();
        PlayerScript.midairState = first_state;
        if(PlayerScript.midairState == MidairState.NORMAL)
        {
            if (PlayerScript.recoverBullet)
            {
                PlayerScript.canShotState = true;
                PlayerScript.recoverBullet = false;
            }
            else
            {
                PlayerScript.canShotState = can_shot;
            }

            PlayerScript.animator.SetBool(PlayerScript.animHash.isBoost, false);
        }
        else if (PlayerScript.midairState == MidairState.BOOST)
        {
            PlayerScript.canShotState = false;
            PlayerScript.animator.SetBool(PlayerScript.animHash.isBoost, true);
        }
        
    }


    public override void UpdateState()
    {

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
    }

    public override void Move()
    {
        //減衰S
        PlayerScript.vel.x *= PlayerScript.MIDAIR_FRICTION;
        if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * (fixedAdjust);
        }
        else if (PlayerScript.adjustLeftStick.x < -PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * -1 * (fixedAdjust);
        }

        //自由落下
        //if (PlayerScript.midairState == MidairState.NORMAL)
        //{
        PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * (fixedAdjust);
        PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
        //}

        if (PlayerScript.midairState == MidairState.BOOST)
        {
            boostTimer += Time.fixedDeltaTime;
            if(boostTimer > 0.1f)
            {
                BoostEnd();    
            }
        }
    }

    private void BoostEnd()
    {
        boostTimer = 0.0f;
        PlayerScript.midairState = MidairState.NORMAL;
        PlayerScript.canShotState = true;
    }


    public override void StateTransition()
    {
        if (shotButton)
        {
            PlayerScript.midairState = MidairState.NONE;
            PlayerScript.mode = new PlayerStateShot(false);
        }

        //着地したら立っている状態に移行
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
