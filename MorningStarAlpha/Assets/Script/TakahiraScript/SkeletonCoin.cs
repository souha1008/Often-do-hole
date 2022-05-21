using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCoin : MonoBehaviour
{
    private Animator CoinAnimator;
    private bool OnceFlag = false;

    private void Awake()
    {
        this.gameObject.GetComponent<Collider>().isTrigger = true;  // トリガー
        CoinAnimator = this.gameObject.GetComponent<Animator>();
    }

    public void OnTriggerEnter(Collider collider)
    {
        // プレイヤーと接触時コイン取得
        if (!OnceFlag && (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Bullet")))
        {
            OnceFlag = true;
            SoundManager.Instance.PlaySound("決定音");
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop(0.1f);

            // 振動
            VibrationManager.Instance.StartVibration(0.8f, 0.8f, 0.12f);

            CoinAnimator.SetBool("GetCoin", true);
        }
    }

    public void Death()
    {
        // コイン取得エフェクト
        EffectManager.Instance.CoinGetEffect(transform.position);
        this.gameObject.SetActive(false);
    }
}
