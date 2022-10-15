using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonChild_Bullet : Gimmick_CannonChild
{
    public override void OnTriggerEnter(Collider collider)
    {
        //Debug.LogWarning(collider.gameObject.name);
        // 弾だけ消える
        if (collider.gameObject.CompareTag("Bullet") ||
            (collider.gameObject.CompareTag("Player") && PlayerMain.instance.refState == EnumPlayerState.SWING))
        {
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop(0.1f);

            // 振動
            VibrationManager.Instance.StartVibration(0.4f, 0.4f, 0.2f);

            // 自身が死亡
            Death();
        }

        // ノックバック
        else if (collider.gameObject.CompareTag("Player"))
        {
            // ヒットストップ
            //GameSpeedManager.Instance.StartHitStop(0.1f);

            // エフェクト
            EffectManager.Instance.SharkExplosionEffect(this.transform.position);

            // 振動
            VibrationManager.Instance.StartVibration(0.7f, 0.7f, 0.2f);

            // プレイヤーをノックバック状態に変更
            PlayerMain.instance.mode = new PlayerState_Knockback(this.transform.position, false);

            // 自身が死亡
            Death();
        }

        // 死亡
        Death();
    }
}
