using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCoin : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider)
    {
        // プレイヤーと接触時コイン取得
        if (collider.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySound("決定音");
            Destroy(this.gameObject);
        }
    }
}
