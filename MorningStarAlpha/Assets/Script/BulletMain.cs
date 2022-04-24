using UnityEngine;

public class BulletMain : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody rb;
    [System.NonSerialized] public Collider co;

    [SerializeField] private GameObject Player;
    [SerializeField] private Renderer[] Part; //構成パーツ、　レンダラーがアタッチされているもの

    private PlayerMain PlayerScript;
    [ReadOnly] public Vector3 vel;
    [ReadOnly] public Vector3 colPoint;
    [ReadOnly] public bool isTouched; //弾がなにかに触れたか
    [ReadOnly] public bool onceFlag; //一回の発射に付き接触が起こるのは一回
    [ReadOnly] public bool StopVelChange; //弾が戻されて引っ張られている状態
    [ReadOnly] public bool swingEnd;
    [ReadOnly] public bool followEnd;

    //下川原
    private int ExitFlameCnt = 0;//存在し始めてからのカウント
    public int STRAIGHT_FLAME_CNT;//まっすぐ進むフレーム数


   //弾関係定数
    [SerializeField] public float BULLET_SPEED; //弾の初速
    [SerializeField] private float BULLET_SPEED_MAX; //弾の初速(最大値）
    [SerializeField] public float BULLET_SPEED_MULTIPLE; //GOとSTRAINEDの倍率（この設定でgoの時間を短くする）
    [SerializeField] private float BULLET_START_DISTANCE; //弾の発射位置
    [SerializeField] public float  BULLET_ROPE_LENGTH; //紐の長さ
    private float BULLET_MAXFALLSPEED = 35.0f;
    private float fixedAdjust;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        co = GetComponent<Collider>();
        PlayerState.BulletScript = this;
    }

    private void Start()
    {
        PlayerScript = Player.GetComponent<PlayerMain>();
        ExitFlameCnt = 0;
        colPoint = Vector3.zero;

        fixedAdjust = Time.fixedDeltaTime * 50;
        InvisibleBullet();
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
            StopVelChange = true;
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
        onceFlag = false;
        StopVelChange = false;
        swingEnd = false;
        followEnd = false;
        colPoint = Vector3.zero;
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;

        //弾の初期化
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        isTouched = false;
        vel += vec * BULLET_SPEED * BULLET_SPEED_MULTIPLE;

        if (vel.x * PlayerScript.vel.x > 0) //飛ばす方向とプレイヤーの方向が同じだったら
        {
            vel.x += PlayerScript.vel.x;
        }



        if(vel.magnitude > BULLET_SPEED_MAX * BULLET_SPEED_MULTIPLE)
        {
            float adjustVec = (BULLET_SPEED_MAX * BULLET_SPEED_MULTIPLE) / vel.magnitude;
            Debug.Log("BulletOverSpeed : adjust Max *= " + adjustVec);
            vel *= adjustVec;
        }
    }

    // 消去
    //public void ShotSlideJumpBullet()
    //{
    //    rb.isKinematic = false;
    //    onceFlag = false;
    //    StopVelChange = false;
    //    swingEnd = false;
    //    followEnd = false;
    //    Vector3 vec = PlayerScript.adjustLeftStick.normalized;

    //    //弾の初期化
    //    rb.velocity = Vector3.zero;
    //    vel = Vector3.zero;
    //    isTouched = false;
    //    vel += vec * BULLET_SPEED * 0.8f;

       
    //     vel += PlayerScript.vel *= 0.6f;
    //}

    void FixedUpdate()
    {
        if (ReferenceEquals(Player, null) == false)
        {
            if (StopVelChange == false)
            {
                RotateBullet();
                ExitFlameCnt++;
                //定数秒以上経ってたら
                if(ExitFlameCnt > STRAIGHT_FLAME_CNT)
                {
                    //重力加算
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

    private void StopBullet()
    {
        isTouched = true;
        GetComponent<Collider>().isTrigger = true;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        StopVelChange = true;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (ReferenceEquals(Player, null) == false)
        {
            //錨が刺さる場所を壁ピッタリにする処理
            //AdjustColPoint(colAspect, colPoint);


            if (onceFlag == false)
            {
                //collsion先のtagで場合分け
                string tag = collision.gameObject.tag;
                switch (tag)
                {
                    case "Platform":
                        onceFlag = true;
                        StopBullet();

                        //面計算
                        Aspect_8 colAspect = DetectAspect.Detection8Pos(collision.gameObject.GetComponent<BoxCollider>(), this.rb.position);
                       
                        //各種演出
                        CameraShake.instance.Shake(rb.velocity);
                        colPoint = collision.GetContact(0).point;
                        EffectManager.instance.StartShotEffect(colPoint, Quaternion.identity);



                        if (colAspect == Aspect_8.UP)
                        {
                            //FOLLOW状態に移行
                            followEnd = true;
                        }
                        else if(colAspect == Aspect_8.UP_RIGHT && this.vel.x >= 0) //右に進んでいるのに右上に当たったとき
                        {
                            //FOLLOW状態に移行
                            followEnd = true;
                        }
                        else if (colAspect == Aspect_8.UP_LEFT && this.vel.x <= 0) //逆
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
                        onceFlag = true;
                        if (PlayerScript.isOnGround)
                        {
                            PlayerScript.ForciblyReleaseMode(false);
                        }
                        else
                        {
                            PlayerScript.ForciblyReleaseMode(true);
                        }                        
                        break;


                    case "WireMesh":    
                        onceFlag = true;
                        StopBullet();
                        followEnd = true;

                        break;

                    case "Player":
                        onceFlag = false;
                        Debug.LogWarning("colPlayerBullet");
                        break;

                    case "Box":
                        onceFlag = false;
                        break;

                    default:
                        break;
                }
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("aaa");
        if (ReferenceEquals(Player, null) == false)
        {
            //錨が刺さる場所を壁ピッタリにする処理
            //AdjustColPoint(colAspect, colPoint);

            if (onceFlag == false)
            {
                //collsion先のtagで場合分け
                string tag = other.gameObject.tag;
                switch (tag)
                {
                    case "WireMesh":
                        onceFlag = true;
                        StopBullet();
                        followEnd = true;

                        break;
                }
            }
        }
    }
}


