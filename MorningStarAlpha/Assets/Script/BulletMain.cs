using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMain : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody rb;
    private GameObject Player;
    private PlayerMain PlayerScript;
    public bool isTouched; //弾がなにかに触れたか
    public bool onceFlag; //一回の発射に付き接触が起こるのは一回
    //弾関係定数
    [SerializeField] private float BULLET_SPEED; //弾の初速   
    [SerializeField] private float BULLET_START_DISTANCE; //弾の発射位置
    [SerializeField] public float BULLET_ROPE_LENGTH; //紐の長さ


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Player = GameObject.Find("Player");
        PlayerScript = Player.GetComponent<PlayerMain>();
        onceFlag = false;
        Vector3 vec = PlayerScript.leftStick.normalized;

        //弾の初期化
        rb.velocity = Vector3.zero;
        rb.AddForce(vec * BULLET_SPEED, ForceMode.VelocityChange);
    }

    void Update()
    {
        rb.AddForce(Vector3.down * 3.8f, ForceMode.Acceleration);
    }

    public void ShotBullet()
    {

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

                case "Player":
                    break;

                default:
                    break;
            }
        }
    }
}
