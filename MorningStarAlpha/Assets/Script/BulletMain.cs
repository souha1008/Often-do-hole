using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMain : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody rb;

    [SerializeField] private GameObject Player;
    [SerializeField] private Renderer[] Part; //�\���p�[�c�A�@�����_���[���A�^�b�`����Ă������

    private PlayerMain PlayerScript;
    [ReadOnly] public Vector3 vel;
    [ReadOnly] public bool isTouched; //�e���Ȃɂ��ɐG�ꂽ��
    [ReadOnly] public bool onceFlag; //���̔��˂ɕt���ڐG���N����͈̂��
    [ReadOnly] public bool StopVelChange; //�e���߂���Ĉ��������Ă�����
    [ReadOnly] public bool swingEnd;
    [ReadOnly] public bool followEnd;

    //���쌴
    private int ExitFlameCnt = 0;//���݂��n�߂Ă���̃J�E���g
    public int STRAIGHT_FLAME_CNT;//�܂������i�ރt���[����


   //�e�֌W�萔
    [SerializeField] private float BULLET_SPEED; //�e�̏���   
    [SerializeField] private float BULLET_START_DISTANCE; //�e�̔��ˈʒu
    [SerializeField] public float BULLET_ROPE_LENGTH; //�R�̒���
    private float BULLET_MAXFALLSPEED = 35.0f;
    private float fixedAdjust;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PlayerState.BulletScript = this;
    }

    private void Start()
    {
        PlayerScript = Player.GetComponent<PlayerMain>();
        ExitFlameCnt = 0;

        fixedAdjust = Time.fixedDeltaTime * 50; 
    }

    public void InvisibleBullet()
    {
        rb.isKinematic = true;
        for(int i = 0; i < Part.Length; i++)
        {
            Part[i].enabled = false;
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            vel = Vector3.zero;
            StopVelChange = true;
        }
    }

    public void VisibleBullet()
    {
        rb.isKinematic = false;
        for (int i = 0; i < Part.Length; i++)
        {
            Part[i].enabled = true;
        }
    }




    public void ShotBullet()
    {
        rb.isKinematic = false;
        onceFlag = false;
        StopVelChange = false;
        swingEnd = false;
        followEnd = false;
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;

        //�e�̏�����
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        isTouched = false;
        vel += vec * BULLET_SPEED;

        //�������Ă��炢�܂���
        //�i�s�����Ɠ��������ɓ�����ꍇ�З͕␳
        //if (Mathf.Sign(vec.x) == Mathf.Sign(PlayerScript.vel.x))
        //{
        //    vel += PlayerScript.vel *= 0.6f;
        //}

    }

    void FixedUpdate()
    {
        if (ReferenceEquals(Player, null) == false)
        {
            if (StopVelChange == false)
            {
                RotateBullet();
                ExitFlameCnt++;
                //�萔�b�ȏ�o���Ă���
                if(ExitFlameCnt > STRAIGHT_FLAME_CNT)
                {
                    //�d�͉��Z
                    vel += Vector3.down * PlayerScript.STRAINED_GRAVITY * (fixedAdjust);
                }            

                Mathf.Max(vel.y, BULLET_MAXFALLSPEED * -1);
            }
            else
            {
                ExitFlameCnt = 0;
            }
            rb.velocity = vel;
        }
    }

    public void ReturnBullet()
    {
        if (ReferenceEquals(Player, null) == false)
        {
            isTouched = false;
            StopVelChange = true;
            rb.isKinematic = false;
            GetComponent<Collider>().isTrigger = true;
            rb.velocity = Vector3.zero;
            vel = Vector3.zero;
            //ExitFlameCnt = 0;
        }
    }

    public void FollowedPlayer()
    {
        if (ReferenceEquals(Player, null) == false)
        {
            isTouched = false;
            StopVelChange = true;
            rb.isKinematic = true;
            GetComponent<Collider>().isTrigger = true;
            rb.velocity = Vector3.zero;
            vel = Vector3.zero;
        }
    }

    void RotateBullet()
    {
        Quaternion quaternion = Quaternion.LookRotation(vel.normalized);
        Quaternion adjustQua = Quaternion.Euler(0, 90, -90); //�␳�p�N�I�[�^�j�I��
        quaternion = quaternion * adjustQua;
        rb.MoveRotation(quaternion);
    }


    private void AdjustColPoint(Aspect colAspect, Vector3 colPoint)
    {
        Vector3 adjustPos = transform.position;

        switch (colAspect)
        {
            case Aspect.UP:
                adjustPos.y = colPoint.y;
                break;

            case Aspect.DOWN:
                adjustPos.y = colPoint.y;
                break;

            case Aspect.LEFT:
                adjustPos.x = colPoint.x;
                break;

            case Aspect.RIGHT:
                adjustPos.x = colPoint.x;
                break;
        }

        transform.position = adjustPos;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (ReferenceEquals(Player, null) == false)
        {
            Aspect colAspect = DetectAspect.DetectionAspect(collision.GetContact(0).normal); //�ڐG�_�̖@���x�N�g��
            Vector3 colPoint = collision.GetContact(0).point;

            //�d���h����ꏊ��ǃs�b�^���ɂ��鏈��
            //AdjustColPoint(colAspect, colPoint);


            //Quaternion rot = Quaternion.identity;
            //switch (colAspect) {
            //    case Aspect.DOWN:
            //        rot = Quaternion.Euler(new Vector3(90, 0, 0));
            //        break;

            //    case Aspect.UP:
            //        rot = Quaternion.Euler(new Vector3(270, 0, 0));
            //        break;

            //    case Aspect.LEFT:
            //        rot = Quaternion.Euler(new Vector3(0, 270, 0));
            //        break;

            //    case Aspect.RIGHT:
            //        rot = Quaternion.Euler(new Vector3(0, 90, 0));
            //        break;
            //}

            if (onceFlag == false)
            {
                onceFlag = true;
                //collsion���tag�ŏꍇ����
                string tag = collision.gameObject.tag;
                switch (tag)
                {
                    case "Platform": 
                        EffectManager.instance.StartShotEffect(colPoint, Quaternion.identity);
                        isTouched = true;
                        GetComponent<Collider>().isTrigger = true;
                        rb.isKinematic = true;
                        rb.velocity = Vector3.zero;
                        StopVelChange = true;


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
                        onceFlag = false;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
