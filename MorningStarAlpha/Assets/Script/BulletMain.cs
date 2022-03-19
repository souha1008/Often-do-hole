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
    public bool isStrained; //弾が戻されて引っ張られている状態
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
        isStrained = false;
        Vector3 vec = PlayerScript.leftStick.normalized;

        //弾の初期化
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        isTouched = false;

        vel += vec * BULLET_SPEED;
        //rb.AddForce(vec * BULLET_SPEED, ForceMode.VelocityChange);
    }

    void FixedUpdate()
    {
        if (isStrained == false)
        {
            if (rb.velocity.y > BULLET_MAXFALLSPEED * -1)
            {
                vel += Vector3.down * 0.4f;
               // rb.AddForce(Vector3.down * 3.8f, ForceMode.Acceleration);
            }
        }
        rb.velocity = vel;
    }

    public void ReturnBullet()
    {
        isStrained = true;
        rb.isKinematic = false;
        GetComponent<Collider>().isTrigger = true;
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (onceFlag == false)
        {
            onceFlag = true;
            //collsion先のtagで場合分け
            string tag = collision.gameObject.tag;
            switch (tag)
            {
                case "Platform":
                    isTouched = true;

                    //物理運動を停止
                    rb.isKinematic = true;
                    rb.velocity = Vector3.zero;
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
