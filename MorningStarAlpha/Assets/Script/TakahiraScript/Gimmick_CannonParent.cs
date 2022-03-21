using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonParent : Gimmick_Main
{
    [Header("[砲台の設定]")]
    [Label("打ち出す弾オブジェクト")]
    public GameObject CannonChild;  // 砲台から打ち出す弾オブジェクト
    [Label("動き出すプレイヤーとの距離")]
    public float StartLength = 15;       // 砲台が動き出すプレイヤーとの距離
    [Label("打ち出す間隔の時間")]
    public float ShootTime = 5;         // 打ち出す間隔

    [Header("[弾の設定]")]
    [Label("弾の速度")]
    public float Speed = 5;        // 弾の速度
    [Label("弾の生存時間")]
    public float LifeTime = 5;     // 弾の生存時間



    private bool StartFlag;     // 起動フラグ
    private float NowShootTime; // 経過時間

    [HideInInspector] public GameObject PlayerObject; // プレイヤーオブジェクト
    

    public override void Init()
    {
        // 初期化
        StartFlag = false;
        NowShootTime = ShootTime; // 最初に1回弾発射

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
                Shoot();
                NowShootTime = 0.0f; // 経過時間リセット
            }

            NowShootTime += Time.fixedDeltaTime; // 経過時間加算
        }
    }

    // 弾発射
    public void Shoot()
    {
        if (CannonChild != null)
        {
            GameObject Child = Instantiate(CannonChild, gameObject.transform.position, Quaternion.Euler(Rad)); // 弾生成

            Child.GetComponent<Gimmick_CannonChild>().SetCannonChild(PlayerObject, Speed, LifeTime); // 弾の値セット
        }
    }

    public override void Death()
    {
        // 自分自身を消す
        Destroy(this.gameObject);
    }
}
