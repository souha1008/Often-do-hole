using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMain : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody rb;
    private GameObject Player;
    private PlayerMain PlayerScript;
    public Vector3 vel;
    public bool isTouched; //�e���Ȃɂ��ɐG�ꂽ��
    public bool onceFlag; //���̔��˂ɕt���ڐG���N����͈̂��
    public bool StopDownVel; //�e���߂���Ĉ��������Ă�����
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

        //�e�̏�����
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
    ///�@�ʌ���
    /// </summary>
    enum Aspect
    {
        UP,        //���
        DOWN,      //����
        LEFT,      //����
        RIGHT,�@�@ //�E��
        INVALID,   //��O
    }

    /// <summary>
    /// �@���x�N�g���ɂ���Ėʂ̌������擾
    /// </summary>
    /// <param name="vec">�@��</param>
    /// <returns></returns>
    private Aspect DetetAspect(Vector3 vec)
    {
        Aspect returnAspect = Aspect.INVALID;
        if (Mathf.Abs(vec.y) > 0.5f) //y�������傫���̂ŏc����
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
        else if (Mathf.Abs(vec.x) > 0.5f) //x�������傫���̂ŉ�����
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
            Debug.LogError("�ڐG�ʂ̖@�����΂߂̉\��������܂�");
        }

        return returnAspect;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Aspect colAspect = Aspect.INVALID;
        colAspect = DetetAspect(collision.contacts[0].normal); //�ڐG�_�̖@���x�N�g��

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
                        Debug.Log("col");
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
