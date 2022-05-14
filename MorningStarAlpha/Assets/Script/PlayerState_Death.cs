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