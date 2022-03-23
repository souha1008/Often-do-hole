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
    public bool StopVelChange; //弾が戻されて引っ張られている状態
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
        StopVelChange = false;
        swingEnd = false;
        followEnd = false;
        Vector3 vec = PlayerScript.leftStick.normalized;

        //弾の初期化
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        isTouched = false;

        vel += vec * BULLET_SPEED;

        //進行方向と同じ向きに投げる場合威力補正
        if (Mathf.Sign(vec.x) == Mathf.Sign(PlayerScript.vel.x))
        {
            vel += PlayerScript.vel *= 0.6f;
        }

        //if(Mathf.Abs(PlayerScript.vel.x) > 20.0f)
        //{
        //    vel *= 0.8f;
        //}
    }

    void FixedUpdate()
    {
        if (ReferenceEquals(Player, null) == false)
        {
            if (StopVelChange == false)
            {
                vel += Vector3.down * PlayerScript.STRAINED_GRAVITY;
                Mathf.Max(vel.y, BULLET_MAXFALLSPEED * -1);
            }
            rb.velocity = vel;
        }
    }

    public void ReturnBullet()
    {
        if (ReferenceEquals(Player, null) == false)
        {
            StopVelChange = true;
            rb.isKinematic = false;
            GetComponent<Collider>().isTrigger = true;
            rb.velocity = Vector3.zero;
            vel = Vector3.zero;
        }
    }

    public void FollowedPlayer()
    {
        if (ReferenceEquals(Player, null) == false)
        {
            StopVelChange = true;
            rb.isKinematic = true;
            GetComponent<Collider>().isTrigger = true;
            rb.velocity = Vector3.zero;
            vel = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ReferenceEquals(Player, null) == false)
        {
            Aspect colAspect = Aspect.INVALID;
            colAspect = DetectAspect.DetetAspect(collision.contacts[0].normal); //接触点の法線ベクトル

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

    private void OnDestroy()
    {
        Player = null;
        PlayerScript = null;
    }
}
