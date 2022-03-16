using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Thorn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            // プレイヤーを死亡状態に変更

            // プレイヤーにダメージエフェクト

            // ゲームオーバー移行
        }
    }
}
