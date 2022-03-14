//#define SPLIT_LEFTSTICK

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerMoveDir
{
    LEFT,
    RIGHT,
}

public class PlayerMain : MonoBehaviour
{
    [System.NonSerialized] public Rigidbody rb;      // [System.NonSerialized] インスペクタ上で表示させたくない
    public GameObject BulletPrefab;
    public PlayerState mode;                         // ステート
    [System.NonSerialized] public GameObject Bullet = null;
    public HingeJoint hinge = null;
    public PlayerMoveDir dir;
    public Vector2 vel;                              // 移動速度(inspector上で確認)
    public Vector2 leftStick;                        // 左スティック
    public bool canShot;                             // 打てる状態か

    [SerializeField] private float[] AdjustAngles;                      //スティック方向を補正する（要素数で分割）値は上が0で時計回りに増加。0~360の範囲

    //----------↓プレイヤー物理挙動関連の定数↓----------------------
    [Range(0.1f, 1.0f)] public float  LATERAL_MOVE_THRESHORD;  // 走り左右移動時の左スティックしきい値
    public float                      MAX_RUN_SPEED;           // 走り最高速度
    public float                      MIN_RUN_SPEED;　　　　　 // 走り最低速度（下回ったら速度0）
    public float                      ADD_RUN_SPEED;           // 走り一秒間で上がるスピード
    [Range(0.1f, 1.0f)] public float　RUN_FRICTION;            // 走りの減衰率


    public float                      ADD_MIDAIR_SPEED;        // 空中一秒間で上がるスピード
    [Range(0.1f, 1.0f)] public float  MIDAIR_FRICTION;         // 空中の速度減衰率
    public float                      BULLET_RECAST_TIME;      // 空中で再び球が打てるようになる時間（秒）
    //----------プレイヤー物理挙動関連の定数終わり----------------------

    

    void Awake()
    {
        PlayerState.PlayerScript = this;  //PlayerState側で参照できるようにする
        PlayerState.Player = gameObject;

        rb = GetComponent<Rigidbody>();
        mode = new PlayerStateStand();
        Bullet = null;
        hinge = null;
        dir = PlayerMoveDir.RIGHT;
        vel = new Vector2(0.0f ,0.0f);
        leftStick = new Vector2(0.0f, 0.0f);
        canShot = false;
    }

    private void Update()
    {
        InputStick();
        mode.UpdateState();
        mode.StateTransition();
    }

    private void FixedUpdate()
    {
        mode.Move();
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
        leftStick = new Vector2(0, 0);

        //入力取得
        leftStick.x = Input.GetAxis("Horizontal");
        leftStick.y = Input.GetAxis("Vertical");


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


        if (leftStick.sqrMagnitude > 0.8f)
        {
            canShot = true;
        }
        else
        {
            canShot = false;
        }


#if SPLIT_LEFTSTICK
        //分割処理
        if (canShot)
        {
            leftStick = vec;
        }
#endif
    }
}
