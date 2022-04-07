using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// プレイヤーの状態管理をするインターフェース
/// MonoBehaviourは継承しない
/// </summary>
public class PlayerState
{
    protected float fixedAdjust = Time.fixedDeltaTime * 50; 

    virtual public void UpdateState() { }      //継承先でコントローラーの入力
    virtual public void Move() { }             //継承先で物理挙動（rigidbodyを使用したもの）overrideする
    virtual public void StateTransition() { }  //継承先でシーンの移動を決める
    virtual public void DebugMessage() { }     //デバッグ用のメッセージ表示

    static public GameObject Player;
    static public PlayerMain PlayerScript;
    static public BulletMain BulletScript;

    /// <summary>
    /// バレットの位置を常にスティック方向に調整
    /// </summary>
    protected void BulletAdjust()
    {
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;
        vec = vec * 3;
        vec.y += 1.0f;
        Vector3 adjustPos = PlayerScript.transform.position + vec;

        BulletScript.transform.position = adjustPos;
    }

}

/// <summary>
/// プレイヤーが地上にいる状態
/// スティックで移動、弾の発射ができる
/// </summary>
public class PlayerStateOnGround : PlayerState
{
    private bool shotButton;
    private bool jumpButton;
    private const float SLIDE_END_TIME = 0.3f; 
    private float slideEndTimer;

    public PlayerStateOnGround()//コンストラクタ
    {
        PlayerScript.refState = EnumPlayerState.ON_GROUND;
        shotButton = false;
        jumpButton = false;
        PlayerScript.vel.y = 0;
        PlayerScript.canShotState = true;
        slideEndTimer = 0.0f;

        //ボール関連
        BulletScript.InvisibleBullet();


        //スライド発射処理
        if (Mathf.Abs(PlayerScript.vel.x) > 30.0f)
        {
            PlayerScript.onGroundState = OnGroundState.SLIDE;
        }
        else
        {
            PlayerScript.onGroundState = OnGroundState.NORMAL;
        }
    }

    public override void UpdateState()
    {
        BulletAdjust();

        if (Input.GetButtonDown("Button_R"))
        {
            if (PlayerScript.canShot)
            {
                shotButton = true;
            }
        }
        else if (Input.GetButtonDown("Jump"))
        {
            jumpButton = true;
            Debug.Log("Press Jump Button");
        }

        //プレイヤー向き回転処理
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            if (PlayerScript.adjustLeftStick.x < -0.01f)
            {
                PlayerScript.dir = PlayerMoveDir.LEFT;
                PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
            }
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            if (PlayerScript.adjustLeftStick.x > 0.01f)
            {
                PlayerScript.dir = PlayerMoveDir.RIGHT;
                PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }

    public override void Move()
    {

        if (PlayerScript.onGroundState == OnGroundState.SLIDE)
        {
            float slide_Weaken = 0.5f;

            if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD) //右移動
            {
                if (PlayerScript.vel.x < -0.2f)//ターンしてるときは早い
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * 2 * slide_Weaken * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * slide_Weaken * 0.4f * (fixedAdjust); 
                }

                //PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);
            }
            else if (PlayerScript.adjustLeftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1) //左移動
            {

                if (PlayerScript.vel.x > 0.2f)
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * -1 * 2 * slide_Weaken * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * -1 * slide_Weaken * 0.4f * (fixedAdjust);
                }
                //PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
            }


            //減衰
            {
                PlayerScript.vel *= 0.97f;
            }

            //スライド終了処理（時間によるもの
            slideEndTimer += Time.fixedDeltaTime;
            if(slideEndTimer > SLIDE_END_TIME)
            {
                PlayerScript.onGroundState = OnGroundState.NORMAL;
                PlayerScript.canShotState = true;
            }
        }
        else //!isSlide
        {
            if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD) //右移動
            {
                if (PlayerScript.vel.x < -0.2f)
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * 2 * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * (fixedAdjust);
                }

                PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);
            }
            else if (PlayerScript.adjustLeftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1) //左移動
            {

                if (PlayerScript.vel.x > 0.2f)
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * -1 * 2 * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * -1 * (fixedAdjust);
                }
                PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
            }
            else //減衰
            {
                PlayerScript.vel *= PlayerScript.RUN_FRICTION; 
            }
        }
    }


    public override void StateTransition()
    {
        if (PlayerScript.isOnGround == false)
        {
            PlayerScript.onGroundState = OnGroundState.NONE;
            PlayerScript.mode = new PlayerStateMidair(true);
        }

        if (shotButton)
        {
            //スライド中で投げる方向が進行方向と同じなら
            if ((PlayerScript.onGroundState == OnGroundState.SLIDE) && (PlayerScript.adjustLeftStick.x * PlayerScript.vel.x > 0))
            {
                PlayerScript.onGroundState = OnGroundState.NONE;
                PlayerScript.mode = new PlayerStateShot_2(true);
            }
            else
            {
                PlayerScript.onGroundState = OnGroundState.NONE;
                PlayerScript.mode = new PlayerStateShot_2(false);
            }
        }

        if (jumpButton)
        {
            Debug.Log("Jump!!!!");
            PlayerScript.onGroundState = OnGroundState.NONE;
            PlayerScript.mode = new PlayerStateMidair(true, true);
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

    const float STRAINED_END_RATIO = 1.0f;

    private void Init()
    {
        countTime = 0.0f;
        bulletVecs = new Queue<Vector3>();
        finishFlag = false;

        PlayerScript.refState = EnumPlayerState.SHOT;
        PlayerScript.shotState = ShotState.GO;
        PlayerScript.canShotState = false;
        PlayerScript.forciblyReturnBulletFlag = false;
        PlayerScript.addVel = Vector3.zero;
        
    }

    public PlayerStateShot_2(bool is_slide_jump)//コンストラクタ
    {
        Init();
        //弾の発射
        BulletScript.GetComponent<Collider>().isTrigger = false;
        BulletScript.VisibleBullet();

        if (is_slide_jump)
        {
            BulletScript.ShotSlideJumpBullet();
            Debug.Log("Slide Shot");
        }
        else
        {
            BulletScript.ShotBullet();
            Debug.Log("Normal Shot");
        }
    }

    /// <summary>
    /// 引っ張られているとき、プレイヤーを進行方向に対して回転
    /// </summary>
    public void RotationPlayer()
    {

        switch (PlayerScript.shotState)
        {
            case ShotState.STRAINED:

                Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;

                Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);
                Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //補正用クオータニオン
                quaternion *= adjustQua;
                PlayerScript.rb.rotation = quaternion;
                break;

            case ShotState.RETURN:
            case ShotState.FOLLOW:
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 90, 0));         
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, -90, 0));
                }
                break;
        }

      
    }

    /// <summary>
    /// 引っ張られている時間にオブジェクトがあったら切り離し
    /// </summary>
    private void StrainedStop()
    {
        Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;
        Ray ray = new Ray(PlayerScript.rb.position, vecToPlayer.normalized);

        if (Physics.SphereCast(ray, PlayerMain.HcolliderRadius, PlayerMain.HcoliderDistance, LayerMask.GetMask("Platform")))
        { 
            if(BulletScript.isTouched == false)
            {
                Debug.Log("collision PlayerHead : Forcibly return");
                PlayerScript.ForciblyReturnBullet(true);
            }
        }
    }

    public override void UpdateState()
    {
        countTime += Time.deltaTime;

        if (countTime > 0.2)
        {
            if (PlayerScript.shotState == ShotState.STRAINED)
            {
                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
                {
                   
                    BulletScript.ReturnBullet();
                    
                    PlayerScript.vel = bulletVecs.Dequeue() * STRAINED_END_RATIO;
                    
                    PlayerScript.useVelocity = true;
                    PlayerScript.shotState = ShotState.RETURN;           
                }
            }
        }

        //アンカーが刺さらない壁にあたったときなど、外部契機で引き戻しに移行
        if (PlayerScript.forciblyReturnBulletFlag)
        {
            PlayerScript.forciblyReturnBulletFlag = false;
           
            if (PlayerScript.forciblyReturnSaveVelocity)
            {
                PlayerScript.vel = bulletVecs.Dequeue() * STRAINED_END_RATIO;
            }
            else
            {
                PlayerScript.vel = Vector3.zero;
            }

            BulletScript.ReturnBullet();
            
            PlayerScript.useVelocity = true;
            PlayerScript.shotState = ShotState.RETURN;
        }

        //ついていく処理
        if (PlayerScript.shotState == ShotState.STRAINED)
        {
#if false
            //弾からプレイヤー方向へBULLET_ROPE_LENGTHだけ離れた位置に常に補正
            Vector3 diff = (PlayerScript.transform.position - BulletScript.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
            Player.transform.position = BulletScript.transform.position + diff;
#else
            //弾からプレイヤー方向へBULLET_ROPE_LENGTHだけ離れた位置に常に補正
            if(Vector3.Magnitude(Player.transform.position - BulletScript.transform.position) > BulletScript.BULLET_ROPE_LENGTH)
            {
                Vector3 diff = (PlayerScript.transform.position - BulletScript.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
                Player.transform.position = BulletScript.transform.position + diff;
                //弾がプレイヤーより強い勢いを持っているときのみ
                if (PlayerScript.vel.magnitude < BulletScript.vel.magnitude * 0.8f)
                {
                    PlayerScript.vel = BulletScript.vel * 0.8f;
                }
            }

            //STRAINEDだけど自由移動のタイミング
            else
            {
                //弱めの重力 
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 0.1f * (fixedAdjust);
                PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
            }
#endif
        }

        if (BulletScript.isTouched)
        {
            if (BulletScript.followEnd)
            {
                BulletScript.FollowedPlayer();

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

        RotationPlayer();

        switch (PlayerScript.shotState) {

            case ShotState.GO: 
                bulletVecs.Enqueue(BulletScript.vel * 0.6f);

                //紐の長さを超えたら引っ張られている状態にする
                if (interval > BulletScript.BULLET_ROPE_LENGTH)
                {
                    //引っ張られたタイミングでボール減速
                    //if(BulletScript.vel.magnitude > 60.0f)
                    //{
                    //    BulletScript.vel *= 0.64f;
                    //}
                    //else if(BulletScript.vel.magnitude > 40.0f)
                    //{
                    //    BulletScript.vel *= 0.92f;
                    //}

                    BulletScript.vel *= 0.84f;

                    PlayerScript.shotState = ShotState.STRAINED;
                    //PlayerScript.useVelocity = false;
                }
                break;

            case ShotState.STRAINED:
               
                bulletVecs.Enqueue(BulletScript.vel);
                bulletVecs.Dequeue();
                StrainedStop();
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
        //ボールが触れたらスイング状態
        if (BulletScript.isTouched)
        {
            PlayerScript.shotState = ShotState.NONE;
            if (BulletScript.swingEnd)
            {
                BulletScript.swingEnd = false;
                PlayerScript.mode = new PlayerStateSwing_R_Release();
            }
        }

        if (finishFlag)
        {
            PlayerScript.shotState = ShotState.NONE;
            //着地したら立っている状態に移行
            if (PlayerScript.isOnGround)
            {
                PlayerScript.mode = new PlayerStateOnGround();
            }
            else //そうでないなら空中
            {
                PlayerScript.mode = new PlayerStateMidair(false);
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
//public class PlayerStateShot_3 : PlayerState
//{
//    float countTime;               //発射からの時間
//    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //発射からの弾のvectorを保存する
//    bool finishFlag;
//    BulletMain BulletScript;
//    public PlayerStateShot_3()//コンストラクタ
//    {
//        PlayerScript.refState = EnumPlayerState.SHOT;
//        PlayerScript.shotState = ShotState.GO;
//        PlayerScript.canShotState = false;
//        PlayerScript.forciblyReturnBulletFlag = false;
//        PlayerScript.addVel = Vector3.zero;
//        //弾の生成と発射
//        //発射時にぶつからないように発射位置を矢印方向にずらす
//        Vector3 vec = PlayerScript.leftStick.normalized;
//        vec = vec * 5;
//        vec.y += 1.0f;
//        Vector3 popPos = PlayerScript.transform.position + vec;

//        if (ReferenceEquals(PlayerScript.Bullet, null) == false)
//        {
//            GameObject.Destroy(PlayerScript.Bullet);
//            PlayerScript.Bullet = null;
//        }

//        PlayerScript.Bullet = Object.Instantiate(PlayerScript.BulletPrefab, popPos, Quaternion.identity);
//        BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //バレット情報のスナップ
//    }

//    public override void UpdateState()
//    {
//        countTime += Time.deltaTime;

//        if (countTime > 0.3)
//        {
//            if (PlayerScript.shotState == ShotState.STRAINED)
//            {
//                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
//                {
//                    if (PlayerScript.Bullet != null)
//                    {
//                        BulletScript.ReturnBullet();
//                    }
//                    PlayerScript.vel = bulletVecs.Dequeue();
//                    PlayerScript.useVelocity = true;
//                    PlayerScript.shotState = ShotState.RETURN;
//                }
//            }
//        }

//        //アンカーが刺さらない壁にあたったときなど、外部契機で引き戻しに移行
//        if (PlayerScript.forciblyReturnBulletFlag)
//        {
//            PlayerScript.forciblyReturnBulletFlag = false;
//            if (PlayerScript.Bullet != null)
//            {
//                if (PlayerScript.forciblyReturnSaveVelocity)
//                {
//                    PlayerScript.vel = bulletVecs.Dequeue();
//                }
//                else
//                {
//                    PlayerScript.vel = Vector3.zero;
//                }

//                BulletScript.ReturnBullet();
//            }
//            PlayerScript.useVelocity = true;
//            PlayerScript.shotState = ShotState.RETURN;
//        }

//        if (PlayerScript.shotState == ShotState.STRAINED)
//        {
//            float interval;
//            interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);

           
//            if (interval > BulletScript.BULLET_ROPE_LENGTH)
//            {
//                //弾からプレイヤー方向へBULLET_ROPE_LENGTHだけ離れた位置に常に補正
//                PlayerScript.useVelocity = false;
//                Vector3 diff = (PlayerScript.transform.position - PlayerScript.Bullet.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
//                Player.transform.position = PlayerScript.Bullet.transform.position + diff;
//            }
//            else
//            {
//                PlayerScript.useVelocity = true;
//            }
//        }

//        if (BulletScript.isTouched)
//        {
//            if (BulletScript.followEnd)
//            {
//                if (PlayerScript.Bullet != null)
//                {
//                    BulletScript.FollowedPlayer();
//                }
//                PlayerScript.vel = bulletVecs.Dequeue();
//                PlayerScript.useVelocity = true;
//                BulletScript.followEnd = false;
//                PlayerScript.shotState = ShotState.FOLLOW;
//            }
//        }
//    }

//    public override void Move()
//    {

//        float interval;
//        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
//        switch (PlayerScript.shotState)
//        {

//            case ShotState.GO:
//                bulletVecs.Enqueue(BulletScript.vel);

//                //紐の長さを超えたら引っ張られている状態にする
//                if (interval > BulletScript.BULLET_ROPE_LENGTH)
//                {
//                    PlayerScript.shotState = ShotState.STRAINED;
//                    PlayerScript.useVelocity = false;
//                }
//                break;

//            case ShotState.STRAINED:
//                bulletVecs.Enqueue(BulletScript.vel);
//                if (PlayerScript.useVelocity == true)
//                {
//                    PlayerScript.vel = bulletVecs.Peek(); //* (1 /Time.fixedDeltaTime); //PlayerScript.FALL_GRAVITY; //
//                }

//                bulletVecs.Dequeue();
//                //このとき、移動処理は直にposition変更しているため???????、update内に記述
//                //ここに記述するとカメラがブレる
                
//                break;

//            case ShotState.RETURN:
//                //自分へ弾を引き戻す
//                Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
//                vecToPlayer = vecToPlayer.normalized;

//                BulletScript.vel = vecToPlayer * 100;

//                //距離が一定以下になったら終了処理フラグを建てる
//                if (interval < 4.0f)
//                {
//                    finishFlag = true;
//                }
//                break;

//            case ShotState.FOLLOW:
//                //自分へ弾を引き戻す
//                Vector3 vecToBullet = BulletScript.rb.position - PlayerScript.rb.position;
//                vecToBullet = vecToBullet.normalized;

//                PlayerScript.vel += vecToBullet * 3;

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
//            //着地したら立っている状態に移行
//            if (PlayerScript.isOnGround)
//            {
//                PlayerScript.mode = new PlayerStateOnGround(true);
//            }
//            else //そうでないなら空中
//            {
//                PlayerScript.mode = new PlayerStateMidair();
//            }
//        }

//        //ボールが触れたらスイング状態
//        if (BulletScript.isTouched)
//        {
//            if (BulletScript.swingEnd)
//            {
//                BulletScript.swingEnd = false;
//                PlayerScript.mode = new PlayerStateSwing_2();
//            }
//        }
//    }
//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Shot");
//    }
//}


/// <summary>
/// プレイヤーが空中にいる状態
/// クールタイムを経て、弾の発射ができる
/// </summary>
public class PlayerStateMidair : PlayerState
{
    private bool shotButton;  
    private bool OnceFallDownFlag;//急降下フラグ
    private void Init()
    {     
        PlayerScript.refState = EnumPlayerState.MIDAIR;
        PlayerScript.midairState = MidairState.NORMAL;
        shotButton = false;
        PlayerScript.canShotState = false;
        OnceFallDownFlag = false;

        BulletScript.InvisibleBullet();
    }

    public PlayerStateMidair(bool can_shot)//コンストラクタ
    {
        Init();
        PlayerScript.canShotState = can_shot;
    }

    public PlayerStateMidair(bool Jump_start, bool can_shot)//コンストラクタ
    {
        Init();
        PlayerScript.canShotState = can_shot;

        if (Jump_start)
        {
            PlayerScript.isOnGround = false;
            PlayerScript.vel.y += 80;
        }
    }

    public override void UpdateState()
    {
        BulletAdjust();

        if (PlayerScript.adjustLeftStick.x > 0.01f)
        {
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (PlayerScript.adjustLeftStick.x < -0.01f)
        {
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
        }

        if (Input.GetButtonDown("Button_R"))
        {
            if (PlayerScript.canShot)
            {
                shotButton = true;
            }
        }

        //急降下入力下？
        if (PlayerScript.sourceLeftStick.y < -0.7f && Mathf.Abs(PlayerScript.sourceLeftStick.x) < 0.3f)
        {
            //一度でも入力されたら永久に
            PlayerScript.midairState = MidairState.FALL;
        }

    }

    public override void Move()
    {
        //減衰S
        PlayerScript.vel.x *= PlayerScript.MIDAIR_FRICTION;
        if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * (fixedAdjust);
        }
        else if (PlayerScript.adjustLeftStick.x < -PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * -1 * (fixedAdjust);
        }

        //急降下中
        if(PlayerScript.midairState == MidairState.FALL)
        {
            //プレイヤーが上に向かっているときは早い
            if(PlayerScript.vel.y > 0.0f)
            {
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 2.0f * (fixedAdjust);
            }
            else　//下のときも少し早い
            {
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 1.5f * (fixedAdjust);
            }
            
            PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1 * 1.3f);
        }
        //自由落下
        else if(PlayerScript.midairState == MidairState.NORMAL)
        {
            PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * (fixedAdjust);
            PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
        }
    }


    public override void StateTransition()
    { 
        if (shotButton)
        {
            PlayerScript.midairState = MidairState.NONE;
            PlayerScript.mode = new PlayerStateShot_2(false);
        }

        //着地したら立っている状態に移行
        if (PlayerScript.isOnGround)
        {
            PlayerScript.midairState = MidairState.NONE;
            PlayerScript.mode = new PlayerStateOnGround();
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:MidAir");
    }
}



///// <summary>
///// 弾が天井でスイングしている状態
///// 移動速度等を保存
///// </summary>
//public class PlayerStateSwing_leftstick : PlayerState
//{
//    private bool finishFlag;
//    private bool releaseButton;
//    private Vector3 BulletPosition; //ボールの位置
   

//    private float betweenLength; //開始時二点間の距離(距離はスイングstate通して固定)
//    private Vector3 startPlayerVel;　　　　　　 //突入時velocity
//    private float startAngle;    //開始時の二点間アングル
//    private float endAngle;      //自動切り離しされる角度(start角度依存)
//    private float minAnglerVel;  //最低角速度（自動切り離し地点にいる時）
//    private float maxAnglerVel;　//最高角速度 (真下にプレイヤーが居る時）
//    private float nowAnglerVel;  //現在角速度

//    private List<Vector2> leftSticks = new List<Vector2>(); //swing開始からのleftStickを保持

//    public PlayerStateSwing_leftstick()  //コンストラクタ
//    {
//        BulletPosition = BulletScript.gameObject.transform.position;

//        //計算用情報格納
//        startPlayerVel = BulletScript.vel;
//        betweenLength = Vector3.Distance(Player.transform.position, BulletPosition);
//        betweenLength = Vector3.Distance(Player.transform.position, BulletPosition);
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);
//        startAngle = endAngle = degree;

//        PlayerScript.refState = EnumPlayerState.SWING;
//        PlayerScript.swingState = SwingState.TOUCHED;
//        PlayerScript.canShotState = false;
//        PlayerScript.endSwing = false;
//        PlayerScript.counterSwing = false;
//        finishFlag = false;
//        releaseButton = false;
//        BulletScript.rb.isKinematic = true;
//        PlayerScript.rb.velocity = Vector3.zero;
//        PlayerScript.vel = Vector3.zero;

//        CalculateStartVariable();
//    }

//    ~PlayerStateSwing_leftstick()
//    {
        
//    }

//    /// <summary>
//    /// 振り子制御用の各種変数を計算
//    /// </summary>
//    public void CalculateStartVariable()
//    {

//        //紐の長さとスピードから角速度を計算
//        float angler_velocity;
//        angler_velocity = (Mathf.Abs(startPlayerVel.x) * 6.0f);
//        angler_velocity /=  (betweenLength * 2.0f * Mathf.PI);
        
//        //範囲内に補正
//        angler_velocity = Mathf.Clamp(angler_velocity, 1.0f, 15.0f);

//        nowAnglerVel = maxAnglerVel = minAnglerVel = angler_velocity;

//        Debug.Log("AnglerVelocity: " + angler_velocity);

//        //切り離しアングルの計算
//        float diff_down = Mathf.Abs(startAngle - 180.0f); //真下と突入角の差
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle -= (diff_down + diff_down);
//            //開始点よりは高い位置にする
//            endAngle -= 10;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 90, 140);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle += (diff_down + diff_down);
//            //開始点よりは高い位置にする
//            endAngle += 10;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 220, 270);
//        }




//        //最低速は突入時プレイヤーvelocity
//        maxAnglerVel = minAnglerVel = angler_velocity;
//        //最高速度は突入角が大きいほど早い
//        maxAnglerVel += (diff_down / 90) * 2.0f; 
//    }

//    /// <summary>
//    /// 壁跳ね返り時の各種計算
//    /// </summary>
//    public void CalculateCounterVariable()
//    {
//        Debug.Log("counter:");

//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //プレイヤー回転処理
//            PlayerScript.dir = PlayerMoveDir.LEFT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //プレイヤー回転処理
//            PlayerScript.dir = PlayerMoveDir.RIGHT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
//        }

//        //切り離しアングルの計算
//        float diff_down = Mathf.Abs(endAngle - 180.0f); //真下と終了角の差
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle -= (diff_down + diff_down);
//            //開始点よりは高い位置にする
//            endAngle -= 20;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 90, 140);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle += (diff_down + diff_down);
//            //開始点より高い位置にする
//            endAngle += 20;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 220, 270);
//        }

//    }






//    /// <summary>
//    /// swing時の左スティックによって切り離し点を調整
//    /// </summary>
//    public void ReleasePointAlternate()
//    {
//        leftSticks.Add(PlayerScript.sourceLeftStick);


//    }

//    public override void UpdateState()
//    {
//        ReleasePointAlternate();

//        //if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
//        //{
//        //    BulletScript.rb.isKinematic = false;
//        //    shotButton = true;
//        //}

//        //ボールプレイヤー間の角度を求める
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);

//        //切り離し
//        if (PlayerScript.swingState == SwingState.TOUCHED)
//        {
//            if (PlayerScript.endSwing)
//            {
//                PlayerScript.endSwing = false;
//                PlayerScript.useVelocity = true;
//                BulletScript.ReturnBullet();
//                PlayerScript.swingState = SwingState.RELEASED;
//            }

//            if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//            {
//                if (degree < endAngle)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    PlayerScript.swingState = SwingState.RELEASED;

//                    //勢い追加
//                    //弾とプレイヤー間のベクトルに直行するベクトル
//                    Vector3 addVec = BulletPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, -90) * addVec;
//                    PlayerScript.vel += addVec * 35.0f;
//                }
//            }
//            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//            {
//                if (degree > endAngle)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    PlayerScript.swingState = SwingState.RELEASED;

//                    //勢い追加
//                    //弾とプレイヤー間のベクトルに直行するベクトル
//                    Vector3 addVec = BulletPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, 90) * addVec;
//                    PlayerScript.vel += addVec * 35.0f;
//                }
//            }
//        }

//        if (PlayerScript.counterSwing)
//        {
//            PlayerScript.counterSwing = false;
//            CalculateCounterVariable();
//        }
//    }

//    public override void Move()
//    {
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);
//        float deg180dif = Mathf.Abs(degree - 180);
//        switch (PlayerScript.swingState)
//        {
//            case SwingState.TOUCHED:
//                //角速度計算
//                float deg180Ratio = deg180dif / Mathf.Abs(endAngle - 180); //真下と最高到達点の比率
//                deg180Ratio = Mathf.Clamp01(deg180Ratio); //一応範囲内に補正
//                deg180Ratio = 1 - deg180Ratio; //真下を1,最高到達点を0とする

//                float easeDeg180Ratio = Easing.Linear(deg180Ratio, 1.0f, 0.0f, 1.0f);

//                nowAnglerVel = ((maxAnglerVel - minAnglerVel) * easeDeg180Ratio) + minAnglerVel;

//                //向きによって回転方向が違う
//                Quaternion angleAxis = Quaternion.Euler(Vector3.forward);
//                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//                {
//                    angleAxis = Quaternion.AngleAxis(nowAnglerVel, Vector3.forward);
//                }
//                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//                {
//                    angleAxis = Quaternion.AngleAxis(nowAnglerVel * -1, Vector3.forward);
//                }

//                Vector3 pos = Player.transform.position;

//                pos -= BulletPosition;
//                pos = angleAxis * pos;
//                pos += BulletPosition;
//                PlayerScript.transform.position = pos;
//                break;

//            case SwingState.RELEASED:
//                //自分へ弾を引き戻す
//                float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
//                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
//                vec = vec.normalized;
//                BulletScript.vel = vec * 100;

//                //距離が一定以下になったら弾を非アクティブ
//                if (interval < 4.0f)
//                {
//                    finishFlag = true;
                    
//                }
//                break;

//            default:
//                break;
//        }
//    }

//    public override void StateTransition()
//    {
//        if (finishFlag)
//        {
//            PlayerScript.swingState = SwingState.NONE;
//            PlayerScript.mode = new PlayerStateMidair(0.0f);
//        }
//    }

//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Swing");
//    }
//}



/// <summary>
/// 弾が天井でスイングしている状態
/// 移動速度等を保存
/// </summary>
public class PlayerStateSwing_R_Release : PlayerState
{
    private bool finishFlag;
    private bool releaseButton;
    private bool countreButton;
    private Vector3 BulletPosition; //ボールの位置
   
    private float betweenLength; //開始時二点間の距離(距離はスイングstate通して固定)
    private Vector3 startPlayerVel;　　　　　　 //突入時velocity
    private float startAngle;    //開始時の二点間アングル
    private float endAngle;      //自動切り離しされる角度(start角度依存)
    private float minAnglerVel;  //最低角速度（自動切り離し地点にいる時）
    private float maxAnglerVel;　//最高角速度 (真下にプレイヤーが居る時）
    private float nowAnglerVel;  //現在角速度

    Vector3 LastBtoP_Angle;  //最後に計測したバレット→プレイヤーの正規化Vector
    Vector3 AfterBtoP_Angle; //角速度計算後のバレット→プレイヤーの正規化Vector


    const float SWING_END_RATIO = 1.4f;

    public PlayerStateSwing_R_Release()  //コンストラクタ
    {
        BulletPosition = BulletScript.gameObject.transform.position;

        //計算用情報格納
        startPlayerVel = BulletScript.vel;
        betweenLength = Vector3.Distance(Player.transform.position, BulletPosition);
        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);
        startAngle = endAngle = degree;

        PlayerScript.refState = EnumPlayerState.SWING;
        PlayerScript.swingState = SwingState.TOUCHED;
        PlayerScript.canShotState = false;
        PlayerScript.endSwing = false;
        PlayerScript.hangingSwing = false;
        finishFlag = false;
        releaseButton = false;
        countreButton = false;
        BulletScript.rb.isKinematic = true;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel = Vector3.zero;

        CalculateStartVariable();
    }

    ~PlayerStateSwing_R_Release()
    {

    }

    /// <summary>
    /// 振り子制御用の各種変数を計算
    /// </summary>
    public void CalculateStartVariable()
    {

        //紐の長さとスピードから角速度を計算
        float angler_velocity;
        float tempY = Mathf.Min(startPlayerVel.y, 0.0f);
        angler_velocity = (Mathf.Abs(startPlayerVel.x) * 2.5f + Mathf.Abs(tempY) * 1.5f);
        angler_velocity /= (betweenLength * 2.0f * Mathf.PI);

        //範囲内に補正
        angler_velocity = Mathf.Clamp(angler_velocity, 1.0f, 3.0f);

        nowAnglerVel = maxAnglerVel = minAnglerVel = angler_velocity;

        Debug.Log("AnglerVelocity: " + angler_velocity);

        //バレットからプレイヤーのアングルを保存
        LastBtoP_Angle = AfterBtoP_Angle = (Player.transform.position - BulletPosition).normalized;

        //切り離しアングルの計算
        float diff_down = Mathf.Abs(startAngle - 180.0f); //真下と突入角の差
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            //終点自動切り離しポイントをy軸に対して対称にする
            endAngle -= (diff_down + diff_down);
            //開始点よりは高い位置にする
            endAngle -= 30;

            //範囲内に補正
            endAngle = Mathf.Clamp(endAngle, 90, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //終点自動切り離しポイントをy軸に対して対称にする
            endAngle += (diff_down + diff_down);
            //開始点よりは高い位置にする
            endAngle += 30;

            //範囲内に補正
            endAngle = Mathf.Clamp(endAngle, 220, 270);
        }

        //最低速は突入時プレイヤーvelocity
        maxAnglerVel = minAnglerVel = angler_velocity;
        //最高速度は突入角が大きいほど早い
        float velDiff = Mathf.Clamp01((diff_down / 90));
        maxAnglerVel += velDiff * 1.2f;
    }

    /// <summary>
    /// 壁跳ね返り時の各種計算
    /// </summary>
    public void CalculateCounterVariable()
    {
        Debug.Log("counter:");

        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            //プレイヤー回転処理
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //プレイヤー回転処理
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
        }

        //切り離しアングルの計算
        ReleaseAngleCalculate();
    }

    private void ReleaseAngleCalculate()
    {
        float diff_down = Mathf.Abs(endAngle - 180.0f); //真下と終了角の差
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            //終点自動切り離しポイントをy軸に対して対称にする
            endAngle -= (diff_down + diff_down);
            //開始点よりは高い位置にする
            endAngle -= 10;

            //範囲内に補正
            endAngle = Mathf.Clamp(endAngle, 90, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //終点自動切り離しポイントをy軸に対して対称にする
            endAngle += (diff_down + diff_down);
            //開始点より高い位置にする
            endAngle += 10;

            //範囲内に補正
            endAngle = Mathf.Clamp(endAngle, 220, 270);
        }
    }

    public void RotationPlayer()
    {

        switch (PlayerScript.swingState)
        {
            case SwingState.TOUCHED:
                float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);

                Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;
                Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);

                Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //補正用クオータニオン

                quaternion *= adjustQua;

                if(PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    if(degree < 180)
                    {
                        quaternion *= Quaternion.Euler(0, 180, 0);
                    }
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    if (degree > 180)
                    {
                        quaternion *= Quaternion.Euler(0, 180, 0);
                    }
                }

                PlayerScript.rb.rotation = quaternion;
                break;

            case SwingState.RELEASED:
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 90, 0));
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, -90, 0));
                }
                break;
        }


    }

    public void InputButton()
    {
        if(PlayerScript.swingState != SwingState.RELEASED)
        {
            if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
            {

                releaseButton = true;
            }

        }

        if (PlayerScript.swingState == SwingState.HANGING)
        {
            if (Input.GetButtonDown("Jump"))
            {
                countreButton = true;
                Debug.Log("Press Jump");
            }
        }
        
    }





    public override void UpdateState()
    {
        //切り離し入力
        InputButton();
        
        //弾の場所更新
        BulletPosition = BulletScript.rb.position;

        //ボールプレイヤー間の角度を求める
        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);

        //切り離し
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
                if ((degree < endAngle) || (releaseButton == true))
                {
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;

                    //勢い追加
                    //弾とプレイヤー間のベクトルに直行するベクトル
                    Vector3 addVec = BulletPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, -90) * addVec;


                    float mutipleVec = (nowAnglerVel / 20.0f) + 1.0f;

                    PlayerScript.vel += addVec * mutipleVec * 40.0f * SWING_END_RATIO;
                }
            }
            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
            {
                if ((degree > endAngle) || (releaseButton == true))
                {
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;

                    //勢い追加
                    //弾とプレイヤー間のベクトルに直行するベクトル
                    Vector3 addVec = BulletPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, 90) * addVec;


                    float mutipleVec = (nowAnglerVel / 20.0f) + 1.0f;

                    PlayerScript.vel += addVec * mutipleVec * 40.0f * SWING_END_RATIO;
                }
            }
        }

       
    }

    public override void Move()
    {
        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position); //バレットからプレイヤーベクトル
        float deg180dif = Mathf.Abs(degree - 180); //プレイヤーからベクトル

        RotationPlayer();

        switch (PlayerScript.swingState)
        {
            case SwingState.TOUCHED:
                //ぶら下がり処理
                if (PlayerScript.hangingSwing)
                {
                    PlayerScript.swingState = SwingState.HANGING;
                    PlayerScript.hangingSwing = false;
                }
               
                //短くする処理
                if (PlayerScript.shortSwing.isShort)
                {
                    betweenLength = PlayerScript.shortSwing.length;
                    PlayerScript.shortSwing.isShort = false;
                }

                //角速度計算
                float deg180Ratio = deg180dif / Mathf.Abs(endAngle - 180); //真下と最高到達点の比率
                deg180Ratio = Mathf.Clamp01(deg180Ratio); //一応範囲内に補正
                deg180Ratio = 1 - deg180Ratio; //真下を1,最高到達点を0とする

                float easeDeg180Ratio = Easing.Linear(deg180Ratio, 1.0f, 0.0f, 1.0f);

                nowAnglerVel = ((maxAnglerVel - minAnglerVel) * easeDeg180Ratio) + minAnglerVel;//角速度（量）

                //前回計算後のAfterAngleを持ってくる
                LastBtoP_Angle = AfterBtoP_Angle;

                //↑を角速度分回す
                //向きによって回転方向が違う
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    AfterBtoP_Angle = Quaternion.Euler(0, 0, nowAnglerVel * 1) * LastBtoP_Angle;

                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    AfterBtoP_Angle = Quaternion.Euler(0, 0, nowAnglerVel * -1) * LastBtoP_Angle;

                }

                //ボール座標 ＋ 正規化した回転後アングル ＊ 長さ
                Vector3 pos = BulletPosition + (AfterBtoP_Angle.normalized) * betweenLength;

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

                }
                break;

            case SwingState.HANGING:
                //反転処理
                if (countreButton)
                { 

                    PlayerScript.swingState = SwingState.TOUCHED;
                    CalculateCounterVariable();
                    countreButton = false;
                }

                if (releaseButton)
                {
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;

                    PlayerScript.vel = Vector3.zero;
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
            PlayerScript.mode = new PlayerStateMidair(true);
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Swing");
    }
}

/// <summary>
/// レール移動時のクラス
/// </summary>
public class PlayerStateRail : PlayerState
{
    public PlayerStateRail()
    {
        PlayerScript.refState = EnumPlayerState.RAILING;
        PlayerScript.canShotState = false; //撃てない
        PlayerScript.vel = Vector3.zero;   //速度0
        PlayerScript.addVel = Vector3.zero;

        BulletScript.rb.velocity = Vector3.zero;
        BulletScript.vel = Vector3.zero;
        BulletScript.StopVelChange = true;
    }

    public override void UpdateState()
    {
        //キー入力不可
    }

    public override void Move()
    {
        //移動なし
    }

    public override void StateTransition()
    {
        //終わったらステート
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
        PlayerScript.canShotState = false;
        PlayerScript.vel = Vector3.zero;
        PlayerScript.addVel = Vector3.zero;



        BulletScript.rb.velocity = Vector3.zero;
        BulletScript.vel = Vector3.zero;
        BulletScript.StopVelChange = true;
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



///// <summary>
///// 弾が天井でスイングしている状態
///// </summary>
//public class PlayerStateSwing : PlayerState
//{

//    enum SwingMode
//    {
//        Touced,   //捕まっている状態
//        Rereased, //切り離した状態
//    }

//    SwingMode swingMode;
//    private bool finishFlag;
//    private bool shotButton;
//    private Vector3 ballPosition;
//    private const float SWING_ANGLER_VELOCITY = 2.7f; //振り子角速度 ←const値ではなく前のstateのvelocityで可変が良さそう
//    private const float SWING_REREASE_ANGLE = 140.0f; //振り子がのなす角が一定以上になったら強制解除

//    public PlayerStateSwing()//コンストラクタ
//    {
//        PlayerScript.refState = EnumPlayerState.SWING;
//        PlayerScript.canShotState = false;
//        finishFlag = false;
//        swingMode = SwingMode.Touced;
//        shotButton = false;
//        BulletScript.rb.isKinematic = true;
//        ballPosition = BulletScript.gameObject.transform.position;
//        PlayerScript.rb.velocity = Vector3.zero;
//        PlayerScript.vel = Vector3.zero;
//    }

//    public override void UpdateState()
//    {
//        //if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
//        //{
//        //    BulletScript.rb.isKinematic = false;
//        //    shotButton = true;
//        //}

//        //ボールプレイヤー間の角度を求める
//        Vector3 dt = Player.transform.position - ballPosition;
//        float rad = Mathf.Atan2(dt.x, dt.y);
//        float degree = rad * Mathf.Rad2Deg;
//        if (degree < 0)//上方向を基準に時計回りに0~360の値に補正
//        {
//            degree += 360;
//        }

//        //一定角度以上で切り離し
//        if (swingMode == SwingMode.Touced)
//        {
//            if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//            {
//                if (degree < SWING_REREASE_ANGLE)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    swingMode = SwingMode.Rereased;

//                    //勢い追加
//                    //弾とプレイヤー間のベクトルに直行するベクトル
//                    Vector3 addVec = ballPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, -110) * addVec;
//                    PlayerScript.vel += addVec * 35.0f;
//                }
//            }
//            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//            {
//                if (degree > 360 - SWING_REREASE_ANGLE)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    swingMode = SwingMode.Rereased;

//                    //勢い追加
//                    //弾とプレイヤー間のベクトルに直行するベクトル
//                    Vector3 addVec = ballPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, 110) * addVec;
//                    PlayerScript.vel += addVec * 35.0f;
//                }
//            }
//        }
//    }

//    public override void Move()
//    {
//        ballPosition = BulletScript.transform.position;

//        switch (swingMode)
//        {
//            case SwingMode.Touced:
//                //向きによって回転方向が違う
//                Quaternion angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY, Vector3.forward);
//                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//                {
//                    angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY, Vector3.forward);
//                }
//                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//                {
//                    angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY * -1, Vector3.forward);
//                }

//                Vector3 pos = Player.transform.position;

//                pos -= ballPosition;
//                pos = angleAxis * pos;
//                pos += ballPosition;
//                PlayerScript.transform.position = pos;
//                break;

//            case SwingMode.Rereased:
//                //自分へ弾を引き戻す
//                float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
//                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
//                vec = vec.normalized;

//                BulletScript.vel = vec * 100;

//                //距離が一定以下になったら弾を非アクティブ

//                if (interval < 4.0f)
//                {
//                    finishFlag = true;
//                }

//                break;

//            default:
//                break;
//        }
//    }

//    public override void StateTransition()
//    {
//        if (finishFlag)
//        {
//            PlayerScript.mode = new PlayerStateMidair(0.0f);
//        }
//    }

//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Swing");
//    }
//}