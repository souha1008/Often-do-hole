using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Conveyor : Gimmick_Main
{
    private bool StartFlag;

    public override void Init()
    {
        // 初期化


        // コリジョン
        //Cd.isTrigger = false; // トリガーオフ

        // 角度を0度固定
        Rad = Vector3.zero;
    }

    public override void FixedMove()
    {
        
    }


    public override void Death()
    {
        
    }

    public override void OnCollisionEnter(Collision collision)
    {
        
    }
}
