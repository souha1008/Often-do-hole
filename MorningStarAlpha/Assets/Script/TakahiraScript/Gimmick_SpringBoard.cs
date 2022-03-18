using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_SpringBoard : Gimmick_Main
{
    // 変数
    public float SpringPower = 100.0f;   // ジャンプ台の力

    public override void Init()
    {

    }

    public override void Move()
    {
        
    }

    public override void Death()
    {
        
    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            float Rad;           // 回転角
            Vector3 VecPower = Vector3.zero;    // 加えるベクトル量

            PlayerMain playermain = collider.gameObject.GetComponent<PlayerMain>(); // プレイヤーメインスクリプト取得
            Rad = this.transform.localEulerAngles.z;  // ジャンプ台の回転角(度)
            Rad = CalculationScript.AngleCalculation(Rad); // 角度ラジアン変換
            VecPower = CalculationScript.AngleVectorXY(Rad) * SpringPower;  // 飛ぶベクトル量

            if (VecPower.x < 1 && VecPower.x > -1) VecPower.x = 0;  // 小さい値は誤差として0にする
            if (VecPower.y < 1 && VecPower.y > -1) VecPower.y = 0;

            //Debug.Log(VecPower.x);
            //Debug.Log(VecPower.y);

            playermain.vel = VecPower; // プレイヤーのベクトル量変更
        }
    }
}
