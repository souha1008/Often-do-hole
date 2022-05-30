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


    // 弾情報セット
    public void SetCannonChild(float speed, float lifetime, bool chaseflag, Vector3 pos, Quaternion quaternion)
    {
        // 値セット
        NowLifeTime = 0.0f;
        Speed = speed;
        LifeTime = lifetime;
        ChaseFlag = chaseflag;
        transform.SetPositionAndRotation(pos, quaternion);
    }

    public override void Init()
    {
        // 初期化

        //ChasePower = 0.1f;
        //NowLifeTime = 0.0f;

        // コライダー
        //this.GetComponent<Collider>().isTrigger = false; // トリガーオフ

        // 弾に速度を与える
        //Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(transform.rotation.eulerAngles.z)) * Speed;
        //Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(Rad.z)).normalized * Speed; // (ノーマライズ)
    }

    public override void UpdateMove()
    {
        // 追尾処理
        if (ChaseFlag)
        {
            Vector3 PlayerPos = PlayerMain.instance.transform.position;    // プレイヤー座標
            Vector3 ThisPos = this.gameObject.transform.position;   // 自身の座標

            transform.rotation = Quaternion.Euler(0, 0, CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos));    // 回転角
        }

        // 弾に速度を与える
        Vel = CalculationScript.AngleVectorXY(CalculationScript.AngleCalculation(transform.rotation.eulerAngles.z)) * Speed;
        //Vel = (PlayerPos - ThisPos).normalized * Speed; // 追尾(ノーマライズ)

        if (NowLifeTime >= LifeTime) // 生存時間で死亡
        {
            Death();
        }
        NowLifeTime += Time.deltaTime; // 時間加算
    }

    public override void Death()
    {
        //Debug.LogWarning("死亡");
        // 自分自身を消す
        //Destroy(this.gameObject);
        this.gameObject.SetActive(false);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        Debug.LogWarning(collider.gameObject.name);
        // 弾だけ消える
        if (collider.gameObject.CompareTag("Bullet") ||
            (collider.gameObject.CompareTag("Player") && PlayerMain.instance.refState == EnumPlayerState.SWING))
        {
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop(0.1f);

            // 振動
            VibrationManager.Instance.StartVibration(0.4f, 0.4f, 0.2f);

            // 自身が死亡
            Death();
        }

        // ノックバック
        else if (collider.gameObject.CompareTag("Player"))
        {
            // ヒットストップ
            //GameSpeedManager.Instance.StartHitStop(0.1f);

            // エフェクト
            EffectManager.Instance.SharkExplosionEffect(this.transform.position);

            // 振動
            VibrationManager.Instance.StartVibration(0.7f, 0.7f, 0.2f);

            // プレイヤーをノックバック状態に変更
            PlayerMain.instance.mode = new PlayerState_Knockback(this.transform.position, false);

            // 自身が死亡
            Death();
        }

        // 死亡
        Death();
    }
}
