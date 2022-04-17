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
    NONE,      //空中状態ではない
    NORMAL,   //通常時
    FALL, 　  //急降下
}


/// <summary>
/// スイング時の細かな状態
/// </summary>
public enum SwingState
{
    NONE,      //スイング状態ではない
    TOUCHED,   //捕まっている状態
    RELEASED,  //切り離した状態
    HANGING,   //ぶら下がり状態
}

/// <summary>
/// 弾射出時の細かな状態
/// </summary>
public enum ShotState
{
    NONE,       //弾射出されていない
    GO,         //引っ張られずに飛んでいる
    STRAINED,  //プレイヤーを引っ張りながら飛んでいる
    RETURN,     //弾がプレイヤーに戻って終了
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
}

public struct ShortenSwing {
    public bool isShort;
    public float length;
}


public class PlayerMain : MonoBehaviour
{
    [System.NonSerialized] public Rigidbody rb;      // [System.NonSerialized] インスペクタ上で表示させたくない
    [System.NonSerialized] public static PlayerMain instance;
    [System.NonSerialized] public Animator animator;
    public BulletMain BulletScript;
    public PlayerState mode;                         // ステート
    private RaycastHit footHit;                      // Ge
    private float killVeltimer;                      //壁ぶつかりで速度を殺すときのチャタリング防止用

    [SerializeField, Tooltip("チェックが入っていたら入力分割")] private bool SplitStick;        //これにチェックが入っていたら分割
    [SerializeField, Tooltip("スティック方向を補正する（要素数で分割）\n値は上が0で時計回りに増加。0~360の範囲")] private float[] AdjustAngles;   //スティック方向を補正する（要素数で分割）値は上が0で時計回りに増加。0~360の範囲
    [SerializeField, Tooltip("チェックが入っていたらボタン離しで発射")] public bool ReleaseMode;
    [SerializeField, Tooltip("チェックが入っていたら振り子自動で切り離し")] public bool AutoRelease;
    [SerializeField, Tooltip("チェックが入っていたら振り子時紐が長くなる")] public bool LongRope;

    [System.NonSerialized] public float colliderRadius = 1.42f;   //接地判定用ray半径
    [System.NonSerialized] public float coliderDistance = 1.8f; //
                                                                 //
    [System.NonSerialized] public float HcolliderRadius = 2.0f;   //頭判定用ray半径
    [System.NonSerialized] public float HcoliderDistance = 0.6f; //頭判定用ray中心点から頭までのオフセット

    [SerializeField] public  float SwingcolliderRadius = 1.5f;   //スイングスライド判定用ray半径
    [SerializeField] public  float SwingcoliderDistance = 1.75f; //スイングスライドray中心点から頭までのオフセット

    //----------↓プレイヤー物理挙動関連の定数↓----------------------
    [Range(0.1f, 1.0f), Tooltip("左右移動開始のスティックしきい値")] public float  LATERAL_MOVE_THRESHORD;   // 走り左右移動時の左スティックしきい値
    [Tooltip("走り最高速度")] public float                      MAX_RUN_SPEED;           // 走り最高速度
    [Tooltip("走り最低速度（下回ったら速度0）")] public float   MIN_RUN_SPEED;　　　　　 // 走り最低速度（下回ったら速度0）
    [Tooltip("走り一フレームで上がるスピード")] public float    ADD_RUN_SPEED;           // 走り一フレームで上がるスピード
    [Tooltip("振り子切り離し時加算")] public float 　　　　　　 RELEASE_FORCE;
    [Tooltip("落下速度制限")] public float                      MAX_FALL_SPEED;          // 重力による最低速度
    [Tooltip("空中にいるときの重力加速度")] public float        FALL_GRAVITY;            // プレイヤーが空中にいるときの重力加速度
    [Tooltip("引っ張られているときの重力加速度")] public float  STRAINED_GRAVITY;　　　　// プレイヤーが引っ張られているときの重力加速度
    [Range(0.1f, 1.0f), Tooltip("地上速度減衰率")] public float　RUN_FRICTION;            // 走りの減衰率


    [Tooltip("空中一フレームで上がるスピード")] public float                      ADD_MIDAIR_SPEED;        // 空中一秒間で上がるスピード
    [Range(0.1f, 1.0f), Tooltip("空中速度減衰率")] public float                   MIDAIR_FRICTION;         // 空中の速度減衰率
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
    [ReadOnly, Tooltip("地面と接触しているか")] public bool isOnGround;                          // 地面に触れているか（onCollisionで変更）
    [ReadOnly, Tooltip("打てる可能性があるか")] public bool canShotState;                             // 打てる状態か
    [ReadOnly, Tooltip("スティックの入力が一定以上あるか：ある場合は打てる")] public bool stickCanShotRange;
    [ReadOnly, Tooltip("壁の近くにいる場合は撃てない")] public bool CanShotColBlock;                           // スティック入力の先に壁が
    [ReadOnly, Tooltip("最終的に打てるかどうか")] public bool canShot;                             // 打てる状態か
    [ReadOnly, Tooltip("velocityでの移動かposition直接変更による移動か")] public bool useVelocity;                         // 移動がvelocityか直接position変更かステートによっては直接位置を変更する時があるため
    [ReadOnly, Tooltip("強制的に弾を戻させるフラグ")] public bool forciblyReturnBulletFlag;            // 強制的に弾を戻させるフラグ
    [ReadOnly, Tooltip("強制的に弾を戻させるときに現在の速度を保存するか")] public bool forciblyReturnSaveVelocity;
    [ReadOnly, Tooltip("スイング強制終了用")] public bool endSwing;
    [ReadOnly, Tooltip("スイング短くする用")] public bool SlideSwing;
    [ReadOnly, Tooltip("スイングぶら下がり用")] public bool hangingSwing;


    void Awake()
    {
        instance = this;
        PlayerState.PlayerScript = this;  //PlayerState側で参照できるようにする
        PlayerState.Player = gameObject;

        //出現位置の設定
        if (CheckPointManager.isTouchCheckPos() == true) {
            transform.position = CheckPointManager.GetCheckPointPos();
         }
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
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
        canShotState = true;
        stickCanShotRange = false;
        CanShotColBlock = false;
        canShot = false;
        isOnGround = true;
        useVelocity = true;

        forciblyReturnBulletFlag = false;
        forciblyReturnSaveVelocity = false;

        endSwing = false;
        SlideSwing = false;
        
        hangingSwing = false;
        killVeltimer = 0.0f;

        Ray footray = new Ray(rb.position, Vector3.down);
        Physics.SphereCast(footray, colliderRadius, out footHit, coliderDistance, LayerMask.GetMask("Platform"));



        rb.sleepThreshold = -1; //リジッドボディが静止していてもonCollision系を呼ばせたい

        mode = new PlayerStateOnGround(); //初期ステート

        if (mode != null)
        {
            mode.UpdateState();
            //mode.Animation();
            mode.StateTransition();
            mode.Move();
        }
    }

    private void Update()
    {
        if (GameStateManager.GetGameState() == GAME_STATE.PLAY && FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
        {
            InputStick();
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
                addVel *= 0.96f;
            }
            else
            {
                addVel = Vector3.zero;
            }

            killVeltimer = Mathf.Clamp(killVeltimer += Time.fixedDeltaTime, 0.0f, 2.0f);

#if UNITY_EDITOR //unityエディター上ではデバッグを行う（ビルド時には無視される）
                //mode.DebugMessage();
#endif
            
        }
    }

    public RaycastHit getFootHit()
    {
        return footHit;
    }

    private void InputStick()
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
        }

        //スティックの角度を求める
        float rad = Mathf.Atan2(adjustLeftStick.x, adjustLeftStick.y);
        float degree = rad * Mathf.Rad2Deg;
        if (degree < 0)//上方向を基準に時計回りに0~360の値に補正
        {
            degree += 360;
        }
        float angle = 0.0f;

        //AjustAngles内の一番近い値にスティックを補正
        float minDif = 9999.0f;
        float dif;

        for (int i = 0; i < AdjustAngles.Length; i++)
        {
            dif = Mathf.Abs(AdjustAngles[i] - degree);
            if (dif < minDif)
            {
                minDif = dif;
                angle = AdjustAngles[i];
            }

            dif = Mathf.Abs(AdjustAngles[i] + 360 - degree);
            if (dif < minDif)
            {
                minDif = dif;
                angle = AdjustAngles[i];
            }
        }

        //角度を読める値に調整
        if (angle > 180)
        {
            angle -= 360;
        }
        angle *= -1;
        angle += 90;
        rad = angle * Mathf.Deg2Rad;

        //角度からベクトルにする
        Vector3 vec = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        vec = vec.normalized;

        //分割処理
        if (SplitStick)
        {
            if (stickCanShotRange)
            {
                adjustLeftStick = vec;
            }
        }
        
    }

    /// <summary>
    /// 弾をうてる状態なのかをチェックする
    /// </summary>
    private void CheckCanShot()
    {
        //デバッグログ
        Vector3 StartPos;
        StartPos = rb.position;
        StartPos.y += 1.0f;

        RaycastHit hit;
        if (Physics.Raycast(StartPos, adjustLeftStick, out hit, 3.0f))
        {
            if (hit.collider.CompareTag("Platform"))
            {
                CanShotColBlock = false;
            }
            else
            {
                CanShotColBlock = true;
            }
        }
        else
        {
            CanShotColBlock = true;
        }
        StartPos.z += 2.0f;
        Debug.DrawRay(StartPos, adjustLeftStick * 3.0f, Color.red);


        //最終的に打てるかの決定
        if(canShotState && stickCanShotRange && CanShotColBlock)
        {
            canShot = true;
        }
        else
        {
            canShot = false;
        }
    } 

    /// <summary>
    /// 強制的に弾を引き戻させる
    /// </summary>
    /// <param name="saveVelocity">true:引き戻し時にもとのベロシティを保持
    ///　false:引き戻し時にもとのベロシティを殺す
    /// </param>
    public void ForciblyReturnBullet(bool saveVelocity)
    {
        forciblyReturnBulletFlag = true;
        forciblyReturnSaveVelocity = saveVelocity;
    }


    private void OnCollisionEnter(Collision collision)
    {
        Aspect asp = DetectAspect.DetectionAspect(collision.contacts[0].normal);


        

        //ショット中に壁にあたったときの処理
        if(refState == EnumPlayerState.SHOT)
        {
            if (collision.gameObject.CompareTag("Platform"))
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
                            ForciblyReturnBullet(false);
                        }
                        break;

                    case ShotState.GO:
                    case ShotState.RETURN:
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
                                vel.x *= 0.2f;
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
                if (collision.gameObject.CompareTag("Platform"))
                {
                    if (dir == PlayerMoveDir.RIGHT && asp == Aspect.LEFT)
                    {
                        hangingSwing = true;
                    }
                    else if (dir == PlayerMoveDir.LEFT && asp == Aspect.RIGHT)
                    {
                        hangingSwing = true;
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
                        hangingSwing = true;
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
                }
            //}
        }

        
        //空中で壁にぶつかったとき速度をなくす
        if (refState == EnumPlayerState.MIDAIR)
        {
            if (killVeltimer > 0.1f)
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
                        vel.x *= 0.2f;
                        vel.y *= 0.0f;
                        break;
                }
                killVeltimer = 0.0f;
            }
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


    //接地判定を計算
    private void CheckMidAir()
    {
        Ray ray = new Ray(rb.position, Vector3.down);
        if (isOnGround)
        {
            if (Physics.SphereCast(ray, colliderRadius, coliderDistance, LayerMask.GetMask("Platform")) == false)
            {
                isOnGround = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        //接地ray
        Ray footRay = new Ray(rb.position, Vector3.down);
        if (isOnGround)
        {
            Gizmos.color = Color.magenta;
        }
        else
        {
            Gizmos.color = Color.cyan;
        }
        Gizmos.DrawWireSphere(footRay.origin + (Vector3.down * (coliderDistance)), colliderRadius);


        //頭
        //if (refState == EnumPlayerState.SHOT)
        //{
        //    if (shotState == ShotState.STRAINED)
        //    {
                Vector3 vecToPlayer = BulletScript.rb.position - rb.position;
                vecToPlayer = vecToPlayer.normalized;

                Ray headRay = new Ray(rb.position, vecToPlayer);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(headRay.origin + (vecToPlayer * (HcoliderDistance)), HcolliderRadius);
        //    }
        //}

        //スイングスライド足元
        //if(refState == EnumPlayerState.SWING)
        //{
        //    if(swingState == SwingState.TOUCHED) 
        //    {
                Vector3 vecToPlayerR = rb.position - BulletScript.rb.position;
                vecToPlayerR = vecToPlayerR.normalized;

                Ray Ray = new Ray(rb.position, vecToPlayerR);
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(Ray.origin + (vecToPlayerR * SwingcoliderDistance), SwingcolliderRadius);
        //    }
        //}   
    }

}
