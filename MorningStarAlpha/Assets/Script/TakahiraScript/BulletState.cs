using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumBulletState
{
    BulletReady = 0,
    BulletGo,
    BulletStop,
    BulletReturn
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
        if (Input.GetButtonDown("Button_R"))
        {
            BulletScript.mode = new BulletGo();
        }
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


        // プレイヤーの移動量　+　発射ベクトル量　を初期ベクトル量にする
        //if (PlayerScript.vel.x < )

        BulletScript.ShotSlideJumpBullet();
    }

    public override void Update()
    {
        if (Input.GetButton("Button_R"))
        {
            // 錨の動き
            if (!BulletScript.StopVelChange)
            {
                BulletScript.RotateBullet();
                ExitFlameCnt++;
                //定数秒以上経ってたら
                if (ExitFlameCnt > BulletScript.STRAIGHT_FLAME_CNT)
                {
                    //重力加算
                    BulletScript.vel += Vector3.down * PlayerScript.STRAINED_GRAVITY * (BulletMain.fixedAdjust);
                }

                Mathf.Max(BulletScript.vel.y, BulletMain.BULLET_MAXFALLSPEED * -1);
            }
        }
        else
        {
            BulletScript.mode = new BulletReturn();
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
            BulletScript.mode = new BulletReady();
        }
    }
}
