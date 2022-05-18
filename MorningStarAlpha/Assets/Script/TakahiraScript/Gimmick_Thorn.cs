using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Thorn : Gimmick_Main
{
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
        if (collider.gameObject.CompareTag("Player"))
        {
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop(0.1f);
            // プレイヤーを死亡状態に変更
            if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
            {
                PlayerMain.instance.mode = new PlayerStateDeath_Thorn();
            }
            // プレイヤーにダメージエフェクト
        }
    }
}
