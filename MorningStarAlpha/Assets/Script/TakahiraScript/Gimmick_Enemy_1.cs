using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Enemy_1 : Enemy_Interface
{
    public float MoveVelX = 10.0f;
    public float MoveVelXMax = 1400.0f;
    private float MoveVelXMaxHalf;

    // スタート処理
    public override void Init()
    {
        Vel.x = MoveVelX;
        MoveVelXMaxHalf = MoveVelXMax / 2;
    }

    // 敵の動き処理
    public override void Move() 
    {
        if (TotalMoveVel.x > MoveVelXMaxHalf)
        {
            Vel.x = -MoveVelX;
        }
        else if (TotalMoveVel.x < -MoveVelXMaxHalf)
        {
            Vel.x = MoveVelX;
        }
    }

    // 敵死亡処理
    public override void Death() 
    {
        // 死亡エフェクト

        // 自身を消す
        Destroy(this.gameObject);
    } 

    // 何かと衝突判定
    public override void OnTriggerEnter(Collider collider) 
    { 
        if (collider.gameObject.tag == "Player") // 当たったオブジェクトが錨だったらに変更予定
        {
            // ヒットストップ

            // 当たったエフェクト

            Death(); // 死亡処理   
        }
    }
}
