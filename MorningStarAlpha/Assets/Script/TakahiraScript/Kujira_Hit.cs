using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kujira_Hit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.tag = "Thorn";
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
            {
                // ヒットストップ
                GameSpeedManager.Instance.StartHitStop(0.1f);

                // 振動
                VibrationManager.Instance.StartVibration(1.0f, 1.0f, 0.3f);

                // 音
                //SoundManager.Instance.PlaySound("sound_21", 0.2f, 0.1f);

                // プレイヤーステートをクジラ死亡に変更
                PlayerMain.instance.mode = new PlayerStateDeath_Kujira();
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.refState != EnumPlayerState.DEATH)
            {
                // ヒットストップ
                GameSpeedManager.Instance.StartHitStop(0.1f);

                // 振動
                VibrationManager.Instance.StartVibration(1.0f, 1.0f, 0.3f);

                // 音
                //SoundManager.Instance.PlaySound("sound_21", 1.0f, 0.2f);

                // プレイヤーステートをクジラ死亡に変更
                PlayerMain.instance.mode = new PlayerStateDeath_Kujira();
            }
        }
    }
}
