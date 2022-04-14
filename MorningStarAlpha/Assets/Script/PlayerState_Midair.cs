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
    private void Init()
    {
        PlayerScript.refState = EnumPlayerState.MIDAIR;
        PlayerScript.midairState = MidairState.NORMAL;
        shotButton = false;
        PlayerScript.canShotState = false;

        BulletScript.InvisibleBullet();
    }

    public PlayerStateMidair(bool can_shot)//コンストラクタ
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
        

        //急降下入力下？
        if (PlayerScript.sourceLeftStick.y < -0.7f && Mathf.Abs(PlayerScript.sourceLeftStick.x) < 0.3f)
        {
            //一度でも入力されたら永久に
            PlayerScript.midairState = MidairState.FALL;
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

        //急降下中
        if (PlayerScript.midairState == MidairState.FALL)
        {
            //プレイヤーが上に向かっているときは早い
            if (PlayerScript.vel.y > 0.0f)
            {
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 2.0f * (fixedAdjust);
            }
            else　//下のときも少し早い
            {
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 1.5f * (fixedAdjust);
            }

            PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1 * 1.3f);
        }
        //自由落下
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
