using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMain : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody rb;
    private GameObject Player;
    private PlayerMain PlayerScript;
    public Vector3 vel;
    public bool isTouched; //弾がなにかに触れたか
    public bool onceFlag; //一回の発射に付き接触が起こるのは一回
    public bool StopDownVel; //弾が戻されて引っ張られている状態
    public bool swingEnd;
    public bool followEnd;

   //弾関係定数
   [SerializeField] private float BULLET_SPEED; //弾の初速   
    [SerializeField] private float BULLET_START_DISTANCE; //弾の発射位置
    [SerializeField] public float BULLET_ROPE_LENGTH; //紐の長さ
    private float BULLET_MAXFALLSPEED = 35.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Player = GameObject.Find("Player");
        PlayerScript = Player.GetComponent<PlayerMain>();
      
        onceFlag = false;
        StopDownVel = false;
        swingEnd = false;
        followEnd = false;
        Vector3 vec = PlayerScript.leftStick.normalized;

        //弾の初期化
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        isTouched = false;

        vel += vec * BULLET_SPEED;
    }

    void FixedUpdate()
    {
        if (StopDownVel == false)
        {
            vel += Vector3.down * PlayerScript.STRAINED_GRAVITY;
            Mathf.Max(vel.y, BULLET_MAXFALLSPEED * -1);
        }
        rb.velocity = vel;
    }

    public void ReturnBullet()
    {
        StopDownVel = true;
        rb.isKinematic = false;
        GetComponent<Collider>().isTrigger = true;
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
    }

    public void FollowedPlayer()
    {
        StopDownVel = true;
        rb.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
    }

    /// <summary>
    ///　面向き
    /// </summary>
    enum Aspect
    {
        UP,        //上面
        DOWN,      //下面
        LEFT,      //左面
        RIGHT,　　 //右面
        INVALID,   //例外
    }

    /// <summary>
    /// 法線ベクトルによって面の向きを取得
    /// </summary>
    /// <param name="vec">法線</param>
    /// <returns></returns>
    private Aspect DetetAspect(Vector3 vec)
    {
        Aspect returnAspect = Aspect.INVALID;
        if (Mathf.Abs(vec.y) > 0.5f) //y成分が大きいので縦向き
        {
            if (vec.y > 0)
            {
                returnAspect = Aspect.UP;
            }
            else
            {
                returnAspect = Aspect.DOWN;
            }
        }
        else if (Mathf.Abs(vec.x) > 0.5f) //x成分が大きいので横向き
        {
            if (vec.x > 0)
            {
                returnAspect = Aspect.RIGHT;
            }
            else
            {
                returnAspect = Aspect.LEFT;
            }
        }
        else
        {
            returnAspect = Aspect.INVALID;
            Debug.LogError("接触面の法線が斜めの可能性があります");
        }

        return returnAspect;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Aspect colAspect = Aspect.INVALID;
        colAspect = DetetAspect(collision.contacts[0].normal); //接触点の法線ベクトル

        if (onceFlag == false)
        {
            onceFlag = true;
            //collsion先のtagで場合分け
            string tag = collision.gameObject.tag;
            switch (tag)
            {
                case "Platform":
                    isTouched = true;
                    rb.isKinematic = true;
                    rb.velocity = Vector3.zero;
                    if (colAspect == Aspect.UP)
                    {
                        //FOLLOW状態に移行
                        followEnd = true;
                        Debug.Log("col");
                    }
                    else
                    {
                        //SWING状態に移行
                        swingEnd = true;
                    }

                    break;

                case "Iron":
                    PlayerScript.ForciblyReturnBullet(true);
                    break;

                case "Player":
                    break;

                default:
                    break;
            }
        }
    }
}
