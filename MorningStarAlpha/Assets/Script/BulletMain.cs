using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMain : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody rb;
    private GameObject Player;
    private PlayerMain PlayerScript;
    public bool isTouched; //�e���Ȃɂ��ɐG�ꂽ��
    public bool onceFlag; //���̔��˂ɕt���ڐG���N����͈̂��
    //�e�֌W�萔
    [SerializeField] private float BULLET_SPEED; //�e�̏���   
    [SerializeField] private float BULLET_START_DISTANCE; //�e�̔��ˈʒu
    [SerializeField] public float BULLET_ROPE_LENGTH; //�R�̒���


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

        //�e�̏�����
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
            //collsion���tag�ŏꍇ����
            string tag = collision.gameObject.tag;
            switch (tag)
            {
                case "Platform":
                    isTouched = true;

                    //�����^�����~
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
