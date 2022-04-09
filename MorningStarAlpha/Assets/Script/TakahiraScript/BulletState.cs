using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumBulletState
{
    BulletReady = 0,
    BulletGo,
    BulletStop,
    BulletReturn,
    BulletReturnFollow
}


// 錨用のステート
public abstract class BulletState
{
    virtual public void Update() { }      // Updateで使う
    virtual public void FixedUpdate() { } // FixedUpdateで使う
    virtual public void StateTransition() { }  //継承先でシーンの移動を決める
    virtual public void DebugMessage() { }     //デバッグ用のメッセージ表示

    static public GameObject Bullet;
    static public PlayerMain PlayerScript;
    static public BulletMain BulletScript;

    
}


// 手に持ってるステート(このステート以外発射不可)
public class BulletReady : BulletState
{
    public BulletReady()
    {
        // 初期化
        BulletScript.NowBulletState = EnumBulletState.BulletReady;
        BulletScript.InvisibleBullet();
    }

    public override void Update()
    {
        // 撃つ
        if (BulletScript.CanShotFlag &&
            Input.GetButtonDown("Button_R") &&
            PlayerScript.CanShotColBlock &&
            PlayerScript.stickCanShotRange)
        {
            BulletScript.SetBulletState(EnumBulletState.BulletGo);
            BulletScript.CanShotFlag = false;
        }

        // バレットの位置を常にスティック方向に調整
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;
        vec = vec * 3;
        vec.y += 1.0f;
        Vector3 adjustPos = PlayerScript.transform.position + vec;

        BulletScript.transform.position = adjustPos;
    }
}


// 発射中ステート
public class BulletGo : BulletState
{
    private int ExitFlameCnt = 0;//存在し始めてからのカウント

    public BulletGo()
    {
        // 初期化
        BulletScript.NowBulletState = EnumBulletState.BulletGo;
        BulletScript.VisibleBullet();
        BulletScript.co.isTrigger = false;


        // 発射
        if (Mathf.Abs(PlayerScript.vel.x) > PlayerScript.MAX_RUN_SPEED &&
           ((PlayerScript.vel.x > 0 && PlayerScript.adjustLeftStick.x > 0) ||
           (PlayerScript.vel.x < 0 && PlayerScript.adjustLeftStick.x < 0)))
        {
            BulletScript.ShotSlideJumpBullet(); // スライドジャンプ
            //BulletScript.ShotBullet();
        }
        else
        {
            BulletScript.ShotBullet();
        }      
    }

    public override void Update()
    {
        if (Input.GetButton("Button_R"))
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
        else
        {
            BulletScript.SetBulletState(EnumBulletState.BulletReturn);
        }
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
        BulletScript.NowBulletState = EnumBulletState.BulletStop;
        BulletScript.vel = Vector3.zero;
        BulletScript.rb.isKinematic = true;
        BulletScript.CanShotFlag = true;
    }

    public override void FixedUpdate()
    {
        if (!Input.GetButton("Button_R"))
        {
            BulletScript.SetBulletState(EnumBulletState.BulletReturn);
        }
    }
}


// 引き戻しステート
public class BulletReturn : BulletState
{
    public BulletReturn()
    {
        // 初期化
        BulletScript.NowBulletState = EnumBulletState.BulletReturn;
        BulletScript.ReturnBullet();       
    }

    public override void FixedUpdate()
    {
        //自分へ弾を引き戻す
        Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
        vecToPlayer = vecToPlayer.normalized;
        BulletScript.vel = vecToPlayer * 100;


        //距離が一定以下になったら終了
        if (Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position) < 4.0f)
        {
            BulletScript.SetBulletState(EnumBulletState.BulletReady);
        }
    }
}


// 錨にプレイヤーが引っ張られて回収
public class BulletReturnFollow : BulletState
{
    public BulletReturnFollow()
    {
        // 初期化
        BulletScript.NowBulletState = EnumBulletState.BulletReturnFollow;
        BulletScript.ReturnBullet();
    }

    public override void FixedUpdate()
    {
        //自分へ弾を引き戻す
        Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
        vecToPlayer = vecToPlayer.normalized;
        BulletScript.vel = vecToPlayer * 4;


        //距離が一定以下になったら終了
        if (Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position) < 4.0f)
        {
            BulletScript.SetBulletState(EnumBulletState.BulletReady);
        }
    }
}
