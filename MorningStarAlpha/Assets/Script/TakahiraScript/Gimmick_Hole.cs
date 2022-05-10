using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Hole : Gimmick_Main
{
    public override void Init()
    {
        // メッシュを見えなくする
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public override void FixedMove()
    {

    }

    public override void Death()
    {

    }

    public override void OnTriggerEnter(Collider collider)
    {
        if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                // プレイヤーを死亡状態に変更
                PlayerMain.instance.mode = new PlayerStateDeath_Void();
                // プレイヤーにダメージエフェクト
            }
        }
    }
}
