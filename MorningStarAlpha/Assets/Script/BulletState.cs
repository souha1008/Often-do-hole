using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumBulletState
{
    READY = 0,
    GO,
    STOP,
    RETURN,
    BulletReturnFollow
}


// 錨用のステート
public abstract class BulletState
{
    virtual public void Update() { }      // Updateで使う
    virtual public void Move() { } // FixedUpdateで使う
    virtual public void StateTransition() { }  //継承先でシーンの移動を決める
    virtual public void DebugMessage() { }     //デバッグ用のメッセージ表示


    static public PlayerMain PlayerScript;
    static public BulletMain BulletScript;

    protected void AdjustBulletPos()
    {
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;
        vec = vec * 1.0f;
        vec.y += 1.0f;
        Vector3 adjustPos = PlayerScript.rb.position + vec;

        BulletScript.rb.position = adjustPos;
    }
}


// 手に持ってるステート(このステート以外発射不可)
public class BulletReady : BulletState
{
    public BulletReady()
    {
        // 初期化
        BulletScript.NowBulletState = EnumBulletState.READY;
        BulletScript.InvisibleBullet();
        BulletScript.isTouched = false;
        BulletScript.co.enabled = false;
        BulletScript.co.isTrigger = true;
        BulletScript.CanShotFlag = true;
    }

    public override void Move()
    {
        // バレットの位置を常にスティック方向に調整
        AdjustBulletPos();
    }
}


// 発射中ステート
public class BulletGo : BulletState
{
    private int ExitFlameCnt = 0;//存在し始めてからのカウント

    public BulletGo()
    {
        // 初期化
        BulletScript.NowBulletState = EnumBulletState.GO;
        BulletScript.VisibleBullet();
        BulletScript.co.enabled = true;
        BulletScript.co.isTrigger = false;
        BulletScript.CanShotFlag = false;

        AdjustBulletPos();

        BulletScript.ShotBullet();   
    }

    public override void Move()
    {  
        // 錨の動き
        BulletScript.RotateBullet();
        ExitFlameCnt++;
        //定数秒以上経ってたら
        if (ExitFlameCnt > BulletScript.STRAIGHT_FLAME_CNT)
        {
            //重力加算
            BulletScript.vel += Vector3.down * PlayerScript.STRAINED_GRAVITY * BulletScript.fixedAdjust;
        }

        Mathf.Max(BulletScript.vel.y, BulletScript.BULLET_MAXFALLSPEED * -1);       
    }

    // BulletMainの OnCollisionEnter 内で
    // 今、BulletGoステートで当たったタグが" "ならステート変える(BulletStop  or BulletReturn)
}


// くっついたステート
public class BulletStop : BulletState
{
    public BulletStop()
    {
        // 初期化
        
        BulletScript.NowBulletState = EnumBulletState.STOP;
        BulletScript.co.enabled = false;
        BulletScript.CanShotFlag = false;
        BulletScript.co.isTrigger = true;
        BulletScript.StopBullet();
        
    }

    public override void Move()
    {
        //BulletScript.RotateBullet();
    }
}


// 引き戻しステート
public class BulletReturn : BulletState
{
    float ratio;
    Vector3 maxPos;

    public BulletReturn()
    {
        // 初期化
        BulletScript.NowBulletState = EnumBulletState.RETURN;

        BulletScript.rb.velocity = Vector3.zero;
        BulletScript.vel = Vector3.zero;
        BulletScript.rb.isKinematic = false;
        BulletScript.co.enabled = false;
        BulletScript.co.isTrigger = true;
        BulletScript.StopVelChange = true;
        BulletScript.CanShotFlag = false;

        ratio = 0.0f;


        maxPos = PlayerScript.rb.position + ((BulletScript.rb.position - PlayerScript.rb.position).normalized * BulletScript.BULLET_ROPE_LENGTH);

    }

    public override void Move()
    {

#if true
        ratio += 0.05f;
        float easeRatio = Easing.Linear(ratio, 1.0f, 0.0f, 1.0f);
        //弾と自分の位置で補完 
        Vector3 BulletPosition = maxPos * (1 - easeRatio) + PlayerScript.rb.position * easeRatio;
        BulletScript.rb.position = BulletPosition;

        //距離が一定以下になったら終了
        if (easeRatio >= 0.8f)
        {
            BulletScript.SetBulletState(EnumBulletState.READY);
        }

#else
        //自分へ弾を引き戻す
        Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
        vecToPlayer = vecToPlayer.normalized;
        BulletScript.vel = vecToPlayer * 200;

        //距離が一定以下になったら終了
        if (Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position) < 4.0f)
        {
            BulletScript.SetBulletState(EnumBulletState.READY);
        }

#endif
    }
}


//// 錨にプレイヤーが引っ張られて回収
//public class BulletReturnFollow : BulletState
//{
//    public BulletReturnFollow()
//    {
//        // 初期化
//        BulletScript.NowBulletState = EnumBulletState.BulletReturnFollow;
//        BulletScript.ReturnBullet();
//    }

//    public override void Move()
//    {
//        //自分へ弾を引き戻す
//        Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
//        vecToPlayer = vecToPlayer.normalized;
//        BulletScript.vel = vecToPlayer * 4;


//        //距離が一定以下になったら終了
//        if (Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position) < 4.0f)
//        {
//            BulletScript.SetBulletState(EnumBulletState.READY);
//        }
//    }
//}
