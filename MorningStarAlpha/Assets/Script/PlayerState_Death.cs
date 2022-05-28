using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum DeathType
{
    THORN,  //針
    VOID,   //奈落
}


/// <summary>
/// 死亡時アニメーション等の制御クラス
/// </summary>
/// 
public class PlayerStateDeath_Thorn : PlayerState
{

    float Timer;

    private static float KnockbackPowerX = 40.0f;    // ノックバック力 X
    private static float KnockbackPowerY = 20.0f;    // ノックバック力 Y
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

        //アニメ用
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

    public override void UpdateState()
    {
        // フェード処理
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
        //ここから派生することはない
        //シーン変更してクイックリトライ位置にリポップ
    }

}


/// <summary>
/// 死亡時アニメーション等の制御クラス
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

        //アニメ用
        PlayerScript.animator.SetFloat("NockbackSpeed", 8.0f);
        PlayerScript.animator.Play("NockBack");
        PlayerScript.animator.SetBool(PlayerScript.animHash.IsDead, true);

      
    }

    public override void UpdateState()
    {
        // フェード処理
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
        //ここから派生することはない
        //シーン変更してクイックリトライ位置にリポップ
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

            //アニメ用
            PlayerScript.ResetAnimation();
            //なし
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
        // フェード処理
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
        //ここから派生することはない
        //シーン変更してクイックリトライ位置にリポップ
    }

}