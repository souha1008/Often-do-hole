using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_FallBlock : Gimmick_Main
{
    [Header("落下距離")]
    public float FallLength = 15.0f;   // 落ちる距離

    [Header("何秒かけて落下するか")]
    public float FallTime = 3.0f;       // 何秒かけて落下するか

    private bool NowFall;               // 落下中か
    private float NowTime;              // 経過時間
    private Vector3 StartPos;           // 初期座標
    private GameObject PlayerObject;    // プレイヤーオブジェクト

    public override void Init()
    {
        // 初期化
        NowFall = false;
        NowTime = 0.0f;
        StartPos = this.gameObject.transform.position;
        PlayerObject = null;

        // コリジョン
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // トリガーオフ

        // リジッドボディ
        Rb.isKinematic = true;  // キネマティックオン
    }

    public override void UpdateMove()
    {
        // ポジション変更
        if (NowFall)
        {
            //Vector3 OldPos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(StartPos.x, Easing.QuartIn(NowTime, FallTime, StartPos.y, StartPos.y - FallLength), StartPos.z);
            NowTime += Time.deltaTime;

            // プレイヤーからレイ飛ばして真下にブロックがあったら
            //if (PlayerObject != null)
            //{
            //    Ray ray = new Ray(PlayerObject.transform.position, Vector3.down);
            //    RaycastHit hit;
            //    if (Physics.Raycast(ray, out hit, 1.5f))
            //    {
            //        if(hit.collider.gameObject == this.gameObject)
            //        {
            //            PlayerObject.gameObject.transform.position =
            //                PlayerObject.gameObject.transform.position + new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
            //        }
            //    }
            //}
        }
        if (NowTime > FallTime)
        {
            Death();
        }
    }

    public override void Death()
    {
        // 自分自身を消す
        Destroy(this.gameObject);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        // プレイヤーか錨と接触
        if (collision.gameObject.tag == "Bullet")
        {
            NowFall = true; // 落下中
            //Rb.isKinematic = false;
        }
        else if (collision.gameObject.tag == "Player")
        {
            // プレイヤーからレイ飛ばして真下にブロックがあったら落下
            Ray ray_1 = new Ray(collision.gameObject.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray_1, out hit, 1.5f))
            {
                NowFall = true; // 落下中
                //Rb.isKinematic = false;
                PlayerObject = collision.gameObject;
            }
        }
    }
}
