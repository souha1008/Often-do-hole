//#define SPLIT_LEFTSTICK

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerMoveDir
{
    LEFT,
    RIGHT,
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
    public GameObject BulletPrefab;
    public PlayerState mode;                         // ステート
    public EnumPlayerState refState;                //ステート確認用(modeの中に入っている派生クラスで値が変わる)
    public GameObject Bullet = null;
    public HingeJoint hinge = null;
    public PlayerMoveDir dir;
    public Vector3 vel;                              // 移動速度(inspector上で確認)
    public Vector3 addVel;                         　//ギミック等で追加される機能
    public Vector2 leftStick;                        // 左スティック
    public bool stickCanShotRange;                             // 打てる状態か
    public bool canShot;                             // 打てる状態か
    public bool isOnGraund;                          // 地面に触れているか（onCollisionで変更）

    [SerializeField] private float[] AdjustAngles;                      //スティック方向を補正する（要素数で分割）値は上が0で時計回りに増加。0~360の範囲



    //----------↓プレイヤー物理挙動関連の定数↓----------------------
    [Range(0.1f, 1.0f)] public float  LATERAL_MOVE_THRESHORD;  // 走り左右移動時の左スティックしきい値
    public float                      MAX_RUN_SPEED;           // 走り最高速度
    public float                      MIN_RUN_SPEED;　　　　　 // 走り最低速度（下回ったら速度0）
    public float                      ADD_RUN_SPEED;           // 走り一秒間で上がるスピード
    public float                      MAX_FALL_SPEED;          // 重力による最低速度
    [Range(0.1f, 1.0f)] public float　RUN_FRICTION;            // 走りの減衰率


    public float                      ADD_MIDAIR_SPEED;        // 空中一秒間で上がるスピード
    [Range(0.1f, 1.0f)] public float  MIDAIR_FRICTION;         // 空中の速度減衰率
    public float                      BULLET_RECAST_TIME;      // 空中で再び球が打てるようになる時間（秒）
    //----------プレイヤー物理挙動関連の定数終わり----------------------

    

    void Awake()
    {
        instance = this;
        PlayerState.PlayerScript = this;  //PlayerState側で参照できるようにする
        PlayerState.Player = gameObject;

        rb = GetComponent<Rigidbody>();
        mode = new PlayerStateOnGround(); //初期ステート
        refState = EnumPlayerState.ON_GROUND;
        Bullet = null;
        hinge = null;　　　　　　　　　　 
        dir = PlayerMoveDir.RIGHT;        //向き初期位置
        vel = Vector3.zero;
        addVel = Vector3.zero;
        leftStick = new Vector2(0.0f, 0.0f);
        stickCanShotRange = false;
        canShot = true;
        isOnGraund = false;

        rb.sleepThreshold = -1; //リジッドボディが静止していてもonCollision系を呼ばせたい
    }

    private void Update()
    {
        InputStick();
        MidAirCheck();
        mode.UpdateState();
        mode.StateTransition();
    }

    private void FixedUpdate()
    {
        mode.Move();
        rb.velocity = vel;
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
        leftStick = Vector2.zero;

        //入力取得
        leftStick.x = Input.GetAxis("Horizontal");
        leftStick.y = Input.GetAxis("Vertical");

        //スティックの入力が一定以上ない場合は撃てない
        if (leftStick.sqrMagnitude > 0.8f)
        {
            stickCanShotRange = true;
        }
        else
        {
            stickCanShotRange = false;
        }



        //スティックの角度を求める
        float rad = Mathf.Atan2(leftStick.x, leftStick.y);
        float degree = rad * Mathf.Rad2Deg;
        if (degree < 0)//上方向を基準に時計回りに0~360の値に補正
        {
            degree += 360;
        }

        float angle = 0.0f;

        //AjustAngles内の一番近い値にスティックを補正
        float minDif = 9999.0f;
        float dif;

        for (int i = 0;i < AdjustAngles.Length; i++)
        {
             dif = Mathf.Abs(AdjustAngles[i] - degree);
            if(dif< minDif)
            {
                minDif = dif;
                angle = AdjustAngles[i];
            }
        }

        Debug.Log(angle);

        //角度を読める値に調整
        if(angle > 180)
        {
            angle -= 360;
        }
        angle *= -1;
        angle += 90;
        rad = angle * Mathf.Deg2Rad;

        //角度からベクトルにする
        Vector3 vec = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        vec = vec.normalized;

#if SPLIT_LEFTSTICK
        //分割処理
        if (canShot)
        {
            leftStick = vec;
        }
#endif
    }

    private void OnCollisionEnter(Collision collision)
    {
        //空中で壁にぶつかったとき速度をなくす
        if(refState == EnumPlayerState.MIDAIR)
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (Mathf.Abs(collision.contacts[i].point.x - transform.position.x) > 0.2f)
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
    }


    private void OnCollisionStay(Collision collision)
    { 
        //接触点のうち、一つでも足元があれば着地判定
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].point.y < transform.position.y - 0.6f)
            {
                isOnGraund = true;
            }
        }   
    }

    //空中にいるかを判定する
    //斜めの床がなければ必要なさそう
    private void MidAirCheck()
    {
        if (isOnGraund)
        {
            Ray downRay = new Ray(rb.position, Vector3.down);
            if (Physics.Raycast(downRay, 1.2f) == false)
            {
                isOnGraund = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isOnGraund)
        {
            isOnGraund = false;
        }
    }
}
