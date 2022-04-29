using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡時アニメーション等の制御クラス
/// </summary>
public class PlayerStateDeath : PlayerState
{
    public PlayerStateDeath()
    {
        PlayerScript.refState = EnumPlayerState.DEATH;
        PlayerScript.canShotState = false;
        PlayerScript.vel = Vector3.zero;
        PlayerScript.addVel = Vector3.zero;

        BulletScript.rb.velocity = Vector3.zero;
        BulletScript.vel = Vector3.zero;
        BulletScript.StopVelChange = true;

        PlayerScript.ResetAnimation();
    }

    public override void UpdateState()
    {
        // フェード処理
        FadeManager.Instance.SetNextFade(FADE_STATE.FADE_OUT, FADE_KIND.FADE_GAMOVER);
    }

    public override void Move()
    {
        //移動なし
    }

    public override void StateTransition()
    {
        //ここから派生することはない
        //シーン変更してクイックリトライ位置にリポップ
    }

}