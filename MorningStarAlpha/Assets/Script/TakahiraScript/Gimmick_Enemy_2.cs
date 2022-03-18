using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Enemy_2 : Gimmick_Main
{
    // 変数


    // スタート処理
    public override void Init()
    {

    }

    // 敵の動き処理
    public override void Move()
    {

    }

    // 敵死亡処理
    public override void Death() 
    {
        // 死亡エフェクト

        // 自身を消す
        Destroy(this.gameObject);
    }

    // 何かと衝突処理(トリガー)
    public override void OnTriggerEnter(Collider collider) 
    {
        if (collider.gameObject.tag == "Bullet")
        {
            // ヒットストップ

            // 当たったエフェクト

            Death(); // 死亡処理   
        }
    }
}
