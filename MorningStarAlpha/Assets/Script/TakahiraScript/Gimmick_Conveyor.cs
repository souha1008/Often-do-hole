using UnityEngine;

public class Gimmick_Conveyor : Gimmick_Main
{
    [EnumPName("回転方向", typeof(MOVE_DIRECTION_X))]
    public MOVE_DIRECTION_X MoveDirection_X;   // 回転方向
    [Label("移動量")]
    public float MovePower = 30;                // 移動量
    [Label("縦ならチェック")]
    public bool MoveTateFlag = false;                // コンベア縦移動フラグ


    [HideInInspector] public ConveyorStateMain conveyorStateMain;
    [HideInInspector] public bool MoveRight;    // 回転方向


    public override void Init()
    {
        // コンベアステート初期化
        conveyorStateMain = new ConveyorStateMain(this);

        // 回転方向更新
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X);

        // リジッドボディ
        Rb.isKinematic = true;

        // コリジョン
        Cd.isTrigger = false; // トリガーオフ

        // 角度を0度固定
        transform.rotation = Quaternion.identity;

        // コンベア縦なら
        if (MoveTateFlag)
            this.gameObject.tag = "Conveyor_Tate";
        else
            this.gameObject.tag = "Conveyor_Yoko";
    }

    public override void FixedMove()
    {
        MoveRight = MoveDirectionBoolChangeX(MoveDirection_X); // 回転方向更新

        // コンベアの動き処理
        conveyorStateMain.Move();
        //Debug.LogWarning(conveyorStateMain.ConveyorState); // 現在のコンベアのステート
    }


    public override void Death()
    {

    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!MoveTateFlag &&
                PlayerMain.instance.getFootHit().collider != null &&
                 PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                conveyorStateMain.PlayerMoveFlag = true;
            }
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            conveyorStateMain.BulletMoveFlag = true;
            // 衝突点取得
            foreach (ContactPoint contact in collision.contacts)
            {
                // ヒットした面取得
                if (contact.normal.y == 0)
                {
                    if (contact.normal.x < 0) // 右
                        conveyorStateMain.TouchSide = TOUCH_SIDE.RIGHT;
                    else                        // 左
                        conveyorStateMain.TouchSide = TOUCH_SIDE.LEFT;
                }
                else
                {
                    if (contact.normal.y < 0) // 上
                        conveyorStateMain.TouchSide = TOUCH_SIDE.UP;
                    else                        // 下
                        conveyorStateMain.TouchSide = TOUCH_SIDE.DOWN;
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
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!MoveTateFlag &&
                PlayerMain.instance.getFootHit().collider != null &&
                 PlayerMain.instance.getFootHit().collider.gameObject == this.gameObject)
            {
                conveyorStateMain.PlayerMoveFlag = true;
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            conveyorStateMain.PlayerMoveFlag = false;
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

public class ConveyorStateMain
{
    public ConveyorStateMain(Gimmick_Conveyor conveyor)
    {
        Conveyor = conveyor;
        ConveyorState = new ConveyorNone(this);
        TouchSide = TOUCH_SIDE.NONE;
        PlayerMoveFlag = false;
        BulletMoveFlag = false;
    }

    // コンベアの動きとステート移行判定
    public void Move() 
    {
        ConveyorState.Move();
    }

    // ステート移行
    public void StateChange(ConveyorState State)
    {
        ConveyorState = State;
    }

    public Gimmick_Conveyor Conveyor;
    public ConveyorState ConveyorState;
    public TOUCH_SIDE TouchSide;
    public bool PlayerMoveFlag;
    public bool BulletMoveFlag;
}

// コンベアのステート
public abstract class ConveyorState
{
    public ConveyorState(ConveyorStateMain conveyorMain)
    {
        ConveyorMain = conveyorMain;
    }
    public virtual void Move() { } // コンベアの動きとステート移行判定

    public ConveyorStateMain ConveyorMain;
}

// 何もしない
public class ConveyorNone : ConveyorState
{
    public ConveyorNone(ConveyorStateMain conveyorMain) : base (conveyorMain)
    {
        ConveyorMain.TouchSide = TOUCH_SIDE.NONE;
        ConveyorMain.PlayerMoveFlag = false;
        ConveyorMain.BulletMoveFlag = false;
        PlayerMain.instance.floorVel = Vector3.zero;
    }
    public override void Move()
    {
        if (ConveyorMain.PlayerMoveFlag)
        {
            if (!ConveyorMain.Conveyor.MoveTateFlag)
            {
                if (ConveyorMain.Conveyor.MoveRight)
                {
                    ConveyorMain.StateChange(new ConveyorPlayerMoveRight(ConveyorMain)); // プレイヤー右移動
                }
                else
                {
                    ConveyorMain.StateChange(new ConveyorPlayerMoveLeft(ConveyorMain)); // プレイヤー左移動
                }
            }
            else
                ConveyorMain.PlayerMoveFlag = false;
        }
        if (ConveyorMain.BulletMoveFlag)
        {
            switch (ConveyorMain.TouchSide)
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
                    //PlayerMain.instance.ForciblyReleaseMode(true);
                    ConveyorMain.BulletMoveFlag = false;
                    break;
                case TOUCH_SIDE.DOWN:
                    // 縦移動でないなら
                    if (!ConveyorMain.Conveyor.MoveTateFlag)
                    {
                        if (ConveyorMain.Conveyor.MoveRight) // 右回転なら左移動
                        {
                            ConveyorMain.StateChange(new ConveyorLeft(ConveyorMain));
                        }
                        else // 左回転なら右移動
                        {
                            ConveyorMain.StateChange(new ConveyorRight(ConveyorMain));
                        }
                    }
                    else
                        ConveyorMain.BulletMoveFlag = false;
                    break;
                case TOUCH_SIDE.RIGHT:
                    // 縦移動なら
                    if (ConveyorMain.Conveyor.MoveTateFlag)
                    {
                        if (ConveyorMain.Conveyor.MoveRight)
                        {
                            ConveyorMain.StateChange(new ConveyorDown(ConveyorMain));
                        }
                        else
                        {
                            ConveyorMain.StateChange(new ConveyorUp(ConveyorMain));
                        }
                    }
                    else
                        ConveyorMain.BulletMoveFlag = false;
                    break;
                case TOUCH_SIDE.LEFT:
                    // 縦移動なら
                    if (ConveyorMain.Conveyor.MoveTateFlag)
                    {
                        if (ConveyorMain.Conveyor.MoveRight)
                        {
                            ConveyorMain.StateChange(new ConveyorUp(ConveyorMain));
                        }
                        else
                        {
                            ConveyorMain.StateChange(new ConveyorDown(ConveyorMain));
                        }
                    }
                    else
                        ConveyorMain.BulletMoveFlag = false;
                    break;
            }
        }
    }
}

// 錨オブジェクト上方向
public class ConveyorUp : ConveyorState
{
    public ConveyorUp(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            PlayerMain.instance.BulletScript.transform.position += new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0);
            PlayerMain.instance.floorVel = new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0) * 1 / Time.fixedDeltaTime;

            if (PlayerMain.instance.BulletScript.transform.position.y > ConveyorMain.Conveyor.transform.position.y + ConveyorMain.Conveyor.transform.lossyScale.y * 0.5f)
                ConveyorMain.BulletMoveFlag = false;

            if (!ConveyorMain.BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0);
                //if (ConveyorMain.Conveyor.MoveRight)
                //    ConveyorMain.StateChange(new ConveyorRight(ConveyorMain));
                //else
                //    ConveyorMain.StateChange(new ConveyorLeft(ConveyorMain));
                PlayerMain.instance.ForciblyReleaseMode(true);
                ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
            }
        }
        else
        {
            PlayerMain.instance.ForciblyReleaseMode(true);
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
        }
    }
}

// 錨オブジェクト下方向
public class ConveyorDown : ConveyorState
{
    public ConveyorDown(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            PlayerMain.instance.BulletScript.transform.position += new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
            PlayerMain.instance.floorVel = new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0) * 1 / Time.fixedDeltaTime;

            if (PlayerMain.instance.BulletScript.transform.position.y < ConveyorMain.Conveyor.transform.position.y - ConveyorMain.Conveyor.transform.lossyScale.y * 0.5f)
                ConveyorMain.BulletMoveFlag = false;

            if (!ConveyorMain.BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(0, ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0);
                //if (ConveyorMain.Conveyor.MoveRight)
                //    ConveyorMain.StateChange(new ConveyorLeft(ConveyorMain));
                //else
                //    ConveyorMain.StateChange(new ConveyorRight(ConveyorMain));
                PlayerMain.instance.ForciblyReleaseMode(true);
                ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
            }
        }
        else
        {
            PlayerMain.instance.ForciblyReleaseMode(true);
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
        }
    }
}

// 錨オブジェクト右方向
public class ConveyorRight : ConveyorState
{
    public ConveyorRight(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            //Debug.Log("錨右移動中");
            PlayerMain.instance.BulletScript.transform.position += new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
            PlayerMain.instance.floorVel = new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0, 0) * 1 / Time.fixedDeltaTime;

            if (PlayerMain.instance.BulletScript.transform.position.x > ConveyorMain.Conveyor.transform.position.x + ConveyorMain.Conveyor.transform.lossyScale.x * 0.5f)
                ConveyorMain.BulletMoveFlag = false;

            if (!ConveyorMain.BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime, 0, 0);
                //if (Conveyor.MoveRight)
                //    StateChange(new ConveyorDown());
                //else
                //    StateChange(new ConveyorUp());
                PlayerMain.instance.ForciblyReleaseMode(true);
                ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
            }
        }
        else
        {
            PlayerMain.instance.ForciblyReleaseMode(true);
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
        }
    }
}

// 錨オブジェクト左方向
public class ConveyorLeft : ConveyorState
{
    public ConveyorLeft(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if (PlayerMain.instance.BulletScript.isTouched)
        {
            //Debug.LogWarning("錨左移動中");
            PlayerMain.instance.BulletScript.transform.position += new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
            PlayerMain.instance.floorVel = new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0) * 1 / Time.fixedDeltaTime;

            if (PlayerMain.instance.BulletScript.transform.position.x < ConveyorMain.Conveyor.transform.position.x - ConveyorMain.Conveyor.transform.lossyScale.x * 0.5f)
                ConveyorMain.BulletMoveFlag = false;

            if (!ConveyorMain.BulletMoveFlag)
            {
                PlayerMain.instance.BulletScript.transform.position -= new Vector3(ConveyorMain.Conveyor.MovePower * Time.fixedDeltaTime * -1, 0, 0);
                //if (Conveyor.MoveRight)
                //    StateChange(new ConveyorUp());
                //else
                //    StateChange(new ConveyorDown());
                PlayerMain.instance.ForciblyReleaseMode(true);
                ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
            }
        }
        else
        {
            PlayerMain.instance.ForciblyReleaseMode(true);
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
        }
    }
}

// プレイヤーオブジェクト右移動
public class ConveyorPlayerMoveRight : ConveyorState
{
    public ConveyorPlayerMoveRight(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if(ConveyorMain.PlayerMoveFlag)
        {
            PlayerMain.instance.floorVel = new Vector3(ConveyorMain.Conveyor.MovePower, 0, 0);
        }
        else
        {
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
        }
    }
}


// プレイヤーオブジェクト左移動
public class ConveyorPlayerMoveLeft : ConveyorState
{
    public ConveyorPlayerMoveLeft(ConveyorStateMain conveyorMain) : base(conveyorMain) { }
    public override void Move()
    {
        if (ConveyorMain.PlayerMoveFlag)
        {
            PlayerMain.instance.floorVel = new Vector3(ConveyorMain.Conveyor.MovePower * -1, 0, 0);
        }
        else
        {
            ConveyorMain.StateChange(new ConveyorNone(ConveyorMain));
        }
    }
}
