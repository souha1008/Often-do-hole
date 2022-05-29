using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


/// <summary>
/// レール移動時のクラス
/// </summary>
public class PlayerState_Rail : PlayerState
{

    public PlayerState_Rail()
    {
        PlayerScript.refState = EnumPlayerState.RAILING;
        PlayerScript.canShotState = false;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel = Vector3.zero;

        PlayerScript.animator.Play("Shot.midair_roop");
        PlayerScript.animator.SetBool(PlayerScript.animHash.onGround, false);
        BulletScript.SetBulletState(EnumBulletState.STOP);
    }

    public override void UpdateState()
    {
        //キー入力不可

    }

    public override void Move()
    {
        //レールに応じて回転
        RotateTowardAngle();
    }

    private void RotateTowardAngle()
    {
        Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;
        Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);
        Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //補正用クオータニオン
        quaternion *= adjustQua;
        PlayerScript.rb.rotation = quaternion;
    }

    public override void StateTransition()
    {
        //終わったらステート

        ////ボールが触れたらスイング状態
        //if (BulletScript.isTouched)
        //{
        //    PlayerScript.shotState = ShotState.NONE;
        //    if (BulletScript.swingEnd)
        //    {
        //        BulletScript.swingEnd = false;
        //        PlayerScript.mode = new PlayerStateSwing();
        //    }
        //}

    }
    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Rail");
    }
}



