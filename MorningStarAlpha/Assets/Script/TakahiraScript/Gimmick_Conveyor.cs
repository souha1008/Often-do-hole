using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Conveyor : Gimmick_Main
{
    public MOVE_DIRECTION_X MoveDirection_X ;   // 移動方向
    public float MovePower = 30;                // 移動量


    private bool MoveRight; // 移動方向
    private float Fugou;    // 符号
    private GameObject Player;
    private PlayerMain PlayerMainScript;
    private GameObject Bullet;
    private BulletMain BulletMainScript;


    public override void Init()
    {
        // 初期化
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X);
        Fugou = CalculationScript.FugouChange(MoveRight);

        // プレイヤーオブジェクト取得
        Player = GameObject.Find("Player");
        PlayerMainScript = Player.GetComponent<PlayerMain>();

        // 錨オブジェクトnull
        Bullet = null;
        BulletMainScript = null;

        // リジッドボディ
        Rb.isKinematic = true;

        // コリジョン
        Cd.isTrigger = false; // トリガーオフ

        // 角度を0度固定
        Rad = Vector3.zero;
    }

    public override void FixedMove()
    {
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X);
        Fugou = CalculationScript.FugouChange(MoveRight);

        // プレイヤーからレイ飛ばして真下にブロックがあったらプレイヤーの移動
        if (Player != null)
        {
            Ray ray = new Ray(Player.transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1.5f))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    //Debug.Log("プレイヤーブロックの上移動中");
                    PlayerMainScript.addVel = new Vector3(MovePower * -Fugou, 0, 0);
                }
            }
        }

        if (Bullet != null)
        {
            if(BulletMainScript.isTouched == true)
            {
                BulletMainScript.transform.position =
                    BulletMainScript.transform.position + new Vector3(MovePower * Time.fixedDeltaTime * Fugou, 0, 0);
            }
            else
            {
                Bullet = null;
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
            Player = collision.gameObject;
        }
        if (collision.gameObject.tag == "Bullet")
        {
            Bullet = collision.gameObject;
            BulletMainScript = Bullet.GetComponent<BulletMain>();
        }
    }

    private bool MoveDirectionBoolChangeX(MOVE_DIRECTION_X MoveDirection_X)
    {
        if (MoveDirection_X == MOVE_DIRECTION_X.MoveRight)
            return true;
        else
            return false;
    }

    private MOVE_DIRECTION_X BoolMoveDirectionChangeX(bool MoveRight)
    {
        if (MoveRight)
            return MOVE_DIRECTION_X.MoveRight;
        else
            return MOVE_DIRECTION_X.MoveLeft;
    }
}
