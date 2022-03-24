using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Barrel : Gimmick_Main
{
    [Label("発射までの待機時間")]
    public float WaitTime = 1.5f;
    [Label("発射の威力")]
    public float SpringPower = 50.0f;

    private bool StartFlag; // 起動フラグ
    private float NowTime;  // 経過時間
    //private Vector3 StartPlayerPos;     // プレイヤーの初期座標
    private GameObject PlayerObject;    // プレイヤーオブジェクト
    private Vector3 SpringVec;          // 飛ぶベクトル量

    public override void Init()
    {
        // 初期化
        NowTime = 0.0f;
        StartFlag = false;
        PlayerObject = GameObject.Find("Player");

        SpringVec = JumpPower();
    }

    public override void FixedMove()
    {
        if (StartFlag)
        {
            if (NowTime >= WaitTime) // 発射時間経過した
            {
                Jump(); // 発射
                NowTime = 0.0f; // 時間リセット
                StartFlag = false; // 起動フラグオフ
            }
            else
            {
                PlayerPosChange(); // プレイヤーの位置移動
                NowTime += Time.fixedDeltaTime; // 時間加算
            }
        }
    }

    private void Jump()
    {
        if (PlayerObject != null)
        {
            //SpringVec = JumpPower();
        }   
    }

    private Vector3 JumpPower()
    {
        float ThisRad;           // 回転角
        Vector3 VecPower = Vector3.zero;    // 加えるベクトル量

        ThisRad = this.transform.localEulerAngles.z;  // 樽の回転角(度)
        ThisRad = CalculationScript.AngleCalculation(ThisRad); // 角度ラジアン変換
        VecPower = CalculationScript.AngleVectorXY(ThisRad) * SpringPower;  // 飛ぶベクトル量

        if (VecPower.x < 1 && VecPower.x > -1) VecPower.x = 0;  // 小さい値は誤差として0にする
        if (VecPower.y < 1 && VecPower.y > -1) VecPower.y = 0;

        return VecPower;
    }

    private void PlayerPosChange()
    {
        if (PlayerObject != null)
        {
            //if (NowTime <= WaitTime / 5.0f) // 1/5の時間だけ中心に向かって移動
            //{
            //     PlayerObject.transform.position =
            //        new Vector3(Easing.QuadInOut(NowTime, WaitTime / 5.0f, StartPlayerPos.x, gameObject.transform.position.x), 
            //        Easing.QuadInOut(NowTime, WaitTime / 5.0f, StartPlayerPos.y, gameObject.transform.position.y), gameObject.transform.position.z);
            //}
            //else // それ以外は中心でとどまる
            //{
            //    PlayerObject.transform.position = this.gameObject.transform.position;
            //}
        }
    }

    public override void Death()
    {

    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && !StartFlag)
        {
            PlayerObject = collider.gameObject;
            //StartPlayerPos = PlayerObject.transform.position;
            //PlayerObject.GetComponent<PlayerMain>().forciblyReturnBulletFlag = true;
            PlayerObject.GetComponent<PlayerMain>().mode = new PlayerState_Barrel(WaitTime, SpringVec, gameObject.transform.position, true);
            StartFlag = true;
        }
        if (collider.gameObject.tag == "Bullet" && !StartFlag)
        {
            PlayerObject = GameObject.Find("Player");
            //StartPlayerPos = PlayerObject.transform.position;
            //PlayerObject.GetComponent<PlayerMain>().forciblyReturnBulletFlag = true;
            PlayerObject.GetComponent<PlayerMain>().mode = new PlayerState_Barrel(WaitTime, SpringVec, gameObject.transform.position, false);
            StartFlag = true;
        }
    }
}
