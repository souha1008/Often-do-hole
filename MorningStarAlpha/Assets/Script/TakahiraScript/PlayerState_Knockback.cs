using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ノックバックステート
public class PlayerState_Knockback : PlayerState
{
    private static float KnockbackTime = 0.8f;       // ノックバック時間
    private static float KnockbackPowerX = 60.0f;    // ノックバック力 X
    private static float KnockbackPowerY = 50.0f;    // ノックバック力 Y


    private float NowTime = 0.0f;               // 経過時間
    private Vector3 HitPos;                     // ヒットしたオブジェクトの座標
    private bool BulletReturnFlag;              // 錨引き戻しフラグ

    private bool isDeath;

    public PlayerState_Knockback(Vector3 HitObjectPos)
    {
        PlayerScript.refState = EnumPlayerState.NOCKBACK;
        NowTime = 0.0f;
        HitPos = HitObjectPos;
        isDeath = false;
        PlayerScript.midairState = MidairState.NORMAL;

        PlayerScript.AnimVariableReset();
        PlayerScript.animator.SetTrigger("NockBack");

        // 錨引き戻し
        BulletScript.ReturnBullet();
        PlayerScript.useVelocity = true;
        BulletReturnFlag = true;


        Knockback(); // ノックバック処理
    }

    public PlayerState_Knockback(Vector3 HitObjectPos, bool is_death)
    {
        PlayerScript.refState = EnumPlayerState.NOCKBACK;
        NowTime = 0.0f;
        HitPos = HitObjectPos;

        isDeath = is_death;

        PlayerScript.midairState = MidairState.NORMAL;

        PlayerScript.AnimVariableReset();
        PlayerScript.animator.SetTrigger("NockBack");

        // 錨引き戻し
        BulletScript.ReturnBullet();
        PlayerScript.useVelocity = true;
        BulletReturnFlag = true;


        Knockback(); // ノックバック処理
    }

    public override void UpdateState()
    {

    }

    public override void Move()
    {
        // 減衰処理
        PlayerSpeedDown();

        //自分へ弾を引き戻す
        if (BulletReturnFlag)
        {
            float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
            Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
            vec = vec.normalized;
            BulletScript.vel = vec * 200.0f;
            //距離が一定以下になったら弾を非アクティブ
            if (interval < 4.0f)
            { 
                BulletReturnFlag = false;
            }
        }


        // 時間経過でステート変更
        if (NowTime > KnockbackTime && !BulletReturnFlag)
        {
            if (isDeath)
            {
                PlayerScript.mode = new PlayerStateDeath();
            }
            else
            {
                if (PlayerScript.isOnGround)
                {
                    PlayerScript.mode = new PlayerStateOnGround();
                }
                else
                {
                    PlayerScript.mode = new PlayerStateMidair(true);
                }
            }
        }


        NowTime += Time.fixedDeltaTime;
    }


    // ノックバック処理
    private void Knockback()
    {
        // ノックバック方向指定
        Vector3 Vec = Player.transform.position - HitPos;

        // キャラの回転
        if (Vec.x < 0)
        {
            PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 90, 0));
        }
        else
        {
            PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 270, 0));
        }

        // ノックバック移動
        if (Vec.x < 0)
        {
            PlayerScript.vel = new Vector3(-KnockbackPowerX, KnockbackPowerY, 0);
        }
        else
        {
            PlayerScript.vel = new Vector3(KnockbackPowerX, KnockbackPowerY, 0);
        }
    }


    // 減衰処理(PlayerStateMidair　Move()　から引用)
    private void PlayerSpeedDown()
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

          if (PlayerScript.midairState == MidairState.NORMAL)
            {
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * (fixedAdjust);
                PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
            }
    }
}
