using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonChild : Gimmick_Main
{
    private float Speed;        // 弾の速度
    private float LifeTime;     // 弾の生存時間
    private bool ChaseFlag;     // 弾が追尾するか
    //private float ChasePower;   // 追いかける力

    private float NowLifeTime;  // 現在の生存時間
    private GameObject PlayerObject;    // プレイヤーオブジェクト

    // 弾情報セット
    public void SetCannonChild(GameObject playerobject, float speed, float lifetime, bool chaseflag)
    {
        Speed = speed;
        LifeTime = lifetime;
        PlayerObject = playerobject;
        ChaseFlag = chaseflag;
    }

    public override void Init()
    {
        // 初期化

        //ChasePower = 0.1f;
        NowLifeTime = 0.0f;

        // コライダー
        //this.GetComponent<Collider>().isTrigger = false; // トリガーオフ

        // 弾に速度を与える
        Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(transform.rotation.eulerAngles.z)) * Speed;
        //Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(Rad.z)).normalized * Speed; // (ノーマライズ)

        // プレイヤーオブジェクト取得
        //PlayerObject = GameObject.Find("Player");
    }

    public override void UpdateMove()
    {
        // 追尾処理
        if (ChaseFlag)
        {
            Vector3 PlayerPos = PlayerObject.transform.position;    // プレイヤー座標
            Vector3 ThisPos = this.gameObject.transform.position;   // 自身の座標

            transform.rotation = Quaternion.Euler(0, 0, CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos));    // 回転角

            Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(transform.rotation.eulerAngles.z)) * Speed; // 追尾
            //Vel = (PlayerPos - ThisPos).normalized * Speed; // 追尾(ノーマライズ)
        }

        if (NowLifeTime >= LifeTime) // 生存時間で死亡
        {
            Death();
        }
        NowLifeTime += Time.deltaTime; // 時間加算
    }

    public override void Death()
    {
        // 自分自身を消す
        Destroy(this.gameObject);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            // ヒットストップ
            //GameSpeedManager.Instance.StartHitStop(0.1f);

            // プレイヤーを死亡状態に変更
            PlayerMain.instance.mode = new PlayerStateDeath();

            // 死亡
            Death();
        }

        if (collider.gameObject.CompareTag("Bullet"))
        {
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop(0.1f);

            // 死亡
            Death();
        }
    }
}
