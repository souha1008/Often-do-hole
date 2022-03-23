using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMain : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody rb;

    [System.NonSerialized] public Rigidbody rb_2;
    [SerializeField] private GameObject Player;
    private PlayerMain PlayerScript;
    public Vector3 vel;
    public bool isTouched; //�e���Ȃɂ��ɐG�ꂽ��
    public bool onceFlag; //���̔��˂ɕt���ڐG���N����͈̂��
    public bool StopVelChange; //�e���߂���Ĉ��������Ă�����
    public bool swingEnd;
    public bool followEnd;

   //�e�֌W�萔
    [SerializeField] private float BULLET_SPEED; //�e�̏���   
    [SerializeField] private float BULLET_START_DISTANCE; //�e�̔��ˈʒu
    [SerializeField] public float BULLET_ROPE_LENGTH; //�R�̒���
    private float BULLET_MAXFALLSPEED = 35.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PlayerState.BulletScript = this;
    }

    private void Start()
    {
        PlayerScript = Player.GetComponent<PlayerMain>();
    }

    public void ShotBullet()
    {
        rb.isKinematic = false;
        onceFlag = false;
        StopVelChange = false;
        swingEnd = false;
        followEnd = false;
        Vector3 vec = PlayerScript.leftStick.normalized;

        //�e�̏�����
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        isTouched = false;

        vel += vec * BULLET_SPEED;

        //�i�s�����Ɠ��������ɓ�����ꍇ�З͕␳
        if (Mathf.Sign(vec.x) == Mathf.Sign(PlayerScript.vel.x))
        {
            vel += PlayerScript.vel *= 0.6f;
        }

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
            colAspect = DetectAspect.DetetAspect(collision.contacts[0].normal); //�ڐG�_�̖@���x�N�g��

            if (onceFlag == false)
            {
                onceFlag = true;
                //collsion���tag�ŏꍇ����
                string tag = collision.gameObject.tag;
                switch (tag)
                {
                    case "Platform":
                        isTouched = true;
                        rb.isKinematic = true;
                        rb.velocity = Vector3.zero;
                        if (colAspect == Aspect.UP)
                        {
                            //FOLLOW��ԂɈڍs
                            followEnd = true;
                        }
                        else
                        {
                            //SWING��ԂɈڍs
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
