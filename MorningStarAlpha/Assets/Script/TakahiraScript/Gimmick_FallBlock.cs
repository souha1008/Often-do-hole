using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_FallBlock : Gimmick_Main
{
    public float FallPower = 0.05f;     // 落ちる力
    private bool NowFall;               // 落下中か

    public override void Init()
    {
        // 初期化
        NowFall = false;

        // コリジョン
        this.gameObject.GetComponent<Collider>().isTrigger = false;  // トリガーオフ

        // リジッドボディ
        Rb.mass = 100000.0f; // 重くして動かないようにする
    }

    public override void Move()
    {
        if (NowFall)
        {
            Vel.y += -FallPower;
        }
        if (TotalMoveVel.y <= -500.0f)
        {
            Death();
        }
    }

    public override void Death()
    {
        // 自分自身を消す
        Destroy(this.gameObject);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        // プレイヤーか錨と接触
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Bullet")
        {
            NowFall = true; // 落下中
        }
    }
}
