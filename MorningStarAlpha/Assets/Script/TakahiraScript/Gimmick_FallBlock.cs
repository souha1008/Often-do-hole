using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_FallBlock : Gimmick_Main
{
    [Label("落下距離")]
    public float FallLength = 15.0f;    // 落ちる距離

    [Label("何秒かけて落下するか")]
    public float FallTime = 3.0f;       // 何秒かけて落下するか

    private bool NowFall;               // 落下中か
    private float NowTime;              // 経過時間
    private Vector3 StartPos;           // 初期座標
    private GameObject PlayerObject;    // プレイヤーオブジェクト
    private GameObject BulletObject;    // 錨オブジェクト
    private BulletMain BulletMainScript;// 錨メインスクリプト

    public override void Init()
    {
        // 初期化
        NowFall = false;
        NowTime = 0.0f;
        StartPos = this.gameObject.transform.position;
        PlayerObject = GameObject.Find("Player");
        BulletObject = null;
        BulletMainScript = null;

        // コリジョン
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // トリガーオフ

        // リジッドボディ
        Rb.isKinematic = true;  // キネマティックオン
    }

    public override void FixedMove()
    {
        // ポジション変更
        if (NowFall)
        {
            Vector3 OldPos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(StartPos.x, Easing.QuartIn(NowTime, FallTime, StartPos.y, StartPos.y - FallLength), StartPos.z);
            NowTime += Time.deltaTime;

            // プレイヤーからレイ飛ばして真下にブロックがあったら
            if (PlayerObject != null)
            {
                Ray ray = new Ray(PlayerObject.transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1.5f))
                {
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        PlayerObject.gameObject.transform.position =
                            PlayerObject.gameObject.transform.position + new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
                    }
                }
            }

            if (BulletObject != null && BulletMainScript != null)
            {
                if (BulletMainScript.isTouched)
                {
                    BulletObject.gameObject.transform.position =
                            BulletObject.gameObject.transform.position + new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
                }
                else
                {
                    BulletObject = null;
                    BulletMainScript = null;
                }
            }
        }
        if (NowTime > FallTime)
        {
            Death();
        }
    }

    public override void Death()
    {
        // プレイヤーの錨引き戻し
        if(PlayerObject != null)
        {
            PlayerObject.GetComponent<PlayerMain>().endSwing = true;
        }

        // 自分自身を消す
        Destroy(this.gameObject);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        // プレイヤーか錨と接触
        if (collision.gameObject.tag == "Bullet")
        {
            NowFall = true; // 落下中
            BulletObject = collision.gameObject;
            BulletMainScript = collision.gameObject.GetComponent<BulletMain>();
        }
        else if (collision.gameObject.tag == "Player")
        {
            // プレイヤーからレイ飛ばして真下にブロックがあったら落下
            Ray ray_1 = new Ray(collision.gameObject.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray_1, out hit, 1.5f))
            {
                NowFall = true; // 落下中
                PlayerObject = collision.gameObject;
            }
        }
    }
}
