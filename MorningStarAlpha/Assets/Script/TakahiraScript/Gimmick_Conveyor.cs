using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_Conveyor : Gimmick_Main
{
    [EnumPName("回転方向", typeof(MOVE_DIRECTION_X))]
    public MOVE_DIRECTION_X MoveDirection_X ;   // 回転方向
    [Label("移動量")]
    public float MovePower = 30;                // 移動量


    [HideInInspector] public ConveyorState conveyorState;
    [HideInInspector] public bool MoveRight;    // 回転方向


    public override void Init()
    {
        // コンベアステート初期化
        conveyorState = new ConveyorStart(this);

        // 回転方向更新
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X);

        // リジッドボディ
        Rb.isKinematic = true;

        // コリジョン
        Cd.isTrigger = false; // トリガーオフ

        // 角度を0度固定
        Rad = Vector3.zero;
    }

    public override void FixedMove()
    {
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X); // 回転方向更新

        // コンベアの動き処理
        conveyorState.Move();
        //Debug.Log(conveyorState); // 現在のコンベアのステート
    }


    public override void Death()
    {
        
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // プレイヤーオブジェクト取得
            ConveyorState.Player = collision.gameObject;
            ConveyorState.PlayerMainScript = collision.gameObject.GetComponent<PlayerMain>();
        }
        if (collision.gameObject.tag == "Bullet")
        {
            // 衝突点取得
            foreach (ContactPoint contact in collision.contacts)
            {
                // ヒットした面取得
                if (contact.normal.y == 0)
                {
                    if (contact.normal.x < 0) // 右
                        conveyorState.TouchSide = TOUCH_SIDE.RIGHT;
                    else                        // 左
                        conveyorState.TouchSide = TOUCH_SIDE.LEFT;
                }
                else
                {
                    if (contact.normal.y < 0) // 上
                        conveyorState.TouchSide = TOUCH_SIDE.UP;
                    else                        // 下
                        conveyorState.TouchSide = TOUCH_SIDE.DOWN;
                }

                //Debug.Log(contact.normal);
                //Debug.Log(conveyorState.TouchSide);

                //============
                // 自分用メモ
                //============
                //print(contact.thisCollider.name + " hit " + contact.otherCollider.name); // 自分が　hit 相手に
                //Debug.Log(contact.normal); // 法線
                //Debug.DrawRay(contact.point, contact.normal, Color.white); // 当たった点でレイを可視化
            }

            // 錨オブジェクト取得
            ConveyorState.Bullet = collision.gameObject;
            ConveyorState.BulletMainScript = collision.gameObject.GetComponent<BulletMain>();
        }
    }

    public bool MoveDirectionBoolChangeX(MOVE_DIRECTION_X MoveDirection_X)
    {
        if (MoveDirection_X == MOVE_DIRECTION_X.MoveRight)
            return true;
        else
            return false;
    }

    public MOVE_DIRECTION_X BoolMoveDirectionChangeX(bool MoveRight)
    {
        if (MoveRight)
            return MOVE_DIRECTION_X.MoveRight;
        else
            return MOVE_DIRECTION_X.MoveLeft;
    }
}

public enum TOUCH_SIDE
{
    NONE = 0,
    RIGHT,
    LEFT,
    UP,
    DOWN
}

// コンベアのステート
public abstract class ConveyorState
{
    public virtual void Move() { } // コンベアの動きとステート移行判定
    public void StateChange(ConveyorState state) // ステート移行
    {
        Conveyor.conveyorState = state;
    }

    public TOUCH_SIDE TouchSide;
    static public Gimmick_Conveyor Conveyor;
    static public GameObject Player;
    static public PlayerMain PlayerMainScript;
    static public GameObject Bullet;
    static public BulletMain BulletMainScript;
}

// スタート処理(ここから始める)
public class ConveyorStart : ConveyorState
{
    public ConveyorStart(Gimmick_Conveyor conveyor) // コンストラクタ
    {
        // 初期化
        Conveyor = conveyor;
        TouchSide = TOUCH_SIDE.NONE;

        // プレイヤーオブジェクトnull
        Player = null;
        PlayerMainScript = null;

        // 錨オブジェクトnull
        Bullet = null;
        BulletMainScript = null;
    }

    public override void Move()
    {
        StateChange(new ConveyorNone());
    }
}

// 何もしない
public class ConveyorNone : ConveyorState
{
    public ConveyorNone() // コンストラクタ
    {
        if (Bullet != null && BulletMainScript != null)
        {
            BulletMainScript.isTouched = false;
        }

        TouchSide = TOUCH_SIDE.NONE;
    }

    public override void Move()
    {
        if (Player != null && PlayerMainScript != null)
        {
            // プレイヤーからレイ飛ばして真下にブロックがあったらプレイヤーオブジェクト取得
            Ray ray_1 = new Ray(Player.gameObject.transform.position + new Vector3(Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
            Ray ray_2 = new Ray(Player.gameObject.transform.position + new Vector3(-Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
            RaycastHit hit;
            float RayLength = Player.gameObject.transform.localScale.y * 0.5f + 1.0f;
            if (Physics.Raycast(ray_1, out hit, RayLength) || Physics.Raycast(ray_2, out hit, RayLength))
            {
                if (hit.collider.gameObject == Conveyor.gameObject)
                {
                    if (Conveyor.MoveRight)
                    {
                        StateChange(new ConveyorPlayerMoveRight()); // プレイヤー右移動
                    }
                    else
                    {
                        StateChange(new ConveyorPlayerMoveLeft()); // プレイヤー左移動
                    }
                }
            }
            else // レイが当たらなかったらnull入れる
            {
                Player = null;
                PlayerMainScript = null;
            }
        }

        if (Bullet != null && BulletMainScript != null)
        {
            switch (TouchSide)
            {
                case TOUCH_SIDE.NONE:
                    break;
                case TOUCH_SIDE.UP:
                    if (Conveyor.MoveRight) // 右回転なら右移動
                    {
                        StateChange(new ConveyorRight());
                    }
                    else // 左回転なら左移動
                    {
                        StateChange(new ConveyorLeft());
                    }
                    break;
                case TOUCH_SIDE.DOWN:
                    if (Conveyor.MoveRight) // 右回転なら左移動
                    {
                        StateChange(new ConveyorLeft());
                    }
                    else // 左回転なら右移動
                    {
                        StateChange(new ConveyorRight());
                    }
                    break;
                case TOUCH_SIDE.RIGHT:
                    if (Conveyor.MoveRight)
                    {
                        StateChange(new ConveyorDown());
                    }
                    else
                    {
                        StateChange(new ConveyorUp());
                    }
                    break;
                case TOUCH_SIDE.LEFT:
                    if (Conveyor.MoveRight)
                    {
                        StateChange(new ConveyorUp());
                    }
                    else
                    {
                        StateChange(new ConveyorDown());
                    }
                    break;
            }
        }
    }
}

// 錨オブジェクト上方向
public class ConveyorUp : ConveyorState
{
    public override void Move()
    {
        if (BulletMainScript.isTouched == true)
        {
            BulletMainScript.transform.position += new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0);

            // 錨からレイ飛ばして真横にブロックが無かったらステート変更
            if (Conveyor.MoveRight)
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(0, -Bullet.transform.localScale.y * 0.5f, 0), Vector3.right);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(0, Bullet.transform.localScale.y * 0.5f, 0), Vector3.right);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.x * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0);
                    StateChange(new ConveyorRight());
                }
            }
            else
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(0, -Bullet.transform.localScale.y * 0.5f, 0), Vector3.left);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(0, Bullet.transform.localScale.y * 0.5f, 0), Vector3.left);
                float RayLength = BulletMainScript.gameObject.transform.localScale.x * 0.5f + 1.0f;
                RaycastHit hit;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0);
                    StateChange(new ConveyorLeft());
                }
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}

// 錨オブジェクト下方向
public class ConveyorDown : ConveyorState
{
    public override void Move()
    {
        if (BulletMainScript.isTouched == true)
        {
            BulletMainScript.transform.position += new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);

            // 錨からレイ飛ばして真横にブロックが無かったらステート変更
            if (Conveyor.MoveRight)
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(0, -Bullet.transform.localScale.y * 0.5f, 0), Vector3.left);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(0, Bullet.transform.localScale.y * 0.5f, 0), Vector3.left);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.x * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
                    StateChange(new ConveyorLeft());
                }
            }
            else
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(0, -Bullet.transform.localScale.y * 0.5f, 0), Vector3.right);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(0, Bullet.transform.localScale.y * 0.5f, 0), Vector3.right);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.x * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
                    StateChange(new ConveyorRight());
                }
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}

// 錨オブジェクト右方向
public class ConveyorRight : ConveyorState
{
    public override void Move()
    {
        if (BulletMainScript.isTouched == true)
        {
            //Debug.Log("錨右移動中");
            BulletMainScript.transform.position += new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);

            // 錨からレイ飛ばして真上にブロックが無かったらステート変更
            if (Conveyor.MoveRight)
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(-Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.y * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
                    StateChange(new ConveyorDown());
                }
            }
            else
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(-Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.up);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.up);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.y * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
                    StateChange(new ConveyorUp());
                }
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}

// 錨オブジェクト左方向
public class ConveyorLeft : ConveyorState
{
    public override void Move()
    {
        if(BulletMainScript.isTouched == true)
        {
            //Debug.Log("錨左移動中");
            BulletMainScript.transform.position += new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);

            // 錨からレイ飛ばして真上にブロックが無かったらステート変更
            if (Conveyor.MoveRight)
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(-Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.up);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.up);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.y * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {              
                    BulletMainScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
                    StateChange(new ConveyorUp());
                }
            }
            else
            {
                Ray ray1 = new Ray(BulletMainScript.transform.position + new Vector3(-Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
                Ray ray2 = new Ray(BulletMainScript.transform.position + new Vector3(Bullet.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
                RaycastHit hit;
                float RayLength = BulletMainScript.gameObject.transform.localScale.y * 0.5f + 1.0f;
                if (!Physics.Raycast(ray1, out hit, RayLength) && !Physics.Raycast(ray2, out hit, RayLength))
                {
                    BulletMainScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
                    StateChange(new ConveyorDown());
                }
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}

// プレイヤーオブジェクト右移動
public class ConveyorPlayerMoveRight : ConveyorState
{
    public override void Move()
    {
        // プレイヤーからレイ飛ばして真下にブロックが無かったらステート変更
        Ray ray_1 = new Ray(Player.gameObject.transform.position + new Vector3(Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
        Ray ray_2 = new Ray(Player.gameObject.transform.position + new Vector3(-Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
        RaycastHit hit;
        float RayLength = Player.gameObject.transform.localScale.y * 0.5f + 1.0f;
        if (Physics.Raycast(ray_1, out hit, RayLength) || Physics.Raycast(ray_2, out hit, RayLength))
        {
            if (hit.collider.gameObject == Conveyor.gameObject)
            {
                //Debug.Log("プレイヤー右に移動中");
                PlayerMainScript.addVel = new Vector3(Conveyor.MovePower, 0, 0);
            }
            else
            {
                StateChange(new ConveyorNone());
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}


// プレイヤーオブジェクト左移動
public class ConveyorPlayerMoveLeft : ConveyorState
{
    public override void Move()
    {
        // プレイヤーからレイ飛ばして真下にブロックが無かったらステート変更
        Ray ray_1 = new Ray(Player.gameObject.transform.position + new Vector3(Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
        Ray ray_2 = new Ray(Player.gameObject.transform.position + new Vector3(-Player.gameObject.transform.localScale.x * 0.5f, 0, 0), Vector3.down);
        RaycastHit hit;
        float RayLength = Player.gameObject.transform.localScale.y * 0.5f + 1.0f;
        if (Physics.Raycast(ray_1, out hit, RayLength) || Physics.Raycast(ray_2, out hit, RayLength))
        {
            if (hit.collider.gameObject == Conveyor.gameObject)
            {
                //Debug.Log("プレイヤー左に移動中");
                PlayerMainScript.addVel = new Vector3(Conveyor.MovePower * -1, 0, 0);
            }
            else
            {
                StateChange(new ConveyorNone());
            }
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}
