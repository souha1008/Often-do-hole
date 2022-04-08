using UnityEngine;

public class BulletMain : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody rb;
    [System.NonSerialized] public Collider co;

    [SerializeField] private GameObject Player;
    [SerializeField] private Renderer[] Part; //構成パーツ、　レンダラーがアタッチされているもの

    private PlayerMain PlayerScript;
    [ReadOnly] public Vector3 vel;
    [ReadOnly] public bool swingEnd;
    [ReadOnly] public bool followEnd;

    //下川原
    public int STRAIGHT_FLAME_CNT;//まっすぐ進むフレーム数


   //弾関係定数
    [SerializeField] private float BULLET_SPEED; //弾の初速   
    [SerializeField] private float BULLET_START_DISTANCE; //弾の発射位置
    [SerializeField] public float BULLET_ROPE_LENGTH; //紐の長さ
    public float BULLET_MAXFALLSPEED = 35.0f;
    public float fixedAdjust;


    // 高平追加
    [System.NonSerialized] public static BulletMain instance;
    [ReadOnly] public EnumBulletState NowBulletState;
    private BulletState mode;
    


    void Awake()
    {
        // 高平追加
        instance = this;
        BulletState.BulletScript = this;
        BulletState.Bullet = gameObject;



        rb = GetComponent<Rigidbody>();
        co = GetComponent<Collider>();
        PlayerState.BulletScript = this;
    }

    private void Start()
    {
        // 高平追加
        BulletState.PlayerScript = PlayerMain.instance; // PlayerMainのAwakeに変更予定
        mode = new BulletReady(); // 初期ステート



        PlayerScript = Player.GetComponent<PlayerMain>();

        fixedAdjust = Time.fixedDeltaTime * 50;
        InvisibleBullet();
    }

    // 錨のステート変更
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

    // 錨のステート生成
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

        //弾の初期化
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        vel += vec * BULLET_SPEED;

        //消させてもらいました
        //進行方向と同じ向きに投げる場合威力補正
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

        //弾の初期化
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        vel += vec * BULLET_SPEED * 0.8f;

       
        vel += PlayerScript.vel *= 0.6f;
    }

    private void Update()
    {
        // 高平追加
        mode.Update();
    }

    void FixedUpdate()
    {
        // 高平追加
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
        Quaternion adjustQua = Quaternion.Euler(0, 90, -90); //補正用クオータニオン
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
            //錨が刺さる場所を壁ピッタリにする処理
            //AdjustColPoint(colAspect, colPoint);


            if (NowBulletState == EnumBulletState.BulletGo)
            {
                //collsion先のtagで場合分け
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
                            //FOLLOW状態に移行
                            followEnd = true;
                        }
                        else if(colAspect == Aspect_8.UP_RIGHT && this.vel.x >= 0) //右に進んでいるのに右上に当たったとき
                        {
                            SetBulletState(EnumBulletState.BulletReturn);
                            //FOLLOW状態に移行
                            followEnd = true;
                        }
                        else if (colAspect == Aspect_8.UP_LEFT && this.vel.x <= 0) //逆
                        {
                            SetBulletState(EnumBulletState.BulletReturn);
                            //FOLLOW状態に移行
                            followEnd = true;
                        }
                        else
                        {
                            SetBulletState(EnumBulletState.BulletStop);    // 壁にくっついた
                        }
                     

                        break;

                    case "Iron":
                        SetBulletState(EnumBulletState.BulletReturn);      // 錨引き戻し
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
