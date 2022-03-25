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
/// スイング時の細かな状態
/// </summary>
public enum SwingState
{
    NONE,      //スイング状態ではない
    TOUCHED,   //捕まっている状態
    RELEASED,  //切り離した状態
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
    DEATH,     //死亡状態
}


public class PlayerMain : MonoBehaviour
{
    [System.NonSerialized] public Rigidbody rb;      // [System.NonSerialized] インスペクタ上で表示させたくない
    [System.NonSerialized] public static PlayerMain instance;
    public GameObject Bullet_2;
    public PlayerState mode;                         // ステート


    [SerializeField, Tooltip("チェックが入っていたら入力分割")] private bool SplitStick;        //これにチェックが入っていたら分割
    [SerializeField, Tooltip("スティック方向を補正する（要素数で分割）\n値は上が0で時計回りに増加。0~360の範囲")] private float[] AdjustAngles;   //スティック方向を補正する（要素数で分割）値は上が0で時計回りに増加。0~360の範囲



    //----------↓プレイヤー物理挙動関連の定数↓----------------------
    [Range(0.1f, 1.0f), Tooltip("左右移動開始のスティックしきい値")] public float  LATERAL_MOVE_THRESHORD;   // 走り左右移動時の左スティックしきい値
    [Tooltip("走り最高速度")] public float                      MAX_RUN_SPEED;           // 走り最高速度
    [Tooltip("走り最低速度（下回ったら速度0）")] public float   MIN_RUN_SPEED;　　　　　 // 走り最低速度（下回ったら速度0）
    [Tooltip("走り一フレームで上がるスピード")] public float    ADD_RUN_SPEED;           // 走り一フレームで上がるスピード
    [Tooltip("落下速度制限")] public float                      MAX_FALL_SPEED;          // 重力による最低速度
    [Tooltip("空中にいるときの重力加速度")] public float        FALL_GRAVITY;            // プレイヤーが空中にいるときの重力加速度
    [Tooltip("引っ張られているときの重力加速度")] public float  STRAINED_GRAVITY;　　　　// プレイヤーが引っ張られているときの重力加速度
    [Range(0.1f, 1.0f), Tooltip("地上速度減衰率")] public float　RUN_FRICTION;            // 走りの減衰率


    [Tooltip("空中一フレームで上がるスピード")] public float                      ADD_MIDAIR_SPEED;        // 空中一秒間で上がるスピード
    [Range(0.1f, 1.0f), Tooltip("空中速度減衰率")] public float  MIDAIR_FRICTION;         // 空中の速度減衰率
    [Tooltip("空中で再び球が打てるようになる時間")]public float                      BULLET_RECAST_TIME;      // 空中で再び球が打てるようになる時間（秒）
    //----------プレイヤー物理挙動関連の定数終わり----------------------

    [ Header("[以下実行時変数確認用：変更不可]")]

    [ReadOnly, Tooltip("現在のステート")] public EnumPlayerState refState;                //ステート確認用(modeの中に入っている派生クラスで値が変わる)
    [ReadOnly, Tooltip("地上時の細かなステート")] public OnGroundState onGroundState;                //ステート確認用(modeの中に入っている派生クラスで値が変わる)
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
    [ReadOnly, Tooltip("スイング跳ね返り用")] public bool counterSwing;
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
        
    }

    private void Start()
    {
        refState = EnumPlayerState.ON_GROUND;
        onGroundState = OnGroundState.NONE;
        shotState = ShotState.NONE;
        swingState = SwingState.NONE;
        dir = PlayerMoveDir.RIGHT;        //向き初期位置
        vel = Vector3.zero;
        addVel = Vector3.zero;
        floorVel = Vector3.zero;
        sourceLeftStick = adjustLeftStick = new Vector2(0.0f, 0.0f);
        canShotState = true;
        stickCanShotRange = false;
        CanShotColBlock = false;
        canShot = false;
        isOnGround = false;
        useVelocity = true;

        forciblyReturnBulletFlag = false;
        forciblyReturnSaveVelocity = false;

        endSwing = false;
        counterSwing = false;

        rb.sleepThreshold = -1; //リジッドボディが静止していてもonCollision系を呼ばせたい

        mode = new PlayerStateOnGround(); //初期ステート
    }

    private void Update()
    {
        InputStick();
        CheckCanShot();
        CheckMidAir();

        if (mode != null)
        {
            mode.UpdateState();
            mode.StateTransition();
        }
        else
        {
            Debug.LogError("STATE == NULL");
        }
    }

    private void FixedUpdate()
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


        if (Mathf.Abs(addVel.magnitude) > 10.0f)
        {
            addVel *= 0.96f;
        }
        else
        {
            addVel = Vector3.zero;
        }

#if UNITY_EDITOR //unityエディター上ではデバッグを行う（ビルド時には無視される）
        //mode.DebugMessage();
#endif
    }

    private void LateUpdate()
    {
        
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
        StartPos = transform.position;
        StartPos.y += 1.0f;

        RaycastHit hit;
        if (Physics.Raycast(StartPos, adjustLeftStick, out hit, 3.0f))
        {
            if (hit.collider.CompareTag("Platform"))
            {
                CanShotColBlock = false;
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

        //空中で壁にぶつかったとき速度をなくす
        if (refState == EnumPlayerState.MIDAIR)
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (Mathf.Abs(collision.contacts[i].point.x - transform.position.x) > 0.3f)
                {
                    vel.x *= 0.2f;
                    if(vel.y > 1.0f)
                    {
                        vel.y = 0;
                    }
                    Debug.Log("KILL");
                }
            }
        }

        //swing中に壁にぶつかったら消す
        if (refState == EnumPlayerState.SWING)
        {
            if (swingState == SwingState.TOUCHED)
            {
                if(dir == PlayerMoveDir.RIGHT && asp == Aspect.LEFT)
                {
                    counterSwing = true;
                }
                else if (dir == PlayerMoveDir.LEFT && asp == Aspect.RIGHT)
                {
                    counterSwing = true;
                }
                else
                {
                    endSwing = true;
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    { 
        //接触点のうち、一つでも足元があれば着地判定
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].point.y < transform.position.y - 0.6f)
            {
                isOnGround = true;
            }
        }

        //FOLLOW中に壁に当たると上に補正
        if (refState == EnumPlayerState.SHOT)
        {
            if (shotState == ShotState.FOLLOW)
            {
                Aspect asp = DetectAspect.DetectionAspect(collision.contacts[0].normal);

                if (asp == Aspect.LEFT || asp == Aspect.RIGHT)
                {
                    Vector3 tempPos = transform.position;
                    tempPos.y += 0.7f;
                    transform.position = tempPos;
                }
            }
        }

        //swing中に壁にぶつかったら消す
        if (refState == EnumPlayerState.SWING)
        {
            endSwing = true;
        }
    }

    //空中にいるかを判定する
    //斜めの床がなければ必要なさそう
    private void CheckMidAir()
    {
        if (isOnGround)
        {
            Ray downRay = new Ray(rb.position, Vector3.down);
            if (Physics.Raycast(downRay, 1.2f) == false)
            {
                isOnGround = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isOnGround)
        {
            isOnGround = false;
        }
    }
}
