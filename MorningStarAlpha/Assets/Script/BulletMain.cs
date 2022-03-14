using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMain : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody rb;
    private GameObject Player;
    private PlayerMain PlayerScript;
    public bool isTouched; //’e‚ª‚È‚É‚©‚ÉG‚ê‚½‚©
    public bool onceFlag; //ˆê‰ñ‚Ì”­Ë‚É•t‚«ÚG‚ª‹N‚±‚é‚Ì‚Íˆê‰ñ
    //’eŠÖŒW’è”
    [SerializeField] private float BULLET_SPEED; //’e‚Ì‰‘¬   
    [SerializeField] private float BULLET_START_DISTANCE; //’e‚Ì”­ËˆÊ’u
    [SerializeField] public float BULLET_ROPE_LENGTH; //•R‚Ì’·‚³


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

        //’e‚Ì‰Šú‰»
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
            //collsionæ‚Ìtag‚Åê‡•ª‚¯
            string tag = collision.gameObject.tag;
            switch (tag)
            {
                case "Platform":
                    isTouched = true;

                    //•¨—‰^“®‚ğ’â~
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
