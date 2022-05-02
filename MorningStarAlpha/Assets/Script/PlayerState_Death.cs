using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡時アニメーション等の制御クラス
/// </summary>
public class PlayerStateDeath : PlayerState
{

    float Timer;

            
    public PlayerStateDeath()
    {
        PlayerScript.refState = EnumPlayerState.DEATH;
        PlayerScript.canShotState = false;



        Timer = 0.0f;
    }

    public override void UpdateState()
    {
        // フェード処理
        Timer += Time.deltaTime;


        if (Timer > 0.6)
        {
            FadeManager.Instance.SetNextFade(FADE_STATE.FADE_OUT, FADE_KIND.FADE_GAMOVER);
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