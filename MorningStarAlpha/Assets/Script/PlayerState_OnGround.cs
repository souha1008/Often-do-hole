using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーが地上にいる状態
/// スティックで移動、弾の発射ができる
/// </summary>
public class PlayerStateOnGround : PlayerState
{
    private bool shotButton;
    private const float SLIDE_END_TIME = 0.3f;
    private float slideEndTimer;

    public PlayerStateOnGround()//コンストラクタ
    {
        PlayerScript.refState = EnumPlayerState.ON_GROUND;
        shotButton = false;
        PlayerScript.vel.y = 0;
        PlayerScript.canShotState = true;
        slideEndTimer = 0.0f;

        //ボール関連
        BulletScript.InvisibleBullet();


        //スライド発射処理
        if (Mathf.Abs(PlayerScript.vel.x) > 40.0f)
        {
            PlayerScript.onGroundState = OnGroundState.SLIDE;
        }
        else
        {
            PlayerScript.onGroundState = OnGroundState.NORMAL;
        }
    }

    public override void UpdateState()
    {
        BulletAdjust();

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



        //プレイヤー向き回転処理
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            if (PlayerScript.adjustLeftStick.x < -0.01f)
            {
                PlayerScript.dir = PlayerMoveDir.LEFT;
                PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
            }
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            if (PlayerScript.adjustLeftStick.x > 0.01f)
            {
                PlayerScript.dir = PlayerMoveDir.RIGHT;
                PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }

    public override void Move()
    {

        if (PlayerScript.onGroundState == OnGroundState.SLIDE)
        {
            float slide_Weaken = 0.5f;

            if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD) //右移動
            {
                if (PlayerScript.vel.x < -0.2f)//ターンしてるときは早い
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x), 3) * 2 * slide_Weaken * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x), 3) * slide_Weaken * 0.4f * (fixedAdjust);
                }

                //PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);
            }
            else if (PlayerScript.adjustLeftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1) //左移動
            {

                if (PlayerScript.vel.x > 0.2f)
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x), 3) * -1 * 2 * slide_Weaken * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x), 3) * -1 * slide_Weaken * 0.4f * (fixedAdjust);
                }
                //PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
            }


            //減衰
            {
                PlayerScript.vel *= 0.97f;
            }

            //スライド終了処理（時間によるもの
            slideEndTimer += Time.fixedDeltaTime;
            if (slideEndTimer > SLIDE_END_TIME)
            {
                PlayerScript.onGroundState = OnGroundState.NORMAL;
                PlayerScript.canShotState = true;
            }
        }
        else //!isSlide
        {
            if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD) //右移動
            {
                if (PlayerScript.vel.x < -0.2f)
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x), 3) * 2 * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x), 3) * (fixedAdjust);
                }

                PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);
            }
            else if (PlayerScript.adjustLeftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1) //左移動
            {

                if (PlayerScript.vel.x > 0.2f)
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x), 3) * -1 * 2 * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x), 3) * -1 * (fixedAdjust);
                }
                PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
            }
            else //減衰
            {
                PlayerScript.vel *= PlayerScript.RUN_FRICTION;
            }
        }
    }


    public override void StateTransition()
    {
        if (PlayerScript.isOnGround == false)
        {
            PlayerScript.onGroundState = OnGroundState.NONE;
            PlayerScript.mode = new PlayerStateMidair(true);
        }

        if (shotButton)
        {
            //スライド中で投げる方向が進行方向と同じなら
            if ((PlayerScript.onGroundState == OnGroundState.SLIDE) && (PlayerScript.adjustLeftStick.x * PlayerScript.vel.x > 0))
            {
                PlayerScript.onGroundState = OnGroundState.NONE;
                PlayerScript.mode = new PlayerStateShot(true);
            }
            else
            {
                PlayerScript.onGroundState = OnGroundState.NONE;
                PlayerScript.mode = new PlayerStateShot(false);
            }
        }
    }
}
