using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerState_Barrel : PlayerState
{
    private float WaitTime;     // 発射までの待機時間
    private Vector3 JumpPower;  // ジャンプ力
    private Vector3 GimmickPos; // ギミックの座標
    private bool isPlayerTouch; // プレイヤーが触ったか弾が触ったか
    private float NowTime;      // 現在の経過時間
    private Vector3 StartPlayerPos; // ギミックに触れた瞬間のプレイヤー座標
    private Vector3 StartBulletPos; // ギミックに触れた瞬間の弾座標
    private bool StateChangeFlag;   // ステート移行フラグ
    BulletMain BulletScript;        // バレットスクリプト
    private static float WaitTimeMin = 0.25f;    // 最低待機時間


    //==================================================================
    // 引数：waittime = 発射までの待機時間
    //     ：jumppower = ジャンプ力
    //     ：gimmickpos = 引き寄せる座標
    //     ：isplayertouch = true:プレイヤーが触った, false:弾が触った
    //==================================================================
    public PlayerState_Barrel(float waittime, Vector3 jumppower, Vector3 gimmickpos, bool isplayertouch) // コンストラクタ
    {
        if (waittime <= WaitTimeMin)                           // 発射までの待機時間セット( WaitTimeMin 以下は WaitTimeMin にする)
            WaitTime = WaitTimeMin;          
        else 
            WaitTime = waittime;
        JumpPower = jumppower;                          // ジャンプ力セット
        GimmickPos = gimmickpos;                        // ギミックの座標セット
        isPlayerTouch = isplayertouch;                  // プレイヤーか弾かセット
        NowTime = 0.0f;                                 // 時間リセット
        StartPlayerPos = PlayerScript.transform.position;   // 触れた瞬間の座標セット
        StateChangeFlag = false;
        PlayerScript.vel = Vector3.zero;                // 移動量ゼロ
        PlayerScript.canShotState = false;              // 弾打てない
        PlayerScript.addVel = Vector3.zero;             // ギミックでの移動量ゼロ
        PlayerScript.forciblyReturnBulletFlag = true;   // 弾の引き戻し
        if (PlayerScript.Bullet != null)
            BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //バレット情報のスナップ(弾無いときは参照しない)
        else
            BulletScript = null;    // 弾null
    }

    public override void UpdateState() // Update
    {
        NowTime += Time.deltaTime;
    }

    public override void Move() // FixedUpdate
    {
        // プレイヤー移動
        if (NowTime <= WaitTimeMin) // 最低待機時間だけ中心に向かって移動
        {
            PlayerScript.transform.position =
               new Vector3(Easing.QuadInOut(NowTime, WaitTimeMin, StartPlayerPos.x, GimmickPos.x),
               Easing.QuadInOut(NowTime, WaitTimeMin, StartPlayerPos.y, GimmickPos.y), GimmickPos.z);
            Debug.Log("中心に移動");
        }
        else // それ以外は中心でとどまる
        {
            PlayerScript.transform.position = GimmickPos;
            PlayerScript.GetComponent<MeshRenderer>().enabled = false; // メッシュ切り替え
            Debug.Log("中心でとどまる");
        }

        // 発射
        if (NowTime > WaitTime)
        {
            PlayerScript.addVel = JumpPower;
            PlayerScript.GetComponent<MeshRenderer>().enabled = true; // メッシュ切り替え
            StateChangeFlag = true;
            Debug.Log("発射");
        }

        //アンカーが刺さらない壁にあたったときなど、外部契機で引き戻しに移行
        if (PlayerScript.forciblyReturnBulletFlag)
        {
            PlayerScript.forciblyReturnBulletFlag = false;
            if (PlayerScript.Bullet != null)
            {
                PlayerScript.vel = Vector3.zero;

                StartBulletPos = BulletScript.transform.position;
                BulletScript.ReturnBullet();
                PlayerScript.useVelocity = true;
                PlayerScript.shotState = ShotState.RETURN;
                //Debug.Log("弾リターン");
            }
            else
            {
                PlayerScript.shotState = ShotState.NONE;
                //Debug.Log("弾ノン");
            }
        }

        // 弾の引き戻し
        switch (PlayerScript.shotState)
        {
            case ShotState.RETURN:
                float interval;
                interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);


                if (isPlayerTouch) // プレイヤーが先に触れたか
                {
                    //自分へ弾を引き戻す
                    Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
                    vecToPlayer = vecToPlayer.normalized;

                    BulletScript.vel = vecToPlayer * 100;
                }
                else
                {
                    // 弾移動
                    if (NowTime <= WaitTimeMin / 2) // 最低待機時間の半分だけ中心に向かって移動
                    {
                        BulletScript.transform.position =
                           new Vector3(Easing.QuadInOut(NowTime, WaitTimeMin / 2, StartBulletPos.x, GimmickPos.x),
                           Easing.QuadInOut(NowTime, WaitTimeMin / 2, StartBulletPos.y, GimmickPos.y), GimmickPos.z);
                    }
                    else // ギミックの座標に固定
                    {
                        BulletScript.transform.position = GimmickPos;
                    }
                }

                if (interval < 4.0f || NowTime > WaitTime)
                {
                    if (PlayerScript.Bullet != null)
                        GameObject.Destroy(PlayerScript.Bullet); // 弾破壊
                    PlayerScript.shotState = ShotState.NONE;
                }
                break;

            case ShotState.NONE:
                break;

            default:
                break;
        }
    }

    public override void StateTransition() // シーン移動
    {
        if (StateChangeFlag)
        {
            PlayerScript.mode = new PlayerStateMidair(1.0f);
        } 
    }
}
