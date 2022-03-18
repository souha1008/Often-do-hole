using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_MoveBlock : Gimmick_Main
{
    // 変数
    public float MoveVelMax = 5.0f;     // 最大移動量
    public float MoveTime = 3.0f;       // 移動時間
    private float NowTime;       // 現在の移動時間

    public override void Init()
    {
        // 初期化
        NowTime = 0.0f;

        // コリジョン
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // トリガーオフ

        // リジッドボディ
        Rb.mass = 100000.0f; // 重くして動かないようにする
    }

    public override void Move()
    {

    }

    public override void Death()
    {

    }

    public override void OnTriggerEnter(Collider collider)
    {
        
    }

    //public void OnCollisionEnter(Collision collision)
    //{

    //}
}
