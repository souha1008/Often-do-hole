using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// レール移動時のクラス
/// </summary>
public class PlayerStateRail : PlayerState
{
    public PlayerStateRail()
    {
        PlayerScript.refState = EnumPlayerState.RAILING;
        PlayerScript.canShotState = false; //撃てない
        PlayerScript.vel = Vector3.zero;   //速度0
        PlayerScript.addVel = Vector3.zero;

        BulletScript.rb.velocity = Vector3.zero;
        BulletScript.vel = Vector3.zero;
        BulletScript.StopVelChange = true;
    }

    public override void UpdateState()
    {
        //キー入力不可
    }

    public override void Move()
    {
        //移動なし
    }

    public override void StateTransition()
    {
        //終わったらステート
    }
}


