using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonParent : Gimmick_Main
{
    [Label("打ち出す弾オブジェクト")]
    public GameObject CannonChild;  // 砲台から打ち出す弾オブジェクト
    [Label("動き出すプレイヤーとの距離")]
    public float StartLength = 15;       // 砲台が動き出すプレイヤーとの距離
    [Label("打ち出す間隔の時間")]
    public float ShootTime = 20;         // 打ち出す間隔
    

    private bool StartFlag;     // 起動フラグ
    private float NowShootTime; // 経過時間

    private GameObject PlayerObject;
    

    public override void Init()
    {
        // 初期化
        StartFlag = false;
        NowShootTime = 0.0f;

        // プレイヤーオブジェクト取得
        PlayerObject = GameObject.Find("Player");
    }

    public override void FixedMove()
    {
        // プレイヤーの座標
        Vector3 PlayerPos = PlayerObject.transform.position;
        Vector3 ThisPos = this.gameObject.transform.position;


        // プレイヤーとの距離を確認
        if (Vector2.Distance(PlayerPos, ThisPos) <= StartLength)
            StartFlag = true;
        else
            StartFlag = false;


        // 起動時の処理
        if (StartFlag)
        {
            // プレイヤーの方向に向く
            Rad.z = CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos);

            // 弾発射処理
            if (NowShootTime >= ShootTime)
            {
                if (CannonChild != null)
                {
                    Instantiate(CannonChild, ThisPos, Quaternion.Euler(Rad), this.gameObject.transform); // 親子指定して弾生成
                }
                NowShootTime = 0.0f; // 経過時間リセット
            }

            NowShootTime += Time.fixedDeltaTime; // 経過時間加算
        }
    }

    public override void Death()
    {
        // 自分自身を消す
        Destroy(this.gameObject);
    }
}
