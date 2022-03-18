using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Thorn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // コリジョン
        this.gameObject.GetComponent<Collider>().isTrigger = true; // トリガーにする
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
            PlayerMain.instance.mode = new PlayerStateDeath();
            // プレイヤーにダメージエフェクト

            // フェード処理
            FadeManager.Instance.SetNextFade(FADE_STATE.FADE_OUT, FADE_KIND.FADE_GAMOVER);
        }
    }
}
