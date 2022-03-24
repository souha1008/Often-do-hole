using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Barrel : PlayerState
{
    private float WaitTime;     // 発射までの待機時間
    private Vector3 JumpPower;  // ジャンプ力
    private Vector3 GimmickPos; // ギミックの座標
    private float NowTime;      // 現在の経過時間
    private Vector3 StartPlayerPos; // ギミックに触れた瞬間のプレイヤー座標
    private bool StateChangeFlag;   // ステート移行フラグ
    BulletMain BulletScript;        // バレットスクリプト
    private static float WaitTimeMin = 0.25f;    // 最低待機時間
    public PlayerState_Barrel(float waittime, Vector3 jumppower, Vector3 gimmickpos) // コンストラクタ
    {
        if (waittime <= WaitTimeMin)                           // 発射までの待機時間セット( WaitTimeMin 以下は WaitTimeMin にする)
            WaitTime = WaitTimeMin;          
        else 
            WaitTime = waittime;
        JumpPower = jumppower;                          // ジャンプ力セット
        GimmickPos = gimmickpos;                        // ギミックの座標セット
        NowTime = 0.0f;                                 // 時間リセット
        StartPlayerPos = PlayerScript.transform.position;     // 触れた瞬間の座標セット
        StateChangeFlag = false;
        PlayerScript.vel = Vector3.zero;                // 移動量ゼロ
        PlayerScript.canShotState = false;              // 弾打てない
        PlayerScript.addVel = Vector3.zero;             // ギミックでの移動量ゼロ
        PlayerScript.ForciblyReturnBullet(false);       // 弾の引き戻し
        if (PlayerScript.Bullet != null)
            BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //バレット情報のスナップ(弾無いときは参照しない)
        else
            BulletScript = null;
    }

    public override void UpdateState() // Update
    {
        NowTime += Time.deltaTime;
    }

    public override void Move() // FixedUpdate
    {
        // 移動
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
            if (BulletScript != null)
                GameObject.Destroy(BulletScript.gameObject);
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
            if (BulletScript != null)
            {
                //if (PlayerScript.forciblyReturnSaveVelocity)
                //{
                //    PlayerScript.vel = bulletVecs.Dequeue();
                //}
                //else
                //{
                //    PlayerScript.vel = Vector3.zero;
                //}
                PlayerScript.vel = Vector3.zero;

                BulletScript.ReturnBullet();
                PlayerScript.useVelocity = true;
                PlayerScript.shotState = ShotState.RETURN;
            }
        }
    }

    public override void StateTransition() // シーン移動
    {
        if (StateChangeFlag)
        {
            PlayerScript.shotState = ShotState.NONE;
            PlayerScript.mode = new PlayerStateMidair(1.0f);
        } 
    }
}
