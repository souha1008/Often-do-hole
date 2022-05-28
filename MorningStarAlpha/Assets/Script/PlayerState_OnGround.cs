using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーが地上にいる状態
/// スティックで移動、弾の発射ができる
/// </summary>
public class PlayerStateOnGround : PlayerState
{
    private const float AddDownForce = -5.0f;

    private bool shotButton;
    private const float SLIDE_END_TIME = 0.5f;
    private float slideEndTimer;
    private float rareMotionTimer;
    private PlayerMoveDir oldMoveDir;

    public PlayerStateOnGround()//コンストラクタ
    {
        PlayerScript.refState = EnumPlayerState.ON_GROUND;
        shotButton = false;
        PlayerScript.vel.y = AddDownForce;
        PlayerScript.canShotState = true;
        slideEndTimer = 0.0f;
        rareMotionTimer = 0.0f;
        oldMoveDir = PlayerScript.dir;
        BulletScript.ReturnBullet();

        RotationStand();
        NoFloorVel();


        //スライド処理
        if (Mathf.Abs(PlayerScript.vel.x) > 40.0f)
        {
            PlayerScript.animator.SetBool(PlayerScript.animHash.isRunning, true);
            PlayerScript.onGroundState = OnGroundState.SLIDE;
        }
        else
        {
            PlayerScript.onGroundState = OnGroundState.NORMAL;
        }

        //アニメ用
        PlayerScript.ResetAnimation();
        PlayerScript.animator.SetBool(PlayerScript.animHash.onGround, true);
    }



    public override void UpdateState()
    {
        if (Input.GetButtonDown("Button_R"))
        {
            if (PlayerScript.canShot)
            {
                shotButton = true;
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


        if(oldMoveDir != PlayerScript.dir)
        {
            oldMoveDir = PlayerScript.dir;
        }
    }

    public override void Move()
    {
#if false
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
                PlayerScript.animator.SetBool(PlayerScript.animHash.isRunning, true);

                if (PlayerScript.vel.x < 5.0f)
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED  * 2 * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED  * (fixedAdjust);
                }

                //限度
                PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);
            }
            else if (PlayerScript.adjustLeftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1) //左移動
            { 
                PlayerScript.animator.SetBool(PlayerScript.animHash.isRunning, true);

                if (PlayerScript.vel.x > -5.0f)
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * -1 * 2 * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * -1 * (fixedAdjust);
                }

                //限度
                PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
            }
            else //減衰
            {
                PlayerScript.animator.SetBool(PlayerScript.animHash.isRunning, false);
               
                PlayerScript.vel *= PlayerScript.RUN_FRICTION;

                //ある程度下回ったら0にする処理
            }
        }
#else
        if(PlayerScript.onGroundState == OnGroundState.SLIDE)
        {

            //スライド終了処理（時間によるもの
            slideEndTimer += Time.fixedDeltaTime;
            if (slideEndTimer > SLIDE_END_TIME)
            {
                PlayerScript.onGroundState = OnGroundState.NORMAL;
                PlayerScript.canShotState = true;
            }
        }



        if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD) //右移動
        {
            PlayerScript.animator.SetBool(PlayerScript.animHash.isRunning, true);

            if (PlayerScript.vel.x < 5.0f)
            {
                PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * 2 * (fixedAdjust);
            }
            else
            {
                PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * (fixedAdjust);
            }

            //限度
            if (PlayerScript.onGroundState == OnGroundState.SLIDE)
            {
                if (PlayerScript.vel.x > PlayerScript.MAX_RUN_SPEED)
                {
                    PlayerScript.vel.x *= 0.94f;
                }
            }
            else 
            {
                PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);
            }
        }
        else if (PlayerScript.adjustLeftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1) //左移動
        {
            PlayerScript.animator.SetBool(PlayerScript.animHash.isRunning, true);

            if (PlayerScript.vel.x > -5.0f)
            {
                PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * -1 * 2 * (fixedAdjust);
            }
            else
            {
                PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * -1 * (fixedAdjust);
            }

            //限度
            if (PlayerScript.onGroundState == OnGroundState.SLIDE)
            {
               if(PlayerScript.vel.x < PlayerScript.MAX_RUN_SPEED * -1)
                {
                    PlayerScript.vel.x *= 0.94f;
                }
            }
            else
            {
                PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
            }
        }
        else //減衰
        {
            PlayerScript.animator.SetBool(PlayerScript.animHash.isRunning, false);

            PlayerScript.vel *= PlayerScript.RUN_FRICTION;

            //ある程度下回ったら0にする処理
        }



#endif
        //バグ防止用にほんの少しだけ弱い重力を与える
        PlayerScript.vel.y = AddDownForce;
    }


    public override void StateTransition()
    {
        if (PlayerScript.isOnGround == false)
        {
            PlayerScript.animator.SetBool(PlayerScript.animHash.isRunning, false);

            PlayerScript.onGroundState = OnGroundState.NONE;
            PlayerScript.mode = new PlayerStateMidair(true, MidairState.NORMAL);
        }

        if (shotButton)
        {
            PlayerScript.animator.SetBool(PlayerScript.animHash.isRunning, false);

            PlayerScript.onGroundState = OnGroundState.NONE;      
            PlayerScript.mode = new PlayerStateShot(false);
        }
    }

    public override void Animation()
    {
        //速度を参照
        float animBlend = Mathf.Abs(PlayerScript.vel.x);
        animBlend = Mathf.Clamp(animBlend, 0.0f, PlayerScript.MAX_RUN_SPEED);
        PlayerScript.animator.SetFloat(Animator.StringToHash("RunSpeed"), animBlend);

        ////速度依存でない一時増加
        //if (Mathf.Abs(PlayerScript.adjustLeftStick.x) > PlayerScript.LATERAL_MOVE_THRESHORD)
        //{
        //    PlayerScript.animator.SetFloat(PlayerScript.animHash.RunSpeed, 1.0f, 0.2f, Time.deltaTime);
        //}
        //else
        //{
        //    PlayerScript.animator.SetFloat(PlayerScript.animHash.RunSpeed, 0.0f, 0.2f, Time.deltaTime);
        //}


        if (PlayerScript.animator.GetBool(PlayerScript.animHash.isRunning) == false)
        {
            rareMotionTimer += Time.deltaTime;
            if (rareMotionTimer > 10.0f)
            {
                rareMotionTimer = 0.0f;
                PlayerScript.animator.SetTrigger(PlayerScript.animHash.rareWaitTrigger);

                int motionType = Random.Range(0, 2);
                switch (motionType)
                {
                    case 0:
                        //SE
                        SoundManager.Instance.PlaySound("CVoice_ (25)", 1.0f);
                        break;

                    case 1:
                        SoundManager.Instance.PlaySound("CVoice_ (26)", 1.0f);
                        break;

                    default:
                        Debug.LogWarning("random :Out OfRange");
                        break;
                }
                PlayerScript.animator.SetInteger(PlayerScript.animHash.rareWaitType, motionType);
            }
        }
        else
        {
            rareMotionTimer = 0.0f;
        }
    }


}
