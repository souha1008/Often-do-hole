using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCoin : MonoBehaviour
{
    private void Awake()
    {
        this.gameObject.GetComponent<Collider>().isTrigger = true;  // トリガー
    }

    public void OnTriggerEnter(Collider collider)
    {
        // プレイヤーと接触時コイン取得
        if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Bullet"))
        {
            SoundManager.Instance.PlaySound("決定音");
            // ヒットストップ
            GameSpeedManager.Instance.StartHitStop(0.1f);
            Destroy(this.gameObject);
        }
    }
}
