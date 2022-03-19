using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_MoveBlock : Gimmick_Main
{
    // 変数
    [Header("移動距離")]
    public float MoveLength = 15.0f;    // 動く距離

    [Header("何秒かけて移動するか")]
    public float MoveTime = 3.0f;       // 何秒かけて移動するか

    [Header("移動方向(右)")]
    public bool MoveRight = true;       // 移動方向(true:右、false:左)

    private bool NowMove;               // 移動中か
    private float NowTime;              // 経過時間
    private Vector3 StartPos;           // 初期座標
    private GameObject PlayerObject;    // プレイヤーオブジェクト
    private float Fugou;

    public override void Init()
    {
        // 初期化
        NowMove = true;
        NowTime = 0.0f;
        StartPos = this.gameObject.transform.position;
        Fugou = CalculationScript.FugouChange(MoveRight);

        // コリジョン
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // トリガーオフ

        // リジッドボディ
        Rb.isKinematic = true;
    }

    public override void UpdateMove()
    {
        Vector3 OldPos = this.gameObject.transform.position;
        if (NowMove)
        {
            this.gameObject.transform.position = new Vector3(Easing.QuadInOut(NowTime, MoveTime, StartPos.x, StartPos.x + MoveLength * Fugou), StartPos.y, StartPos.z);
            NowTime += Time.deltaTime;

            // 移動終了
            if (NowTime > MoveTime)
                NowMove = false;
        }
        else
        {
            StartPos = this.gameObject.transform.position;
            NowTime = 0.0f;
            MoveRight = CalculationScript.TureFalseChange(MoveRight);   // 向き反転
            Fugou = CalculationScript.FugouChange(MoveRight);   // 符号反転
            NowMove = true;
        }

        // プレイヤーからレイ飛ばして真下にブロックがあったら
        if (PlayerObject != null)
        {
            Ray ray = new Ray(PlayerObject.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1.5f))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    //Debug.Log("プレイヤーブロックの上移動中");
                    PlayerObject.gameObject.transform.position =
                        PlayerObject.gameObject.transform.position + new Vector3(this.gameObject.transform.position.x - OldPos.x, 0, 0);
                }
            }
        }
    }

    public override void Death()
    {

    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerObject = collision.gameObject;
        }
    }
}
