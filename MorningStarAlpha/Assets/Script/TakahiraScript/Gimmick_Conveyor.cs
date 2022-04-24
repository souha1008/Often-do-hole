using UnityEngine;

public class Gimmick_Conveyor : Gimmick_Main
{
    [EnumPName("回転方向", typeof(MOVE_DIRECTION_X))]
    public MOVE_DIRECTION_X MoveDirection_X;   // 回転方向
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
        transform.rotation = Quaternion.identity;
    }

    public override void FixedMove()
    {
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X); // 回転方向更新

        // コンベアの動き処理
        conveyorState.Move();
        //Debug.LogWarning(conveyorState); // 現在のコンベアのステート
    }


    public override void Death()
    {

    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.getFootHit().collider != null &&
                 PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                ConveyorState.PlayerMoveFlag = true;
            }
        }

        //if (collision.gameObject.CompareTag("Bullet"))
        //{
        //    ConveyorState.BulletMoveFlag = true;
        //    // 衝突点取得
        //    foreach (ContactPoint contact in collision.contacts)
        //    {
        //        // ヒットした面取得
        //        if (contact.normal.y == 0)
        //        {
        //            if (contact.normal.x < 0) // 右
        //                ConveyorState.TouchSide = TOUCH_SIDE.RIGHT;
        //            else                        // 左
        //                ConveyorState.TouchSide = TOUCH_SIDE.LEFT;
        //        }
        //        else
        //        {
        //            if (contact.normal.y < 0) // 上
        //                ConveyorState.TouchSide = TOUCH_SIDE.UP;
        //            else                        // 下
        //                ConveyorState.TouchSide = TOUCH_SIDE.DOWN;
        //        }

        //        //Debug.Log(contact.normal);
        //        //Debug.Log(conveyorState.TouchSide);

        //        //============
        //        // 自分用メモ
        //        //============
        //        //print(contact.thisCollider.name + " hit " + contact.otherCollider.name); // 自分が　hit 相手に
        //        //Debug.Log(contact.normal); // 法線
        //        //Debug.DrawRay(contact.point, contact.normal, Color.white); // 当たった点でレイを可視化
        //    }
        //}
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerMain.instance.getFootHit().collider != null &&
                 PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                ConveyorState.PlayerMoveFlag = true;
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ConveyorState.PlayerMoveFlag = false;
        }
        //if (collision.gameObject.CompareTag("Bullet"))
        //{
        //    ConveyorState.BulletMoveFlag = false;
        //}
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

    static public TOUCH_SIDE TouchSide;
    static public Gimmick_Conveyor Conveyor;
    static public bool PlayerMoveFlag;
    static public bool BulletMoveFlag;
}

// スタート処理(ここから始める)
public class ConveyorStart : ConveyorState
{
    public ConveyorStart(Gimmick_Conveyor conveyor) // コンストラクタ
    {
        // 初期化
        Conveyor = conveyor;
        TouchSide = TOUCH_SIDE.NONE;
        PlayerMoveFlag = false;
        BulletMoveFlag = false;
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
        TouchSide = TOUCH_SIDE.NONE;
        PlayerMoveFlag = false;
        BulletMoveFlag = false;
        PlayerMain.instance.floorVel = Vector3.zero;
    }

    public override void Move()
    {
        if (PlayerMoveFlag)
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
        if (BulletMoveFlag)
        {
            switch (TouchSide)
            {
                case TOUCH_SIDE.NONE:
                    break;
                case TOUCH_SIDE.UP:
                    //if (Conveyor.MoveRight) // 右回転なら右移動
                    //{
                    //    StateChange(new ConveyorRight());
                    //}
                    //else // 左回転なら左移動
                    //{
                    //    StateChange(new ConveyorLeft());
                    //}
                    PlayerMain.instance.ForciblyReleaseMode(true);
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
                    //if (Conveyor.MoveRight)
                    //{
                    //    StateChange(new ConveyorDown());
                    //}
                    //else
                    //{
                    //    StateChange(new ConveyorUp());
                    //}
                    PlayerMain.instance.ForciblyReleaseMode(true);
                    break;
                case TOUCH_SIDE.LEFT:
                    //if (Conveyor.MoveRight)
                    //{
                    //    StateChange(new ConveyorUp());
                    //}
                    //else
                    //{
                    //    StateChange(new ConveyorDown());
                    //}
                    PlayerMain.instance.ForciblyReleaseMode(true);
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
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            PlayerMain.instance.BulletScript.transform.position += new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0);
            PlayerMain.instance.floorVel = new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0) * 1 / Time.fixedDeltaTime;

            if (!BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime, 0);
                if (Conveyor.MoveRight)
                    StateChange(new ConveyorRight());
                else
                    StateChange(new ConveyorLeft());
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
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            PlayerMain.instance.BulletScript.transform.position += new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
            PlayerMain.instance.floorVel = new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0) * 1 / Time.fixedDeltaTime;

            if (!BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(0, Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
                if (Conveyor.MoveRight)
                    StateChange(new ConveyorLeft());
                else
                    StateChange(new ConveyorRight());
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
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            //Debug.Log("錨右移動中");
            PlayerMain.instance.BulletScript.transform.position += new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
            PlayerMain.instance.floorVel = new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0) * 1 / Time.fixedDeltaTime;

            if (PlayerMain.instance.BulletScript.transform.position.x > Conveyor.transform.position.x + Conveyor.transform.lossyScale.x * 0.5f)
                BulletMoveFlag = false;

            if (!BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
                //if (Conveyor.MoveRight)
                //    StateChange(new ConveyorDown());
                //else
                //    StateChange(new ConveyorUp());
                StateChange(new ConveyorNone());
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
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            //Debug.LogWarning("錨左移動中");
            PlayerMain.instance.BulletScript.transform.position += new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
            PlayerMain.instance.floorVel = new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0) * 1 / Time.fixedDeltaTime;

            if (PlayerMain.instance.BulletScript.transform.position.x < Conveyor.transform.position.x - Conveyor.transform.lossyScale.x * 0.5f)
                BulletMoveFlag = false;

            if (!BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
                //if (Conveyor.MoveRight)
                //    StateChange(new ConveyorUp());
                //else
                //    StateChange(new ConveyorDown());
                StateChange(new ConveyorNone());
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
        if(PlayerMoveFlag)
        {
            PlayerMain.instance.floorVel = new Vector3(Conveyor.MovePower, 0, 0);
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
        if (PlayerMoveFlag)
        {
            PlayerMain.instance.floorVel = new Vector3(Conveyor.MovePower * -1, 0, 0);
        }
        else
        {
            StateChange(new ConveyorNone());
        }
    }
}
