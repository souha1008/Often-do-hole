using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Wind : Gimmick_Main
{
    // 変数
    public float WindPower = 5.0f;

    public override void Init()
    {

    }

    public override void FixedMove()
    {

    }

    public override void Death()
    {

    }

    public override void OnTriggerEnter(Collider collider)
    {

    }

    public void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            // プレイヤーに風を与える(風係数を渡す)

        }
    }
}
