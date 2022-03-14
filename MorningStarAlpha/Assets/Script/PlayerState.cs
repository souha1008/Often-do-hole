using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// プレイヤーの状態管理をするインターフェース
/// MonoBehaviourは継承しない
/// </summary>
/// 

//うんち！
//ちんちん！
//やったーーーーーー！！
//少し湿っている


public class PlayerState
{
    virtual public void UpdateState() { }      //継承先でコントローラーの入力
    virtual public void Move() { }             //継承先で物理挙動（rigidbodyを使用したもの）overrideする
    virtual public void StateTransition() { }  //継承先でシーンの移動を決める
    virtual public void DebugMessage() { }     //デバッグ用のメッセージ表示

    static public GameObject Player;
    static public PlayerMain PlayerScript;
}


/// <summary>
/// プレイヤーが立ち止まっている状態
/// スティック入力での移動、弾の発射ができる
/// </summary>
public class PlayerStateStand : PlayerState
{  
    private bool shotButton;
    public PlayerStateStand()//コンストラクタ
    {
        shotButton = false;
    }

    public override void UpdateState() 
    {
        if (Input.GetButtonDown("Button_R"))
        {
            if (PlayerScript.canShot)
            {
                shotButton = true;
            }
        }

        if (PlayerScript.leftStick.x > 0.2f)
        {
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (PlayerScript.leftStick.x < -0.2f)
        {
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public override void Move()
    {
        //減衰
        if (PlayerScript.vel.x > PlayerScript.MIN_RUN_SPEED)
        {
            PlayerScript.vel *= PlayerScript.RUN_FRICTION;
            if(PlayerScript.vel.x < PlayerScript.MIN_RUN_SPEED)
            {
                PlayerScript.vel.x = 0;
            }
        }
        if (PlayerScript.vel.x < PlayerScript.MIN_RUN_SPEED * -1)
        {
            PlayerScript.vel *= PlayerScript.RUN_FRICTION;
            if (PlayerScript.vel.x > PlayerScript.MIN_RUN_SPEED * -1)
            {
                PlayerScript.vel.x = 0;
            }
        }

        PlayerScript.rb.AddForce(Vector3.down * 20.8f, ForceMode.Acceleration);
        PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.rb.velocity.y, 0);
    }


    public override void StateTransition()
    {
        //右移動に移行
        if (PlayerScript.leftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.mode = new PlayerStateMove();
        }

        //左移動に移行
        if (PlayerScript.leftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1)
        {
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.mode = new PlayerStateMove();
        }

        if (shotButton)
        {
            PlayerScript.mode = new PlayerStateShot();
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Stand");
    }
}

/// <summary>
/// プレイヤーが移動している状態
/// スティック入力での移動、弾の発射ができる
/// </summary>
public class PlayerStateMove : PlayerState
{
    bool shotButton;

    public PlayerStateMove()//コンストラクタ
    {
        shotButton = false;

        //プレイヤーの回転
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public override void UpdateState()
    {
        if (Input.GetButtonDown("Button_R"))
        {
            if (PlayerScript.canShot)
            {
                shotButton = true;
            }
        }
    }

    public override void Move()
    {
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED;
            PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);           
        }
        if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * -1;
            PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
        }

        PlayerScript.rb.AddForce(Vector3.down * 20.8f, ForceMode.Acceleration);
        PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.rb.velocity.y, 0);
    }

    public override void StateTransition()
    {
        //スティックがしきい値以下ならstand状態に移行
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            if (PlayerScript.leftStick.x < (PlayerScript.LATERAL_MOVE_THRESHORD) - 0.1)
            {
                PlayerScript.mode = new PlayerStateStand();
            }
        }
        if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            if (PlayerScript.leftStick.x > (PlayerScript.LATERAL_MOVE_THRESHORD * -1) + 0.1)
            {
                PlayerScript.mode = new PlayerStateStand();
            }
        }

        if (shotButton)
        {
            PlayerScript.mode = new PlayerStateShot();
        }
    }


    public override void DebugMessage()
    {
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            Debug.Log("PlayerState:RunRight");
        }
        if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            Debug.Log("PlayerState:RunLeft");
        }
    }
}


/// <summary>
/// 弾を撃った状態
/// 弾はオブジェクトには接触していない
/// スティックでの移動不可、弾を引き戻すことのみ可能
/// </summary>
public class PlayerStateShot : PlayerState
{
    private enum SHOT_STATE{
        GO,         //引っ張られずに飛んでいる
        STRAINED,　 //プレイヤーを引っ張りながら飛んでいる
        RETURN,     //プレイヤーに戻っている
    }

    SHOT_STATE shotState;               //紐が張り詰めているか（張り詰めていたら弾についていく）
    float countTime;               //発射からの時間
    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //発射からの弾のvectorを保存する
    bool finishFlag;
    BulletMain BulletScript;
    public PlayerStateShot()//コンストラクタ
    {    
        //弾の生成と発射
        //発射時にぶつからないように発射位置を矢印方向にずらす
        Vector3 vec = PlayerScript.leftStick.normalized;
        Vector3 popPos = PlayerScript.transform.position + (vec * 5);
        PlayerScript.Bullet = Object.Instantiate(PlayerScript.BulletPrefab, popPos, Quaternion.identity);
        BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //バレット情報のスナップ
    }

    public override void UpdateState()
    {
        countTime += Time.deltaTime;

        if (countTime > 0.3)
        {
            if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
            {
                shotState = SHOT_STATE.RETURN;                                          
            }
        }
    }

    public override void Move()
    {
      
        float interval;

        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
        switch (shotState) {

            case SHOT_STATE.GO:
               
                bulletVecs.Enqueue(BulletScript.rb.velocity);

                //紐の長さを超えたら引っ張られている状態にする
                if (interval > BulletScript.BULLET_ROPE_LENGTH)
                {
                    shotState = SHOT_STATE.STRAINED;
                }

                PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.vel.y, 0);
                break;

            case SHOT_STATE.STRAINED:
                //弾の進行方向の逆方向に位置補正
                bulletVecs.Enqueue(BulletScript.rb.velocity);
                PlayerScript.vel = bulletVecs.Dequeue();
                PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.vel.y, 0);

                //調整
                //if (interval > BulletScript.BULLET_ROPE_LENGTH)
                //{
                //    Vector3 ballVel = BulletScript.rb.velocity;
                //    Vector3 reverceVel = (ballVel * -1).normalized;

                //    PlayerScript.transform.position = BulletScript.transform.position + reverceVel * BulletScript.BULLET_ROPE_LENGTH;
                //}
                break;

            case SHOT_STATE.RETURN:
                //自分へ弾を引き戻す
                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
                vec = vec.normalized;

                BulletScript.rb.velocity = vec * 30;

                //距離が一定以下になったら弾を非アクティブ

                if (interval < 4.0f)
                {
                    finishFlag = true;
                    Object.Destroy(BulletScript.gameObject);
                }

                PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.vel.y, 0);
                break;
        }

        
    }

    public override void StateTransition()
    {
        if (finishFlag)
        {
            //着地したら立っている状態に移行
            Ray downRay = new Ray(PlayerScript.rb.position, Vector3.down);
            if (Physics.Raycast(downRay, 2.0f))
            {
                PlayerScript.mode = new PlayerStateStand();
            }
            else //そうでないなら空中
            {
                PlayerScript.mode = new PlayerStateMidair();
            }
        } 
        
        //BulletMain BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>();

        //ボールが触れたらスイング状態
        if (BulletScript.isTouched)
        {
            PlayerScript.mode = new PlayerStateSwing();
        }
    }
    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Shot");
    }
}



/// <summary>
/// プレイヤーが空中にいる状態
/// クールタイムを経て、弾の発射ができる
/// 
/// </summary>
public class PlayerStateMidair : PlayerState
{
    private bool shotButton;
    private float countTime;


    public PlayerStateMidair()//コンストラクタ
    {
        shotButton = false;
        countTime = 0.0f;
    }

    public override void UpdateState()
    {
        countTime += Time.deltaTime;

        if (PlayerScript.leftStick.x > 0.2f)
        {
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (PlayerScript.leftStick.x < -0.2f)
        {
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (countTime > PlayerScript.BULLET_RECAST_TIME)
        {
            if (Input.GetButtonDown("Button_R"))
            {
                shotButton = true;
            }
        }
    }

    public override void Move()
    {
        //減衰
        PlayerScript.vel.x *= PlayerScript.MIDAIR_FRICTION;
        if (PlayerScript.leftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED;
        }
        else if (PlayerScript.leftStick.x < -PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * -1;
        }


        PlayerScript.rb.AddForce(Vector3.down * 30.8f, ForceMode.Acceleration);
        PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.rb.velocity.y, 0);
    }


    public override void StateTransition()
    { 
        if (shotButton)
        {
            PlayerScript.mode = new PlayerStateShot();
        }

        //着地したら立っている状態に移行
        Ray downRay = new Ray(PlayerScript.rb.position, Vector3.down);
        if (Physics.Raycast(downRay, 2.0f))
        {
            PlayerScript.mode = new PlayerStateStand();
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:MidAir");
    }
}

/// <summary>
/// 弾が天井でスイングしている状態
/// </summary>
public class PlayerStateSwing : PlayerState
{

    enum SwingMode { 
        Touced,   //捕まっている状態
        Rereased, //切り離した状態
    }

    SwingMode swingMode;
    BulletMain BulletScript;
    private bool finishFlag;
    private bool shotButton;
    private Vector3 ballPosition;
    private const float SWING_ANGLER_VELOCITY = 2.7f; //振り子角速度 ←const値ではなく前のstateのvelocityで可変が良さそう
    private const float SWING_REREASE_ANGLE = 140.0f; //振り子がのなす角が一定以上になったら強制解除

    public PlayerStateSwing()//コンストラクタ
    {
        finishFlag = false;
        swingMode = SwingMode.Touced;
        BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>();　
        shotButton = false;
        BulletScript.rb.isKinematic = true;
        ballPosition = BulletScript.gameObject.transform.position;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel = Vector3.zero;
    }

    public override void UpdateState()
    {
        //if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
        //{
        //    BulletScript.rb.isKinematic = false;
        //    shotButton = true;
        //}

        //ボールプレイヤー間の角度を求める
        Vector3 dt = Player.transform.position - ballPosition;
        float rad = Mathf.Atan2(dt.x, dt.y);
        float degree = rad * Mathf.Rad2Deg;
        if (degree < 0)//上方向を基準に時計回りに0~360の値に補正
        {
            degree += 360;
        }

        //一定角度以上で切り離し
        if (swingMode == SwingMode.Touced)
        {
            if (PlayerScript.dir == PlayerMoveDir.RIGHT)
            {
                if (degree < SWING_REREASE_ANGLE)
                {
                    BulletScript.gameObject.GetComponent<Collider>().isTrigger = true;
                    BulletScript.rb.isKinematic = false;
                    swingMode = SwingMode.Rereased;

                    //勢い追加
                    //弾とプレイヤー間のベクトルに直行するベクトル
                    Vector3 addVec = ballPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, -110) * addVec;
                    PlayerScript.rb.AddForce(addVec * 35.0f, ForceMode.VelocityChange);
                    
                }
            }
            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
            {
                if (degree > 360 - SWING_REREASE_ANGLE)
                {
                    BulletScript.gameObject.GetComponent<Collider>().isTrigger = true;
                    BulletScript.rb.isKinematic = false;
                    swingMode = SwingMode.Rereased;

                    //勢い追加
                    //弾とプレイヤー間のベクトルに直行するベクトル
                    Vector3 addVec = ballPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, 110) * addVec;
                    PlayerScript.rb.AddForce(addVec * 35.0f, ForceMode.VelocityChange);
                    
                }
            }
        }
    }

    public override void Move()
    {

        switch (swingMode) 
        {
            case SwingMode.Touced:
                //向きによって回転方向が違う
                Quaternion angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY, Vector3.forward);
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY, Vector3.forward);
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY * -1, Vector3.forward);
                }

                Vector3 pos = Player.transform.position;

                pos -= ballPosition;
                pos = angleAxis * pos;
                pos += ballPosition;
                PlayerScript.transform.position = pos;
                break;

            case SwingMode.Rereased:
                //自分へ弾を引き戻す
                float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
                vec = vec.normalized;

                BulletScript.rb.velocity = vec * 100;

                //距離が一定以下になったら弾を非アクティブ

                if (interval < 4.0f)
                {
                    PlayerScript.vel = PlayerScript.rb.velocity; //速度受け渡し
                    finishFlag = true;
                    Object.Destroy(BulletScript.gameObject);
                }

                break;

            default:
                break;
        }

        

    }


    public override void StateTransition()
    {
        if (finishFlag)
        {
            PlayerScript.mode = new PlayerStateMidair();
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Swing");
    }
}