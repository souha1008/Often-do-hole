using UnityEngine;

public class BulletMain : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody rb;
    [System.NonSerialized] public Collider co;

    [SerializeField] private GameObject Player;
    [SerializeField] private Renderer[] Part; //�\���p�[�c�A�@�����_���[���A�^�b�`����Ă������

    private PlayerMain PlayerScript;
    [ReadOnly] public Vector3 vel;
    [ReadOnly] public bool swingEnd;
    [ReadOnly] public bool followEnd;

    //���쌴
    public int STRAIGHT_FLAME_CNT;//�܂������i�ރt���[����


   //�e�֌W�萔
    [SerializeField] private float BULLET_SPEED; //�e�̏���   
    [SerializeField] private float BULLET_START_DISTANCE; //�e�̔��ˈʒu
    [SerializeField] public float BULLET_ROPE_LENGTH; //�R�̒���
    public float BULLET_MAXFALLSPEED = 35.0f;
    public float fixedAdjust;


    // �����ǉ�
    [System.NonSerialized] public static BulletMain instance;
    [ReadOnly] public EnumBulletState NowBulletState;
    private BulletState mode;
    


    void Awake()
    {
        // �����ǉ�
        instance = this;
        BulletState.BulletScript = this;
        BulletState.Bullet = gameObject;



        rb = GetComponent<Rigidbody>();
        co = GetComponent<Collider>();
        PlayerState.BulletScript = this;
    }

    private void Start()
    {
        // �����ǉ�
        BulletState.PlayerScript = PlayerMain.instance; // PlayerMain��Awake�ɕύX�\��
        mode = new BulletReady(); // �����X�e�[�g



        PlayerScript = Player.GetComponent<PlayerMain>();

        fixedAdjust = Time.fixedDeltaTime * 50;
        InvisibleBullet();
    }

    // �d�̃X�e�[�g�ύX
    public void SetBulletState(EnumBulletState bulletState)
    {
        BulletState state = null;

        switch (NowBulletState)
        {
            case EnumBulletState.BulletReady:

                if (bulletState == EnumBulletState.BulletGo)
                    state = GetBulletState(bulletState);

                break;
            case EnumBulletState.BulletGo:

                if (bulletState == EnumBulletState.BulletStop || 
                    bulletState == EnumBulletState.BulletReturn || 
                    bulletState == EnumBulletState.BulletReturnFollow)
                    state = GetBulletState(bulletState);

                break;
            case EnumBulletState.BulletStop:

                if (bulletState == EnumBulletState.BulletReturn || 
                    bulletState == EnumBulletState.BulletReturnFollow)
                    state = GetBulletState(bulletState);

                break;
            case EnumBulletState.BulletReturn:
            case EnumBulletState.BulletReturnFollow:
                if (bulletState == EnumBulletState.BulletReady)
                    state = GetBulletState(bulletState);
                break;
        }

        if (state != null)
        {
            mode = state;
        }
    }

    // �d�̃X�e�[�g����
    private BulletState GetBulletState(EnumBulletState bulletState)
    {
        switch (bulletState)
        {
            case EnumBulletState.BulletReady:
                return new BulletReady();
            case EnumBulletState.BulletGo:
                return new BulletGo();
            case EnumBulletState.BulletStop:
                return new BulletStop();
            case EnumBulletState.BulletReturn:
                return new BulletReturn();
            case EnumBulletState.BulletReturnFollow:
                return new BulletReturnFollow();
        }
        return null;
    }


    public void InvisibleBullet()
    {
        rb.isKinematic = true;
        co.enabled = false;
        for (int i = 0; i < Part.Length; i++)
        {
            Part[i].enabled = false;
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            vel = Vector3.zero;
        }
    }

    public void VisibleBullet()
    {
        rb.isKinematic = false;
        co.enabled = true;
        for (int i = 0; i < Part.Length; i++)
        {
            Part[i].enabled = true;
        }
    }




    public void ShotBullet()
    {
        rb.isKinematic = false;
        swingEnd = false;
        followEnd = false;
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;

        //�e�̏�����
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        vel += vec * BULLET_SPEED;

        //�������Ă��炢�܂���
        //�i�s�����Ɠ��������ɓ�����ꍇ�З͕␳
        //if (Mathf.Sign(vec.x) == Mathf.Sign(PlayerScript.vel.x))
        //{
        //    vel += PlayerScript.vel *= 0.6f;
        //}

    }

    public void ShotSlideJumpBullet()
    {
        rb.isKinematic = false;
        swingEnd = false;
        followEnd = false;
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;

        //�e�̏�����
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        vel += vec * BULLET_SPEED * 0.8f;

       
        vel += PlayerScript.vel *= 0.6f;
    }

    private void Update()
    {
        // �����ǉ�
        mode.Update();
    }

    void FixedUpdate()
    {
        // �����ǉ�
        mode.FixedUpdate();

        if (ReferenceEquals(Player, null) == false)
        {
            rb.velocity = vel;
        }
    }

    public void ReturnBullet()
    {
        if (ReferenceEquals(Player, null) == false)
        {
            rb.isKinematic = false;
            GetComponent<Collider>().isTrigger = true;
            rb.velocity = Vector3.zero;
            vel = Vector3.zero;
            //ExitFlameCnt = 0;
        }
    }

    public void RotateBullet()
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
            //�d���h����ꏊ��ǃs�b�^���ɂ��鏈��
            //AdjustColPoint(colAspect, colPoint);


            if (NowBulletState == EnumBulletState.BulletGo)
            {
                //collsion���tag�ŏꍇ����
                string tag = collision.gameObject.tag;
                switch (tag)
                {
                    case "Platform":
                        Aspect_8 colAspect = DetectAspect.Detection8Pos(collision.gameObject.GetComponent<BoxCollider>(), this.rb.position);
                        Vector3 colPoint = collision.GetContact(0).point;

                        EffectManager.instance.StartShotEffect(colPoint, Quaternion.identity);
                        GetComponent<Collider>().isTrigger = true;
                        rb.isKinematic = true;
                        rb.velocity = Vector3.zero;

                       
                        if (colAspect == Aspect_8.UP)
                        {
                            SetBulletState(EnumBulletState.BulletReturnFollow);
                            //FOLLOW��ԂɈڍs
                            followEnd = true;
                        }
                        else if(colAspect == Aspect_8.UP_RIGHT && this.vel.x >= 0) //�E�ɐi��ł���̂ɉE��ɓ��������Ƃ�
                        {
                            SetBulletState(EnumBulletState.BulletReturn);
                            //FOLLOW��ԂɈڍs
                            followEnd = true;
                        }
                        else if (colAspect == Aspect_8.UP_LEFT && this.vel.x <= 0) //�t
                        {
                            SetBulletState(EnumBulletState.BulletReturn);
                            //FOLLOW��ԂɈڍs
                            followEnd = true;
                        }
                        else
                        {
                            SetBulletState(EnumBulletState.BulletStop);    // �ǂɂ�������
                        }
                     

                        break;

                    case "Iron":
                        SetBulletState(EnumBulletState.BulletReturn);      // �d�����߂�
                        break;

                    case "Player":
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
