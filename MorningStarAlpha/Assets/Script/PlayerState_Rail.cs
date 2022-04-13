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
        //Init();
        


        ////弾の発射
        //BulletScript.GetComponent<Collider>().isTrigger = false;
        //BulletScript.VisibleBullet();

        ////if (is_slide_jump)
        ////{
        ////    BulletScript.ShotSlideJumpBullet();
        ////    Debug.Log("Slide Shot");
        ////}
        
        //{
        //    BulletScript.ShotBullet();
        //    Debug.Log("Normal Shot");
        //}
    }

    public override void UpdateState()
    {
        //キー入力不可
    }

    public override void Move()
    {
        
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



