using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// プレイヤーの状態管理をするインターフェース
/// MonoBehaviourは継承しない
/// </summary>
public class PlayerState
{
    virtual public void UpdateState() { }      //継承先でコントローラーの入力
    virtual public void Move() { }             //継承先で物理挙動（rigidbodyを使用したもの）overrideする
    virtual public void StateTransition() { }  //継承先でシーンの移動を決める
    virtual public void DebugMessage() { }     //デバッグ用のメッセージ表示

    static public GameObject Player;
    static public PlayerMain PlayerScript;
}


//public class PlayerStateTest : PlayerState
//{
//    Vector3 sPos, ePos;
//    float time = 0;

//    public PlayerStateTest()//コンストラクタ
//    {
//        PlayerScript.vel = Vector3.zero;
//        PlayerScript.canShot = true;

//        time = 0;
//        sPos = ePos = Player.transform.position;
//        ePos.y += 10.0f;
//    }

//    public override void UpdateState()
//    {
//        if (Input.GetKey(KeyCode.Space))
//        {
//            if (time < 1)
//            {
//                time += Time.deltaTime;
               
//            }
//        }
//    }
//}



/// <summary>
/// プレイヤーが地上にいる状態
/// スティックで移動、弾の発射ができる
/// </summary>
public class PlayerStateOnGround : PlayerState
{
    private bool shotButton;
    public PlayerStateOnGround()//コンストラクタ
    {
        PlayerScript.refState = EnumPlayerState.ON_GROUND;
        shotButton = false;
        PlayerScript.vel.y = 0;
        PlayerScript.canShot = true;

        if(PlayerScript.Bullet != null)
        {
            GameObject.Destroy(PlayerScript.Bullet);
        }
    }

    public override void UpdateState()
    {
        if (Input.GetButtonDown("Button_R"))
        {
            if (PlayerScript.stickCanShotRange)
            {
                shotButton = true;
            }
        }

        //プレイヤー向き回転処理
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            if (PlayerScript.leftStick.x < -0.01f)
            {
                PlayerScript.dir = PlayerMoveDir.LEFT;
                PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            if (PlayerScript.leftStick.x > 0.01f)
            {
                PlayerScript.dir = PlayerMoveDir.RIGHT;
                PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    public override void Move()
    {
        if(PlayerScript.leftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)　//右移動
        {
            if (PlayerScript.vel.x < -0.2f)
            {
                PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * 2;
            }
            else
            {
                PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED;
            }
           
            PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);
        }
        else if (PlayerScript.leftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1)　//左移動
        {

            if (PlayerScript.vel.x > 0.2f)
            {
                PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * -1 * 2;
            }
            else
            {
                PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * -1;
            }
                PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
        }
        else //減衰
        {
            PlayerScript.vel *= PlayerScript.RUN_FRICTION;
        }

    }


    public override void StateTransition()
    {
        if (PlayerScript.isOnGround == false)
        {
            PlayerScript.mode = new PlayerStateMidair(0.0f);
        }

        if (shotButton)
        {
            PlayerScript.mode = new PlayerStateShot_3();
        }
    }

}

/// <summary>
/// 弾を撃った状態(一度紐が伸び切ったら長さ固定のもの)
/// 弾はオブジェクトには接触していない
/// スティックでの移動不可、弾を引き戻すことのみ可能
/// </summary>
public class PlayerStateShot_2 : PlayerState
{
    float countTime;               //発射からの時間
    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //発射からの弾のvectorを保存する
    bool finishFlag;
    BulletMain BulletScript;
    public PlayerStateShot_2()//コンストラクタ
    {
        PlayerScript.refState = EnumPlayerState.SHOT;
        PlayerScript.shotState = ShotState.GO;
        PlayerScript.canShot = false;
        PlayerScript.forciblyReturnBulletFlag = false;
        PlayerScript.addVel = Vector3.zero;
        //弾の生成と発射
        //発射時にぶつからないように発射位置を矢印方向にずらす
        Vector3 vec = PlayerScript.leftStick.normalized;
        vec = vec * 5;
        vec.y += 1.0f;
        Vector3 popPos = PlayerScript.transform.position + vec;

        if (PlayerScript.Bullet != null)
        {
            GameObject.Destroy(PlayerScript.Bullet);
        }
        PlayerScript.Bullet = Object.Instantiate(PlayerScript.BulletPrefab, popPos, Quaternion.identity);
        BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //バレット情報のスナップ
    }

    public override void UpdateState()
    {
        countTime += Time.deltaTime;

        if (countTime > 0.3)
        {
            if (PlayerScript.shotState == ShotState.STRAINED)
            {
                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
                {
                    if (PlayerScript.Bullet != null)
                    {
                        BulletScript.ReturnBullet();
                    }
                    PlayerScript.vel = bulletVecs.Dequeue();
                    PlayerScript.useVelocity = true;
                    PlayerScript.shotState = ShotState.RETURN;           
                }
            }
        }

        //アンカーが刺さらない壁にあたったときなど、外部契機で引き戻しに移行
        if (PlayerScript.forciblyReturnBulletFlag)
        {
            PlayerScript.forciblyReturnBulletFlag = false;
            if (PlayerScript.Bullet != null)
            {
                if (PlayerScript.forciblyReturnSaveVelocity)
                {
                    PlayerScript.vel = bulletVecs.Dequeue();
                }
                else
                {
                    PlayerScript.vel = Vector3.zero;
                }

                BulletScript.ReturnBullet();
            }
            PlayerScript.useVelocity = true;
            PlayerScript.shotState = ShotState.RETURN;
        }

        if (PlayerScript.shotState == ShotState.STRAINED)
        {
            //弾からプレイヤー方向へBULLET_ROPE_LENGTHだけ離れた位置に常に補正
            Vector3 diff = (PlayerScript.transform.position - PlayerScript.Bullet.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
            Player.transform.position = PlayerScript.Bullet.transform.position + diff;
        }

        if (BulletScript.isTouched)
        {
            if (BulletScript.followEnd)
            {
                if (PlayerScript.Bullet != null)
                {
                    BulletScript.FollowedPlayer();
                }
                PlayerScript.vel = bulletVecs.Dequeue();
                PlayerScript.useVelocity = true;
                BulletScript.followEnd = false;
                PlayerScript.shotState = ShotState.FOLLOW;
            }
        }
    }

    public override void Move()
    {

        float interval;
        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
        switch (PlayerScript.shotState) {

            case ShotState.GO: 
                bulletVecs.Enqueue(BulletScript.vel);

                //紐の長さを超えたら引っ張られている状態にする
                if (interval > BulletScript.BULLET_ROPE_LENGTH)
                {
                    PlayerScript.shotState = ShotState.STRAINED;
                    PlayerScript.useVelocity = false;
                }
                break;

            case ShotState.STRAINED:
                bulletVecs.Enqueue(BulletScript.vel);
                bulletVecs.Dequeue();
                //このとき、移動処理は直にposition変更しているため???????、update内に記述
                //ここに記述するとカメラがブレる
                break;

            case ShotState.RETURN:
                //自分へ弾を引き戻す
                Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
                vecToPlayer = vecToPlayer.normalized;

                BulletScript.vel = vecToPlayer * 100;

                //距離が一定以下になったら終了処理フラグを建てる
                if (interval < 4.0f)
                {
                    finishFlag = true;
                }
                break;

            case ShotState.FOLLOW:
                //自分へ弾を引き戻す
                Vector3 vecToBullet = BulletScript.rb.position - PlayerScript.rb.position;
                vecToBullet = vecToBullet.normalized;

                PlayerScript.vel += vecToBullet * 3;

                if (interval < 4.0f)
                {
                    finishFlag = true;
                }

                break;
        }

        
    }

    public override void StateTransition()
    {
        if (finishFlag)
        {
            Object.Destroy(BulletScript.gameObject);
            //着地したら立っている状態に移行
            if (PlayerScript.isOnGround)
            {
                PlayerScript.mode = new PlayerStateOnGround();
            }
            else //そうでないなら空中
            {
                PlayerScript.mode = new PlayerStateMidair();
            }
        } 
   
        //ボールが触れたらスイング状態
        if (BulletScript.isTouched)
        {
            if (BulletScript.swingEnd)
            {
                BulletScript.swingEnd = false;
                PlayerScript.mode = new PlayerStateSwing_2();
            }
        }
    }
    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Shot");
    }
}


/// <summary>
/// 弾を撃った状態(一度紐が伸び切ったら長さ固定のもの)
/// 弾はオブジェクトには接触していない
/// スティックでの移動不可、弾を引き戻すことのみ可能
/// </summary>
public class PlayerStateShot_3 : PlayerState
{
    float countTime;               //発射からの時間
    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //発射からの弾のvectorを保存する
    bool finishFlag;
    BulletMain BulletScript;
    public PlayerStateShot_3()//コンストラクタ
    {
        PlayerScript.refState = EnumPlayerState.SHOT;
        PlayerScript.shotState = ShotState.GO;
        PlayerScript.canShot = false;
        PlayerScript.forciblyReturnBulletFlag = false;
        PlayerScript.addVel = Vector3.zero;
        //弾の生成と発射
        //発射時にぶつからないように発射位置を矢印方向にずらす
        Vector3 vec = PlayerScript.leftStick.normalized;
        vec = vec * 5;
        vec.y += 1.0f;
        Vector3 popPos = PlayerScript.transform.position + vec;

        if (PlayerScript.Bullet != null)
        {
            GameObject.Destroy(PlayerScript.Bullet);
        }
        PlayerScript.Bullet = Object.Instantiate(PlayerScript.BulletPrefab, popPos, Quaternion.identity);
        BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //バレット情報のスナップ
    }

    public override void UpdateState()
    {
        countTime += Time.deltaTime;

        if (countTime > 0.3)
        {
            if (PlayerScript.shotState == ShotState.STRAINED)
            {
                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
                {
                    if (PlayerScript.Bullet != null)
                    {
                        BulletScript.ReturnBullet();
                    }
                    PlayerScript.vel = bulletVecs.Dequeue();
                    PlayerScript.useVelocity = true;
                    PlayerScript.shotState = ShotState.RETURN;
                }
            }
        }

        //アンカーが刺さらない壁にあたったときなど、外部契機で引き戻しに移行
        if (PlayerScript.forciblyReturnBulletFlag)
        {
            PlayerScript.forciblyReturnBulletFlag = false;
            if (PlayerScript.Bullet != null)
            {
                if (PlayerScript.forciblyReturnSaveVelocity)
                {
                    PlayerScript.vel = bulletVecs.Dequeue();
                }
                else
                {
                    PlayerScript.vel = Vector3.zero;
                }

                BulletScript.ReturnBullet();
            }
            PlayerScript.useVelocity = true;
            PlayerScript.shotState = ShotState.RETURN;
        }

        if (PlayerScript.shotState == ShotState.STRAINED)
        {
            float interval;
            interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
            if (interval > BulletScript.BULLET_ROPE_LENGTH)
            {
                //弾からプレイヤー方向へBULLET_ROPE_LENGTHだけ離れた位置に常に補正
                Vector3 diff = (PlayerScript.transform.position - PlayerScript.Bullet.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
                Player.transform.position = PlayerScript.Bullet.transform.position + diff;
            }
        }

        if (BulletScript.isTouched)
        {
            if (BulletScript.followEnd)
            {
                if (PlayerScript.Bullet != null)
                {
                    BulletScript.FollowedPlayer();
                }
                PlayerScript.vel = bulletVecs.Dequeue();
                PlayerScript.useVelocity = true;
                BulletScript.followEnd = false;
                PlayerScript.shotState = ShotState.FOLLOW;
            }
        }
    }

    public override void Move()
    {

        float interval;
        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
        switch (PlayerScript.shotState)
        {

            case ShotState.GO:
                bulletVecs.Enqueue(BulletScript.vel);

                //紐の長さを超えたら引っ張られている状態にする
                if (interval > BulletScript.BULLET_ROPE_LENGTH)
                {
                    PlayerScript.shotState = ShotState.STRAINED;
                    PlayerScript.useVelocity = false;
                }
                break;

            case ShotState.STRAINED:
                bulletVecs.Enqueue(BulletScript.vel);
                bulletVecs.Dequeue();
                //このとき、移動処理は直にposition変更しているため???????、update内に記述
                //ここに記述するとカメラがブレる
                break;

            case ShotState.RETURN:
                //自分へ弾を引き戻す
                Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
                vecToPlayer = vecToPlayer.normalized;

                BulletScript.vel = vecToPlayer * 100;

                //距離が一定以下になったら終了処理フラグを建てる
                if (interval < 4.0f)
                {
                    finishFlag = true;
                }
                break;

            case ShotState.FOLLOW:
                //自分へ弾を引き戻す
                Vector3 vecToBullet = BulletScript.rb.position - PlayerScript.rb.position;
                vecToBullet = vecToBullet.normalized;

                PlayerScript.vel += vecToBullet * 3;

                if (interval < 4.0f)
                {
                    finishFlag = true;
                }

                break;
        }


    }

    public override void StateTransition()
    {
        if (finishFlag)
        {
            Object.Destroy(BulletScript.gameObject);
            //着地したら立っている状態に移行
            if (PlayerScript.isOnGround)
            {
                PlayerScript.mode = new PlayerStateOnGround();
            }
            else //そうでないなら空中
            {
                PlayerScript.mode = new PlayerStateMidair();
            }
        }

        //ボールが触れたらスイング状態
        if (BulletScript.isTouched)
        {
            if (BulletScript.swingEnd)
            {
                BulletScript.swingEnd = false;
                PlayerScript.mode = new PlayerStateSwing_2();
            }
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
/// </summary>
public class PlayerStateMidair : PlayerState
{
    private bool shotButton;  
    private float countTimer; //再射出可能時間に使うタイマー
    private float recastTime; //再射出可能時間

    private void Init()
    {
        PlayerScript.refState = EnumPlayerState.MIDAIR;
        shotButton = false;
        countTimer = 0.0f;
        PlayerScript.canShot = false;

        if (PlayerScript.Bullet != null)
        {
            GameObject.Destroy(PlayerScript.Bullet);
        }
    }

    public PlayerStateMidair()//コンストラクタ
    {
        Init();
        recastTime = PlayerScript.BULLET_RECAST_TIME;
    }

    //再射出可能時間を指定
    public PlayerStateMidair(float　recast_time)
    {
        Init();
        recastTime = recast_time;
        if(recastTime < 0.0001f)
        {
            PlayerScript.canShot = true; 
        }
    }

    
    public override void UpdateState()
    {
        countTimer += Time.deltaTime;

        if (PlayerScript.leftStick.x > 0.01f)
        {
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (PlayerScript.leftStick.x < -0.01f)
        {
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (countTimer > recastTime)
        {
            if(PlayerScript.canShot == false)
            {
                PlayerScript.canShot = true;
            }

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


        PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY;
        PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
    }


    public override void StateTransition()
    { 
        if (shotButton)
        {
            PlayerScript.mode = new PlayerStateShot_3();
        }

        //着地したら立っている状態に移行
        if (PlayerScript.isOnGround)
        {
            PlayerScript.mode = new PlayerStateOnGround();
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
        PlayerScript.refState = EnumPlayerState.SWING;
        PlayerScript.canShot = false;
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
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    swingMode = SwingMode.Rereased;

                    //勢い追加
                    //弾とプレイヤー間のベクトルに直行するベクトル
                    Vector3 addVec = ballPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, -110) * addVec;
                    PlayerScript.vel += addVec * 35.0f;
                }
            }
            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
            {
                if (degree > 360 - SWING_REREASE_ANGLE)
                {
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    swingMode = SwingMode.Rereased;

                    //勢い追加
                    //弾とプレイヤー間のベクトルに直行するベクトル
                    Vector3 addVec = ballPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, 110) * addVec;
                    PlayerScript.vel += addVec * 35.0f;
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

                BulletScript.vel= vec * 100;

                //距離が一定以下になったら弾を非アクティブ

                if (interval < 4.0f)
                {
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
            PlayerScript.mode = new PlayerStateMidair(0.0f);
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Swing");
    }
}


/// <summary>
/// 弾が天井でスイングしている状態
/// 移動速度等を保存
/// </summary>
public class PlayerStateSwing_2 : PlayerState
{
    BulletMain BulletScript;
    private bool finishFlag;
    private bool shotButton;
    private Vector3 ballPosition; //ボールの位置
    private const float SWING_ANGLER_VELOCITY = 2.7f; //振り子角速度 ←const値ではなく前のstateのvelocityで可変が良さそう
    private const float SWING_REREASE_ANGLE = 140.0f; //振り子がのなす角が一定以上になったら強制解除


    private float betweenLength; //開始時二点間の距離(距離はスイングstate通して固定)
    private Vector3 startPlayerVel;　　　　　　 //突入時
    private float startAngle;    //開始時の二点間アングル
    private float endAngle;      //自動切り離しされる角度(start角度依存)
    private float minAnglerVel;  //最低角速度（自動切り離し地点にいる時）
    private float maxAnglerVel;　//最高角速度 (真下にプレイヤーが居る時）
    private float nowAnglerVel;  //現在角速度
    public PlayerStateSwing_2()  //コンストラクタ
    {
        PlayerScript.refState = EnumPlayerState.SWING;
        PlayerScript.swingState = SwingState.TOUCHED;
        PlayerScript.canShot = false;
        PlayerScript.endSwing = false;
        finishFlag = false;
        BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>();
        shotButton = false;
        BulletScript.rb.isKinematic = true;
        ballPosition = BulletScript.gameObject.transform.position;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel = Vector3.zero;

        CalculateStartVariable();
    }

    public void CalculateStartVariable()
    {
        betweenLength = Vector3.Distance(Player.transform.position, ballPosition);

        float degree = CalculationScript.TwoPointAngle360(ballPosition, Player.transform.position);
        startAngle = endAngle = degree;



        //切り離しアングルの計算
        float diff_down = Mathf.Abs(startAngle - 180.0f);
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            endAngle -= diff_down + diff_down;
            endAngle = Mathf.Clamp(endAngle, 90, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            endAngle += diff_down + diff_down;

            endAngle = Mathf.Clamp(endAngle, 220, 270);
        }

        //突入時角速度の計算
        minAnglerVel = 2.0f;
        maxAnglerVel = 4.5f;
        nowAnglerVel = minAnglerVel;
    }

    public override void UpdateState()
    {
        //if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
        //{
        //    BulletScript.rb.isKinematic = false;
        //    shotButton = true;
        //}

        //ボールプレイヤー間の角度を求める
        float degree = CalculationScript.TwoPointAngle360(ballPosition, Player.transform.position);

        //一定角度以上で切り離し
        if (PlayerScript.swingState == SwingState.TOUCHED)
        {
            if (PlayerScript.endSwing)
            {
                PlayerScript.endSwing = false;
                PlayerScript.useVelocity = true;
                BulletScript.ReturnBullet();
                PlayerScript.swingState = SwingState.RELEASED;
            }

            if (PlayerScript.dir == PlayerMoveDir.RIGHT)
            {
                if (degree < endAngle)
                {
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;

                    //勢い追加
                    //弾とプレイヤー間のベクトルに直行するベクトル
                    Vector3 addVec = ballPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, -110) * addVec;
                    PlayerScript.vel += addVec * 35.0f;
                }
            }
            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
            {
                if (degree > endAngle)
                {
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;

                    //勢い追加
                    //弾とプレイヤー間のベクトルに直行するベクトル
                    Vector3 addVec = ballPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, 110) * addVec;
                    PlayerScript.vel += addVec * 35.0f;
                }
            }
        }
    }

    public override void Move()
    {
        float degree = CalculationScript.TwoPointAngle360(ballPosition, Player.transform.position);
        float deg180dif = Mathf.Abs(degree - 180);
        switch (PlayerScript.swingState)
        {
            case SwingState.TOUCHED:
                //角速度計算
                float deg180Ratio = deg180dif / Mathf.Abs(endAngle - 180); //真下と最高到達点の比率
                deg180Ratio = Mathf.Clamp01(deg180Ratio); //一応範囲内に補正
                deg180Ratio = 1 - deg180Ratio; //真下を1,最高到達点を0とする

                float vvv = Easing.Linear(deg180Ratio, 1.0f, 0.0f, 1.0f);

                nowAnglerVel = ((maxAnglerVel - minAnglerVel) * vvv) + minAnglerVel;

                //向きによって回転方向が違う
                Quaternion angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY, Vector3.forward);
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    angleAxis = Quaternion.AngleAxis(nowAnglerVel, Vector3.forward);
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    angleAxis = Quaternion.AngleAxis(nowAnglerVel * -1, Vector3.forward);
                }

                Vector3 pos = Player.transform.position;

                pos -= ballPosition;
                pos = angleAxis * pos;
                pos += ballPosition;
                PlayerScript.transform.position = pos;
                break;

            case SwingState.RELEASED:
                //自分へ弾を引き戻す
                float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
                vec = vec.normalized;
                BulletScript.vel = vec * 100;

                //距離が一定以下になったら弾を非アクティブ
                if (interval < 4.0f)
                {
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
            PlayerScript.swingState = SwingState.NONE;
            PlayerScript.mode = new PlayerStateMidair(0.0f);
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Swing");
    }
}


/// <summary>
/// 死亡時アニメーション等の制御クラス
/// </summary>
public class PlayerStateDeath : PlayerState
{
    public PlayerStateDeath()
    {
        PlayerScript.refState = EnumPlayerState.DEATH;
        PlayerScript.canShot = false;
        PlayerScript.vel = Vector3.zero;
        PlayerScript.addVel = Vector3.zero;

        if (PlayerScript.Bullet != null)
        {
            PlayerScript.Bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
            PlayerScript.Bullet.GetComponent<BulletMain>().vel = Vector3.zero;
            PlayerScript.Bullet.GetComponent<BulletMain>().StopDownVel = true;
        }
    }

    public override void UpdateState()
    {
        // フェード処理
        FadeManager.Instance.SetNextFade(FADE_STATE.FADE_OUT, FADE_KIND.FADE_GAMOVER);
    }

    public override void Move()
    {
        //移動なし
    }

    public override void StateTransition()
    {
        //ここから派生することはない
        //シーン変更してクイックリトライ位置にリポップ
    }

}



///過去のスクリプト
///// <summary>
///// プレイヤーが立ち止まっている状態
///// スティック入力での移動、弾の発射ができる
///// </summary>
//public class PlayerStateStand : PlayerState
//{
//    private bool shotButton;
//    public PlayerStateStand()//コンストラクタ
//    {
//        shotButton = false;
//    }

//    public override void UpdateState()
//    {
//        if (Input.GetButtonDown("Button_R"))
//        {
//            if (PlayerScript.canShot)
//            {
//                shotButton = true;
//            }
//        }

//        if (PlayerScript.leftStick.x > 0.2f)
//        {
//            PlayerScript.dir = PlayerMoveDir.RIGHT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
//        }
//        else if (PlayerScript.leftStick.x < -0.2f)
//        {
//            PlayerScript.dir = PlayerMoveDir.LEFT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
//        }
//    }

//    public override void Move()
//    {
//        //減衰
//        if (PlayerScript.vel.x > PlayerScript.MIN_RUN_SPEED)
//        {
//            PlayerScript.vel *= PlayerScript.RUN_FRICTION;
//            if (PlayerScript.vel.x < PlayerScript.MIN_RUN_SPEED)
//            {
//                PlayerScript.vel.x = 0;
//            }
//        }
//        if (PlayerScript.vel.x < PlayerScript.MIN_RUN_SPEED * -1)
//        {
//            PlayerScript.vel *= PlayerScript.RUN_FRICTION;
//            if (PlayerScript.vel.x > PlayerScript.MIN_RUN_SPEED * -1)
//            {
//                PlayerScript.vel.x = 0;
//            }
//        }

//        //重力
//        PlayerScript.vel += Vector3.down * 0.8f;
//        PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
//    }


//    public override void StateTransition()
//    {
//        //右移動に移行
//        if (PlayerScript.leftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)
//        {
//            PlayerScript.dir = PlayerMoveDir.RIGHT;
//            PlayerScript.mode = new PlayerStateMove();
//        }

//        //左移動に移行
//        if (PlayerScript.leftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1)
//        {
//            PlayerScript.dir = PlayerMoveDir.LEFT;
//            PlayerScript.mode = new PlayerStateMove();
//        }

//        if (shotButton)
//        {
//            PlayerScript.mode = new PlayerStateShot();
//        }
//    }

//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Stand");
//    }
//}

///// <summary>
///// プレイヤーが移動している状態
///// スティック入力での移動、弾の発射ができる
///// </summary>
//public class PlayerStateMove : PlayerState
//{
//    bool shotButton;

//    public PlayerStateMove()//コンストラクタ
//    {
//        shotButton = false;

//        //プレイヤーの回転
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
//        }
//        if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
//        }
//    }

//    public override void UpdateState()
//    {
//        if (Input.GetButtonDown("Button_R"))
//        {
//            if (PlayerScript.canShot)
//            {
//                shotButton = true;
//            }
//        }
//    }

//    public override void Move()
//    {
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED;
//            PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);
//        }
//        if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * -1;
//            PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
//        }

//        //重力
//        PlayerScript.vel += Vector3.down * 0.8f;
//        PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
//    }

//    public override void StateTransition()
//    {
//        //スティックがしきい値以下ならstand状態に移行
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            if (PlayerScript.leftStick.x < (PlayerScript.LATERAL_MOVE_THRESHORD) - 0.1)
//            {
//                PlayerScript.mode = new PlayerStateStand();
//            }
//        }
//        if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            if (PlayerScript.leftStick.x > (PlayerScript.LATERAL_MOVE_THRESHORD * -1) + 0.1)
//            {
//                PlayerScript.mode = new PlayerStateStand();
//            }
//        }

//        if (shotButton)
//        {
//            PlayerScript.mode = new PlayerStateShot();
//        }
//    }


//    public override void DebugMessage()
//    {
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            Debug.Log("PlayerState:RunRight");
//        }
//        if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            Debug.Log("PlayerState:RunLeft");
//        }
//    }
//}

///// <summary>
///// 弾を撃った状態
///// 弾はオブジェクトには接触していない
///// スティックでの移動不可、弾を引き戻すことのみ可能
///// </summary>
//public class PlayerStateShot : PlayerState
//{
//    private enum SHOT_STATE
//    {
//        GO,         //引っ張られずに飛んでいる
//        STRAINED,　 //プレイヤーを引っ張りながら飛んでいる
//        RETURN,     //プレイヤーに戻っている
//    }

//    SHOT_STATE shotState;               //紐が張り詰めているか（張り詰めていたら弾についていく）
//    float countTime;               //発射からの時間
//    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //発射からの弾のvectorを保存する
//    bool finishFlag;
//    BulletMain BulletScript;
//    public PlayerStateShot()//コンストラクタ
//    {
//        PlayerScript.refState = EnumPlayerState.SHOT;
//        PlayerScript.canShot = false;
//        //弾の生成と発射
//        //発射時にぶつからないように発射位置を矢印方向にずらす
//        Vector3 vec = PlayerScript.leftStick.normalized;
//        Vector3 popPos = PlayerScript.transform.position + (vec * 5);
//        PlayerScript.Bullet = Object.Instantiate(PlayerScript.BulletPrefab, popPos, Quaternion.identity);
//        BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //バレット情報のスナップ
//    }

//    public override void UpdateState()
//    {
//        countTime += Time.deltaTime;

//        if (countTime > 0.3)
//        {
//            if (shotState == SHOT_STATE.STRAINED)
//            {
//                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
//                {
//                    if (PlayerScript.Bullet != null)
//                    {
//                        BulletScript.ReturnBullet();
//                    }
//                    shotState = SHOT_STATE.RETURN;
//                }
//            }
//        }
//    }

//    public override void Move()
//    {

//        float interval;

//        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
//        switch (shotState)
//        {

//            case SHOT_STATE.GO:

//                bulletVecs.Enqueue(BulletScript.vel);

//                //紐の長さを超えたら引っ張られている状態にする
//                if (interval > BulletScript.BULLET_ROPE_LENGTH)
//                {
//                    shotState = SHOT_STATE.STRAINED;
//                }

//                break;

//            case SHOT_STATE.STRAINED:
//                //弾の進行方向の逆方向に位置補正
//                bulletVecs.Enqueue(BulletScript.vel);
//                PlayerScript.vel = bulletVecs.Dequeue();

//                //調整
//                //if (interval > BulletScript.BULLET_ROPE_LENGTH)
//                //{
//                //    Vector3 ballVel = BulletScript.rb.velocity;
//                //    Vector3 reverceVel = (ballVel * -1).normalized;

//                //    PlayerScript.transform.position = BulletScript.transform.position + reverceVel * BulletScript.BULLET_ROPE_LENGTH;
//                //}
//                break;

//            case SHOT_STATE.RETURN:
//                //自分へ弾を引き戻す
//                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
//                vec = vec.normalized;

//                BulletScript.vel = vec * 50;

//                //距離が一定以下になったら終了処理フラグを建てる
//                if (interval < 4.0f)
//                {
//                    finishFlag = true;
//                }

//                break;
//        }


//    }

//    public override void StateTransition()
//    {
//        if (finishFlag)
//        {
//            Object.Destroy(BulletScript.gameObject);
//            //着地したら立っている状態に移行
//            if (PlayerScript.isOnGround)
//            {
//                PlayerScript.mode = new PlayerStateOnGround();
//            }
//            else //そうでないなら空中
//            {
//                PlayerScript.mode = new PlayerStateMidair();
//            }
//        }

//        //ボールが触れたらスイング状態
//        if (BulletScript.isTouched)
//        {
//            PlayerScript.mode = new PlayerStateSwing();
//        }
//    }
//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Shot");
//    }
//}
