using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonChild : Gimmick_Main
{
    private float Speed;        // 弾の速度
    private float LifeTime;     // 弾の生存時間
    //private float ChasePower;   // 追いかける力

    private float NowLifeTime;  // 現在の生存時間

    private GameObject PlayerObject;    // プレイヤーオブジェクト

    // 弾情報セット
    public void SetCannonChild(GameObject playerobject, float speed, float lifetime)
    {
        Speed = speed;
        LifeTime = lifetime;
        PlayerObject = playerobject;
    }

    public override void Init()
    {
        // 初期化

        //ChasePower = 0.1f;
        NowLifeTime = 0.0f;

        // 現在の角度方向に弾の速度を与える
        Vel = CalculationScript.AngleVectorXY(Rad.z) * Speed;

        // プレイヤーオブジェクト取得
        //PlayerObject = GameObject.Find("Player");
    }

    public override void FixedMove()
    {
        Vector3 PlayerPos = PlayerObject.transform.position;    // プレイヤー座標
        Vector3 ThisPos = this.gameObject.transform.position;   // 自身の座標

        Rad.z = CalculationScript.UnityTwoPointAngle360(ThisPos, PlayerPos);    // 回転角

        Vel = (PlayerPos - ThisPos).normalized * Speed; // 追尾

        if (NowLifeTime >= LifeTime) // 生存時間で死亡
        {
            Death();
        }
        NowLifeTime += Time.fixedDeltaTime; // 時間加算
    }

    public override void Death()
    {
        // 自分自身を消す
        Destroy(this.gameObject);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop();

            // プレイヤーを死亡状態に変更
            PlayerMain.instance.mode = new PlayerStateDeath();

            // 死亡
            Death();
        }

        if (collider.gameObject.tag == "Bullet")
        {
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop();

            // 死亡
            Death();
        }
    }
}
