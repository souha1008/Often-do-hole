using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_FallBlock : Gimmick_Main
{
    public float FallPower = 0.1f;     // 落ちる力
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
        // ※後でイージングを利用した処理に変更予定
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
        if (collision.gameObject.tag == "Bullet")
        {
            NowFall = true; // 落下中
        }
        else if (collision.gameObject.tag == "Player")
        {
            // プレイヤーからレイ飛ばして真下にブロックがあったら落下
            Ray ray_1 = new Ray(collision.gameObject.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray_1, out hit, 1.5f))
            {
                NowFall = true; // 落下中
            }
        }
    }

    // プレイヤーの落下でぽむぽむするの調整用
    //public void OnCollisionStay(Collision collision)
    //{
    //    // プレイヤーと接触中
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        // プレイヤーの右と左にレイを上下2本ずつ飛ばす
    //        Ray ray_1 = new Ray(collision.gameObject.transform.position + new Vector3(0, 0.5f, 0), Vector3.left);
    //        Ray ray_2 = new Ray(collision.gameObject.transform.position + new Vector3(0, -0.5f, 0), Vector3.left);
    //        Ray ray_3 = new Ray(collision.gameObject.transform.position + new Vector3(0, 0.5f, 0), Vector3.right);
    //        Ray ray_4 = new Ray(collision.gameObject.transform.position + new Vector3(0, -0.5f, 0), Vector3.right);

    //        RaycastHit hit;
    //        if (Physics.Raycast(ray_1, out hit, 1.5f) || Physics.Raycast(ray_2, out hit, 1.5f) ||
    //            Physics.Raycast(ray_3, out hit, 1.5f) || Physics.Raycast(ray_4, out hit, 1.5f))
    //        {
    //            return;
    //        }
    //        // プレイヤーの速度も同じにする
    //        collision.gameObject.GetComponent<PlayerMain>().vel.y = Vel.y;
    //    }
    //}
}
