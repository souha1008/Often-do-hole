using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの向いている向き
/// </summary>
public enum PlayerMoveDir
{
    LEFT,
    RIGHT,
}

public enum OnGroundState {
    NONE,
    NORMAL, //通常時
    SLIDE,  //滑っている
}

/// <summary>
/// 空中時の細かな状態
/// </summary>
public enum MidairState
{
    NONE,     //空中状態ではない
    NORMAL,   //通常時
    BOOST,    //ブースト
}


/// <summary>
/// スイング時の細かな状態
/// </summary>
public enum SwingState
{
    NONE,      //スイング状態ではない
    TOUCHED,   //捕まっている状態
}

/// <summary>
/// 弾射出時の細かな状態
/// </summary>
public enum ShotState
{
    NONE,       //弾射出されていない
    GO,         //引っ張られずに飛んでいる
    STRAINED,  //プレイヤーを引っ張りながら飛んでいる
    FOLLOW,     //弾に勢いよく飛んでいき終了
}

/// <summary>
/// プレイヤーのステート
/// ほかオブジェクトの読み取り用
/// </summary>
public enum EnumPlayerState 
{ 
    ON_GROUND, //地上にいる
    SHOT,      //弾を撃っている状態
    MIDAIR,　　//空中にいて弾を撃っていない
    SWING,     //振り子状態
    RAILING,   //レール状態
    NOCKBACK,  //ノックバック状態
    DEATH,     //死亡状態
    STAN,      //スタン状態
    CLEAR,     //クリア状態
}

/// <summary>
///アニメーションコントローラー用の文字列をindexにして格納
/// </summary>
public struct AnimHash{
    public int onGround;
    public int isRunning;
    public int isShot;
    public int isSwing;
    public int wallKick;
    public int NockBack;
    public int isBoost;
    public int shotdirType;
    public int RunSpeed;
    public int rareWaitTrigger;
    public int rareWaitType;
    public int IsDead;
}


public class PlayerMain : MonoBehaviour
{
    [System.NonSerialized] public Rigidbody rb;      // [System.NonSerialized] インスペクタ上で表示させたくない
    [System.NonSerialized] public static PlayerMain instance;
    [System.NonSerialized] public Animator animator;
    [System.NonSerialized] public AnimHash animHash;
    [System.NonSerialized] public GameObject[] animBullet = new GameObject[3];

    public BulletMain BulletScript;
    public PlayerState mode;                         // ステート
    private RaycastHit footHit;                      // 下に当たっているものの情報格納

    [System.NonSerialized] public float colliderRadius = 1.65f;   //接地判定用ray半径
    [System.NonSerialized] public float coliderDistance = 1.8f; //
                                                                 //
    [System.NonSerialized] public float HcolliderRadius = 2.0f;   //頭判定用ray半径
    [System.NonSerialized] public float HcoliderDistance = 0.6f; //頭判定用ray中心点から頭までのオフセット

    [System.NonSerialized] public  float SwingcolliderRadius = 1.5f;   //スイングスライド判定用ray半径
    [System.NonSerialized] public  float SwingcoliderDistance = 1.75f; //スイングスライドray中心点から頭までのオフセット

    //----------↓プレイヤー物理挙動関連の定数↓----------------------
    [System.NonSerialized, Tooltip("左右移動開始のスティックしきい値")] public float LATERAL_MOVE_THRESHORD = 0.2f;   // 走り左右移動時の左スティックしきい値
    [System.NonSerialized, Tooltip("走り最高速度")] public float                      MAX_RUN_SPEED = 30.0f;           // 走り最高速度
    [System.NonSerialized, Tooltip("走り最低速度（下回ったら速度0）")] public float   MIN_RUN_SPEED = 2.0f;　　　　　 // 走り最低速度（下回ったら速度0）
    [System.NonSerialized, Tooltip("走り一フレームで上がるスピード")] public float    ADD_RUN_SPEED = 4.0f;           // 走り一フレームで上がるスピード
    [System.NonSerialized, Tooltip("落下速度制限")] public float                      MAX_FALL_SPEED = 70.0f;          // 重力による最低速度
    [System.NonSerialized, Tooltip("空中にいるときの重力加速度")] public float        FALL_GRAVITY = 2.7f;            // プレイヤーが空中にいるときの重力加速度
    [System.NonSerialized, Tooltip("引っ張られているときの重力加速度")] public float  STRAINED_GRAVITY = 2.5f;　　　　// プレイヤーが引っ張られているときの重力加速度
    [System.NonSerialized, Tooltip("地上速度減衰率")] public float　RUN_FRICTION = 0.92f;            // 走りの減衰率


    [System.NonSerialized, Tooltip("空中一フレームで上がるスピード")] public float                      ADD_MIDAIR_SPEED = 1.5f;        // 空中一秒間で上がるスピード
    [System.NonSerialized, Tooltip("空中速度減衰率")] public float                   MIDAIR_FRICTION = 0.982f;         // 空中の速度減衰率
    //----------プレイヤー物理挙動関連の定数終わり----------------------

    [ Header("[以下実行時変数確認用：変更不可]")]

    [ReadOnly, Tooltip("現在のステート")] public EnumPlayerState refState;                //ステート確認用(modeの中に入っている派生クラスで値が変わる)
    [ReadOnly, Tooltip("地上時の細かなステート")] public OnGroundState onGroundState;                //ステート確認用(modeの中に入っている派生クラスで値が変わる)
    [ReadOnly, Tooltip("空中時の細かなステート")] public MidairState midairState;
    [ReadOnly, Tooltip("ショット状態の細かなステート")] public ShotState shotState;
    [ReadOnly, Tooltip("swing状態の細かなstate")] public SwingState swingState;
    [ReadOnly, Tooltip("プレイヤーの向き")] public PlayerMoveDir dir;
    [ReadOnly, Tooltip("プレイヤーの速度:入力によるもの")] public Vector3 vel;                              // 移動速度(inspector上で確認)
    [ReadOnly, Tooltip("プレイヤーの速度:ギミックでの反発によるもの")] public Vector3 addVel;                           // ギミック等で追加される速度
    [ReadOnly, Tooltip("プレイヤーの速度:移動床によるもの")] public Vector3 floorVel;                         // 動く床等でのベロシティ
    [ReadOnly, Tooltip("スティック入力角（調整前）")] public Vector2 sourceLeftStick;                        // 左スティック  
    [ReadOnly, Tooltip("スティック入力角（調整後）")] public Vector2 adjustLeftStick;                        // 左スティック
    [ReadOnly, Tooltip("最後のな入力角")] public float oldStickAngle;                        // 左スティック  
    [ReadOnly, Tooltip("地面と接触しているか")] public bool isOnGround;                          // 地面に触れているか（onCollisionで変更）
    [ReadOnly, Tooltip("打てる可能性があるか")] public bool canShotState;                             // 打てる状態か
    [ReadOnly, Tooltip("スティックの入力が一定以上あるか：ある場合は打てる")] public bool stickCanShotRange;
    [ReadOnly, Tooltip("壁の近くにいる場合は撃てない")] public bool CanShotColBlock;                           // スティック入力の先に壁が
    [ReadOnly, Tooltip("最終的に打てるかどうか")] public bool canShot;                             // 打てる状態か
    [ReadOnly, Tooltip("velocityでの移動かposition直接変更による移動か")] public bool useVelocity;                         // 移動がvelocityか直接position変更かステートによっては直接位置を変更する時があるため
    [ReadOnly, Tooltip("強制的に弾を戻させるフラグ")] public bool forciblyRleaseFlag;            // 強制的に弾を戻させるフラグ
    [ReadOnly, Tooltip("強制的に弾を戻させるときに現在の速度を保存するか")] public bool forciblyReleaseSaveVelocity;
    [ReadOnly, Tooltip("強制的に弾についていくときのフラグ")] public bool forciblyFollowFlag;
    [ReadOnly, Tooltip("強制的に弾についていくときにvelocityの向きを弾方向に変換する")] public bool forciblyFollowVelToward;
    [ReadOnly, Tooltip("強制的にswing開始するフラグ")] public bool forciblySwingFlag;
    [ReadOnly, Tooltip("強制的にswing開始するフラグ")] public bool forciblySwingNextFollow;
    [ReadOnly, Tooltip("強制的にswing開始するフラグ")] public bool forciblySwingSaveVelocity;
    [ReadOnly, Tooltip("スイング強制終了用")] public bool endSwing;
    [ReadOnly, Tooltip("スイング短くする用")] public bool SlideSwing;
    [ReadOnly, Tooltip("スイングぶら下がり用")] public bool conuterSwing;
    [ReadOnly, Tooltip("発射回復")] public bool recoverBullet;
    public float GameSpeed = 1.0f;


    void Awake()
    {
        instance = this;
        PlayerState.PlayerScript = this;  //PlayerState側で参照できるようにする
        PlayerState.Player = gameObject;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        gameObject.tag = "Player";

        animBullet[0] = transform.Find("anchor_fix3:group12/anchor_fix3:body/anchor_fix3:Anchor_body/anchor_fix3:anchor_body").gameObject;
        animBullet[1] = transform.Find("anchor_fix3:group12/anchor_fix3:body/anchor_fix3:Anchor_body/anchor_fix3:anchor_L_needle").gameObject;
        animBullet[2] = transform.Find("anchor_fix3:group12/anchor_fix3:body/anchor_fix3:Anchor_body/anchor_fix3:anchor_R_needle").gameObject;

        Time.timeScale = GameSpeed;
    }

    private void Start()
    {
        ////出現位置の設定
        //transform.position = CheckPointManager.Instance.GetCheckPointPos();

        refState = EnumPlayerState.ON_GROUND;
        onGroundState = OnGroundState.NONE;
        midairState = MidairState.NONE;
        shotState = ShotState.NONE;
        swingState = SwingState.NONE;
        dir = PlayerMoveDir.RIGHT;        //向き初期位置
        rb.rotation = Quaternion.Euler(0, 90, 0);
        vel = Vector3.zero;
        addVel = Vector3.zero;
        floorVel = Vector3.zero;
        sourceLeftStick = adjustLeftStick = new Vector2(0.0f, 0.0f);
        oldStickAngle = -1;
        canShotState = true;
        stickCanShotRange = false;
        CanShotColBlock = false;
        canShot = false;
        isOnGround = true;
        useVelocity = true;

        ClearModeTransitionFlag();
        SetAnimHash();


        endSwing = false;
        SlideSwing = false;
        
        conuterSwing = false;

        Ray footray = new Ray(rb.position, Vector3.down);
        Physics.SphereCast(footray, colliderRadius, out footHit, coliderDistance, LayerMask.GetMask("Platform"));


        rb.sleepThreshold = -1; //リジッドボディが静止していてもonCollision系を呼ばせたい

        mode = new PlayerStateOnGround(); //初期ステート

        if (mode != null)
        {
            mode.UpdateState();
            mode.Animation();
            mode.StateTransition();
            mode.Move();
        }
    }

    public void VisibleAnimBullet(bool on_off)
    {
        for (int i = 0; i < 3; i++) 
        {
            animBullet[i].SetActive(on_off);
        }
    }

    private void Update()
    {
        if (GameStateManager.GetGameState() == GAME_STATE.PLAY && FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
        {
            InputStick_Fixed();
            //InputStick();
            CheckCanShot();
            CheckMidAir();

            if (mode != null)
            {
                mode.UpdateState();
                mode.Animation();
                mode.StateTransition(); 
            }
            else
            {
                Debug.LogError("STATE == NULL");
            }
            
        }
    }

    private void FixedUpdate()
    {
        if (GameStateManager.GetGameState() == GAME_STATE.PLAY && FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
        {
            if (mode != null)
            {
                mode.Move();
            }
            else
            {
                Debug.LogError("STATE == NULL");
            }
            rb.velocity = Vector3.zero;
            if (useVelocity)
            {
                rb.velocity = vel;
            }
            rb.velocity += addVel;
            rb.velocity += floorVel;


            Vector3 resetZo = rb.position;
            resetZo.z = 0.0f;
            rb.position = resetZo;

            if (Mathf.Abs(addVel.magnitude) > 10.0f)
            {
                addVel *= 0.98f;
            }
            else
            {
                addVel = Vector3.zero;
            }

#if UNITY_EDITOR //unityエディター上ではデバッグを行う（ビルド時には無視される）
                //mode.DebugMessage();
#endif
            
        }
    }

    private void SetAnimHash()
    {
        animHash.onGround = Animator.StringToHash("onGround");
        animHash.isRunning = Animator.StringToHash("isRunning");
        animHash.isShot = Animator.StringToHash("isShot");
        animHash.isSwing = Animator.StringToHash("isSwing");
        animHash.wallKick = Animator.StringToHash("wallKick");
        animHash.NockBack = Animator.StringToHash("NockBack");
        animHash.isBoost = Animator.StringToHash("isBoost");
        animHash.shotdirType = Animator.StringToHash("shotdirType");
        animHash.RunSpeed = Animator.StringToHash("RunSpeed");
        animHash.rareWaitTrigger = Animator.StringToHash("rareWaitTrigger");
        animHash.rareWaitType = Animator.StringToHash("rareWaitType");
        animHash.IsDead = Animator.StringToHash("IsDead");
    }   

    public void AnimVariableReset()
    {
        animator.SetBool(animHash.isShot, false);
        animator.SetBool(animHash.isSwing, false);
        animator.SetBool(animHash.isBoost, false);
        animator.SetBool(animHash.IsDead, false);
    }

    public void AnimTriggerReset()
    {
        animator.ResetTrigger(animHash.wallKick);
        animator.ResetTrigger(animHash.NockBack);
        animator.ResetTrigger(animHash.rareWaitTrigger);
    }

    public void ResetAnimation()
    {
        AnimVariableReset();
        AnimTriggerReset();
    }

    public RaycastHit getFootHit()
    {
        return footHit;
    }

    void InputStick_Fixed()
    {
        //初期化
        sourceLeftStick = adjustLeftStick = Vector2.zero;

        //入力取得
        sourceLeftStick.x = adjustLeftStick.x = Input.GetAxis("Horizontal");
        sourceLeftStick.y = adjustLeftStick.y = Input.GetAxis("Vertical");

        //スティックの入力が一定以上ない場合は撃てない
        if (Mathf.Abs(sourceLeftStick.magnitude) > 0.7f)
        {
            stickCanShotRange = true;
        }
        else
        {
            stickCanShotRange = false;
            oldStickAngle = -1;
        }


        float angle = CalculationScript.TwoPointAngle360(Vector3.zero, sourceLeftStick);
        float adjustAngle = 0;
        //angleを固定(25.75.285.335)
        

        if(angle == 0)
        {
            if (oldStickAngle == -1)
            {
                if (dir == PlayerMoveDir.RIGHT)
                {
                    adjustAngle = 25;
                    oldStickAngle = 25;
                }
                else if (dir == PlayerMoveDir.LEFT)
                {
                    adjustAngle = 335;
                    oldStickAngle = 335;
                }
            }
            else
            {
                adjustAngle = oldStickAngle;
            }
        }
        else if(angle < 5)
        {
            if(oldStickAngle == -1)
            {
                adjustAngle = 25;
                oldStickAngle = 25;
            }
            else
            {
                adjustAngle = oldStickAngle;
            }
        }
        else if(angle < 45)
        {
            adjustAngle = 25;
            oldStickAngle = 25;
        }
        else if (angle <= 120)
        {
            adjustAngle = 75;
            oldStickAngle = 75;
        }
        else if (angle < 240)
        {
            oldStickAngle = -1;
            adjustAngle = oldStickAngle;
        }
        else if (angle < 315)
        {
            adjustAngle = 285;
            oldStickAngle = 285;
        }
        else if (angle <= 355)
        {
            adjustAngle = 335;
            oldStickAngle = 335;
        }
        else
        {
            if (oldStickAngle == -1)
            {
                adjustAngle = 335;
                oldStickAngle = 335;
            }
            else
            {
                adjustAngle = oldStickAngle;
            }
        }
    
        if(oldStickAngle < 0)
        {
            stickCanShotRange = false;
        }


        //角度を読める値に調整
        if (adjustAngle > 180)
        {
            adjustAngle -= 360;
        }
        adjustAngle *= -1;
        adjustAngle += 90;
        float rad = adjustAngle * Mathf.Deg2Rad;

        //角度からベクトルにする
        Vector3 vec = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        vec = vec.normalized;

        if (stickCanShotRange)
        {
            adjustLeftStick = (Vector2)vec;
        }
        else
        {
            adjustLeftStick = Vector2.zero;
        }
    }

    //private void InputStick()
    //{
    //    //初期化
    //    sourceLeftStick = adjustLeftStick = Vector2.zero;

    //    //入力取得
    //    sourceLeftStick.x = adjustLeftStick.x = Input.GetAxis("Horizontal");
    //    sourceLeftStick.y = adjustLeftStick.y = Input.GetAxis("Vertical");

    //    //スティックの入力が一定以上ない場合は撃てない
    //    if (Mathf.Abs(sourceLeftStick.magnitude) > 0.7f)
    //    {
    //        stickCanShotRange = true;
    //    }
    //    else
    //    {
    //        stickCanShotRange = false;
    //    }

    //    //スティックの角度を求める
    //    float rad = Mathf.Atan2(adjustLeftStick.x, adjustLeftStick.y);
    //    float degree = rad * Mathf.Rad2Deg;
    //    if (degree < 0)//上方向を基準に時計回りに0~360の値に補正
    //    {
    //        degree += 360;
    //    }
    //    float angle = 0.0f;

    //    //AjustAngles内の一番近い値にスティックを補正
    //    float minDif = 9999.0f;
    //    float dif;

    //    for (int i = 0; i < AdjustAngles.Length; i++)
    //    {
    //        dif = Mathf.Abs(AdjustAngles[i] - degree);
    //        if (dif < minDif)
    //        {
    //            minDif = dif;
    //            angle = AdjustAngles[i];
    //        }

    //        dif = Mathf.Abs(AdjustAngles[i] + 360 - degree);
    //        if (dif < minDif)
    //        {
    //            minDif = dif;
    //            angle = AdjustAngles[i];
    //        }
    //    }

    //    //角度を読める値に調整
    //    if (angle > 180)
    //    {
    //        angle -= 360;
    //    }
    //    angle *= -1;
    //    angle += 90;
    //    rad = angle * Mathf.Deg2Rad;

    //    //角度からベクトルにする
    //    Vector3 vec = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
    //    vec = vec.normalized;

    //    //分割処理
    //    if (SplitStick)
    //    {
    //        if (stickCanShotRange)
    //        {
    //            adjustLeftStick = (Vector2)vec;
    //        }
    //        else
    //        {
    //            adjustLeftStick = Vector2.zero;
    //        }
    //    }
    //}

    /// <summary>
    /// 弾をうてる状態なのかをチェックする
    /// </summary>
    private void CheckCanShot()
    {

        //最終的に打てるかの決定
        if (canShotState && stickCanShotRange && BulletScript.CanShotFlag)
        {
            canShot = true;
        }
        else
        {
            canShot = false;
        }
    } 

    public void ClearModeTransitionFlag()
    {
        forciblyRleaseFlag = false;
        forciblyFollowFlag = false;
        forciblySwingFlag = false;
        forciblyReleaseSaveVelocity = false;
        forciblyFollowVelToward = false;
        forciblySwingNextFollow = false;
        forciblySwingSaveVelocity = false;
    }

    /// <summary>
    /// 弾を引き戻させる
    /// </summary>
    /// <param name="saveVelocity">true:引き戻し時にもとのベロシティを保持
    ///　false:引き戻し時にもとのベロシティを殺す
    /// </param>
    public void ForciblyReleaseMode(bool saveVelocity)
    {
        ClearModeTransitionFlag();
        if (refState == EnumPlayerState.SHOT)
        {
            forciblyRleaseFlag = true;
            forciblyReleaseSaveVelocity = saveVelocity;

        }
        else if(refState == EnumPlayerState.SWING)
        {
            endSwing = true;
            forciblySwingSaveVelocity = saveVelocity;
        }
    }

    public void ForciblyFollowMode(bool velTowardBullet)
    {
        ClearModeTransitionFlag();
        if (refState == EnumPlayerState.SHOT)
        {
            forciblyFollowFlag = true;
            forciblyFollowVelToward = velTowardBullet;
        }
    }

    public void ForciblySwingMode(bool nextFollow)
    {
        ClearModeTransitionFlag();
        if (refState == EnumPlayerState.SHOT)
        {
            forciblySwingFlag = true;
            forciblySwingNextFollow = nextFollow;
        }
    }

    public void RecoverBullet()
    {
        recoverBullet = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Aspect asp = DetectAspect.DetectionAspect(collision.contacts[0].normal);


        //空中で壁にぶつかったとき速度をなくす
        if (refState == EnumPlayerState.MIDAIR)
        {
                switch (asp)
                {
                    case Aspect.LEFT:
                    case Aspect.RIGHT:
                        vel.x *= 0.0f;
                        if (vel.y > 1.0f)
                        {
                            vel.y *= 0.0f;
                        }

                        break;

                    case Aspect.DOWN:
                        vel.x *= 1.0f;
                        vel.y *= 0.0f;

                        
                        break;
                }
        }


        //ショット中に壁にあたったときの処理
        if (refState == EnumPlayerState.SHOT)
        {
            if (collision.gameObject.CompareTag("Platform") || 
                collision.gameObject.CompareTag("Conveyor_Tate") || collision.gameObject.CompareTag("Conveyor_Yoko"))
            {
                switch (shotState)
                {
                    case ShotState.STRAINED: //紐張り詰め
                        //RAYに移行
                        //if (isOnGround == false)
                        //{
                        //    ForciblyReturnBullet(true);
                        //}

                        break;

                    case ShotState.FOLLOW: //紐に引っ張られ
                        if (asp == Aspect.DOWN)
                        {
                            ForciblyReleaseMode(false);
                        }
                        break;

                    case ShotState.GO:
                        //勢い殺す
                        switch (asp)
                        {
                            case Aspect.LEFT:
                            case Aspect.RIGHT:
                                vel.x *= 0.0f;
                                if (vel.y > 1.0f)
                                {
                                    vel.y *= 0.1f;
                                }
                                break;

                            case Aspect.DOWN:
                                vel.x *= 1.0f;
                                vel.y *= 0.0f;
                                break;
                        }
                        break;
                }
            }
        }


        //swing中に壁にぶつかったときの処理(反転、強制終了)
        if (refState == EnumPlayerState.SWING)
        {
            if (swingState == SwingState.TOUCHED)
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
                {
                    if (dir == PlayerMoveDir.RIGHT && asp == Aspect.LEFT)
                    {
                        conuterSwing = true;
                    }
                    else if (dir == PlayerMoveDir.LEFT && asp == Aspect.RIGHT)
                    {
                        conuterSwing = true;
                    }
                    else if (dir == PlayerMoveDir.RIGHT && asp == Aspect.RIGHT)
                    {
                        Debug.Log("collision Platform : Wall Jump");
                    }
                    else if (dir == PlayerMoveDir.LEFT && asp == Aspect.LEFT)
                    {
                        Debug.Log("collision Platform : Wall Jump");
                    }
                    else　if (asp == Aspect.DOWN)
                    {
                        Debug.Log("collision Platform down: swing end");
                        conuterSwing = true;
                    }

                }
            }
        }

    }



    private void OnCollisionStay(Collision collision)
    {
        Aspect asp = DetectAspect.DetectionAspect(collision.contacts[0].normal);
        Collider col = GetComponent<Collider>();

        //footHit格納用
        Ray footray = new Ray(rb.position, Vector3.down);
        Physics.SphereCast(footray, colliderRadius, out footHit, coliderDistance, LayerMask.GetMask("Platform"));


        //着地判定
        if (isOnGround == false)
        {
            //if (vel.y < 0)
            //{
                Ray ray = new Ray(rb.position, Vector3.down);
                if (Physics.SphereCast(ray, colliderRadius, coliderDistance, LayerMask.GetMask("Platform")))
                {
                    isOnGround = true;
                    animator.SetBool(animHash.onGround, true);

                    EffectManager.instance.landEffect(collision.contacts[0].point);
                 }
            //}
        }

        //swing中に壁にぶつかったときの処理(スライド)
        if (refState == EnumPlayerState.SWING)
        {
            if (swingState == SwingState.TOUCHED)
            {
                if (collision.gameObject.CompareTag("Platform"))
                {
                   if (asp == Aspect.UP)
                    {
                        Vector3 vecToPlayerR = rb.position - BulletScript.rb.position;
                        vecToPlayerR = vecToPlayerR.normalized;
                        Ray footRay = new Ray(rb.position, vecToPlayerR);

                        if (Physics.SphereCast(footRay, SwingcolliderRadius, SwingcoliderDistance, LayerMask.GetMask("Platform")))
                        {
                            Debug.Log("collision Platform : slide continue");
                            SlideSwing = true;
                        }
                        else
                        {
                            Debug.Log("collision Platform : swing end");
                            endSwing = true;
                        }
                    }
                }
            }
        }


        //FOLLOW中に壁に当たると上に補正
        if (refState == EnumPlayerState.SHOT)
        {
            if (shotState == ShotState.FOLLOW)
            {
                if (asp == Aspect.LEFT || asp == Aspect.RIGHT)
                {
                    Vector3 tempPos = transform.position;
                    tempPos.y += 0.7f;
                    transform.position = tempPos;
                }
            }
        }

        
    }

    /// <summary>
    /// トリガー類はすぐに遷移しない場合リセット
    /// </summary>
    private void OnAnimatorMove()
    {
        AnimTriggerReset();
    }


    //接地判定を計算
    private void CheckMidAir()
    {
        Ray ray = new Ray(rb.position, Vector3.down);
        if (isOnGround)
        {
            if (Physics.SphereCast(ray, colliderRadius, coliderDistance, LayerMask.GetMask("Platform")) == false)
            {
                isOnGround = false;
                animator.SetBool(animHash.onGround, false);
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    //接地ray
    //    Ray footRay = new Ray(rb.position, Vector3.down);

    //        if (isOnGround)
    //        {
    //            Gizmos.color = Color.magenta;
    //        }
    //        else
    //        {
    //            Gizmos.color = Color.cyan;
    //        }
    //        Gizmos.DrawWireSphere(footRay.origin + (Vector3.down * (coliderDistance)), colliderRadius);


    //        //頭
    //        //if (refState == EnumPlayerState.SHOT)
    //        //{
    //        //    if (shotState == ShotState.STRAINED)
    //        //    {
    //        Vector3 vecToPlayer = BulletScript.rb.position - rb.position;
    //        vecToPlayer = vecToPlayer.normalized;

    //        Ray headRay = new Ray(rb.position, vecToPlayer);
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireSphere(headRay.origin + (vecToPlayer * (HcoliderDistance)), HcolliderRadius);
    //        //    }
    //        //}

    //        //スイングスライド足元
    //        //if(refState == EnumPlayerState.SWING)
    //        //{
    //        //    if(swingState == SwingState.TOUCHED) 
    //        //    {
    //        Vector3 vecToPlayerR = rb.position - BulletScript.rb.position;
    //        vecToPlayerR = vecToPlayerR.normalized;

    //        Ray Ray = new Ray(rb.position, vecToPlayerR);
    //        Gizmos.color = Color.black;
    //        Gizmos.DrawWireSphere(Ray.origin + (vecToPlayerR * SwingcoliderDistance), SwingcolliderRadius);
    //        //    }
    //        //}   
    //    
    //}

}
