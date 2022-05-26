using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Thorn : Gimmick_Main
{
    public override void Init()
    {
        Rb.isKinematic = true;
        Cd.isTrigger = false;
        this.gameObject.tag = "Thorn";
    }

    public override void FixedMove()
    {
        
    }

    public override void Death()
    {
        
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {   
            if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
            {
                // ヒットストップ
                GameSpeedManager.Instance.StartHitStop(0.1f);

                // 振動
                VibrationManager.Instance.StartVibration(1.0f, 1.0f, 0.22f);

                // 音
                SoundManager.Instance.PlaySound("sound_21", 0.2f, 0.1f);

                // プレイヤーステートを死亡に変更
                PlayerMain.instance.mode = new PlayerStateDeath_Thorn(gameObject.transform.position);
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // ヒットストップ
                GameSpeedManager.Instance.StartHitStop(0.1f);

                // 振動
                VibrationManager.Instance.StartVibration(1.0f, 1.0f, 0.22f);

                // 音
                SoundManager.Instance.PlaySound("sound_21", 1.0f, 0.2f);

                // プレイヤーステートを死亡に変更
                PlayerMain.instance.mode = new PlayerStateDeath_Thorn(gameObject.transform.position);
            }
        }
    }
}
