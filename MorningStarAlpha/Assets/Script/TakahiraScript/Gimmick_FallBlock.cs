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

    private bool PlayerMoveFlag = false;
    private bool BulletMoveFlag = false;

    public override void Init()
    {
        // 初期化
        NowFall = false;
        NowTime = 0.0f;
        StartPos = this.gameObject.transform.position;
        PlayerMoveFlag = false;
        BulletMoveFlag = false;

        // コリジョン
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // トリガーオフ

        // リジッドボディ
        Rb.isKinematic = true;  // キネマティックオン
    }

    public override void FixedMove()
    {
        // 床移動
        if (NowFall)
        {
            Vector3 OldPos = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3(StartPos.x, Easing.QuartIn(NowTime, FallTime, StartPos.y, StartPos.y - FallLength), StartPos.z);
            NowTime += Time.fixedDeltaTime;

            // プレイヤー移動
            if (PlayerMoveFlag)
            {
                PlayerMain.instance.transform.position +=
                            new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
                //PlayerMainScript.addVel = new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0) / Time.fixedDeltaTime;
            }

            // 錨移動
            if (BulletMoveFlag)
            {
                if (PlayerMain.instance.BulletScript.isTouched)
                {
                    PlayerMain.instance.BulletScript.transform.position +=
                        new Vector3(0, this.gameObject.transform.position.y - OldPos.y, 0);
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
        PlayerMain.instance.endSwing = true;

        // 自分自身を消す
        Destroy(this.gameObject);
    }

    public override void OnCollisionEnter(Collision collision)
    {
        // プレイヤーか錨と接触
        if (collision.gameObject.CompareTag("Bullet"))
        {
            NowFall = true; // 落下中
            BulletMoveFlag = true;
        }
        
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("プレイヤー当たった");
            // プレイヤーからレイ飛ばして真下にブロックがあったら落下
            if (PlayerMain.instance.getFootHit().collider != null &&
                PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                NowFall = true; // 落下中
                PlayerMoveFlag = true;
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.getFootHit().collider != null &&
                PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                NowFall = true; // 落下中
                PlayerMoveFlag = true;
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMoveFlag = false;
        }
    }
}
