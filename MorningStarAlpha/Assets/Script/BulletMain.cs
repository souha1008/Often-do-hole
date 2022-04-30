using UnityEngine;

public enum BulletRefState
{
    READY,
    GO,
    RETURN,
    STOP,
}



public class BulletMain : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody rb;
    [System.NonSerialized] public Collider co;

    private PlayerMain PlayerScript;
    [SerializeField] private GameObject Player;
    [SerializeField] private SkinnedMeshRenderer[] Part; //構成パーツ、　レンダラーがアタッチされているもの
                                                       //[SerializeField] private SkinnedMeshRenderer[] animPart; //構成パーツ、　レンダラーがアタッチされているもの

    [System.NonSerialized] public static BulletMain instance;
    private BulletState mode;
    [ReadOnly] public EnumBulletState NowBulletState;
    [ReadOnly] public bool CanShotFlag;
    [ReadOnly] public Vector3 vel;
    [ReadOnly] public Vector3 colPoint;
    [ReadOnly] public bool isTouched; //弾がなにかに触れたか
    [ReadOnly] public bool onceFlag; //一回の発射に付き接触が起こるのは一回
    [ReadOnly] public bool StopVelChange; //弾が戻されて引っ張られている状態
    [ReadOnly] public bool isInside; //弾が内側にある状態


    //下川原
    private int ExitFlameCnt = 0;//存在し始めてからのカウント
    public int STRAIGHT_FLAME_CNT;//まっすぐ進むフレーム数


   //弾関係定数
    [SerializeField] public float BULLET_SPEED; //弾の初速
    [SerializeField] private float BULLET_SPEED_MAX; //弾の初速(最大値）
    [SerializeField] public float BULLET_SPEED_MULTIPLE; //GOとSTRAINEDの倍率（この設定でgoの時間を短くする）
    [SerializeField] private float BULLET_START_DISTANCE; //弾の発射位置
    [SerializeField] public float  BULLET_ROPE_LENGTH; //紐の長さ
    public float BULLET_MAXFALLSPEED = 35.0f;
    [System.NonSerialized] public float fixedAdjust;

  

    void Awake()
    {
        // 高平追加
        instance = this;
        BulletState.BulletScript = this;
        PlayerState.BulletScript = this;
        rb = GetComponent<Rigidbody>();
        co = GetComponent<Collider>();

    }

    private void Start()
    {
        BulletState.PlayerScript = PlayerMain.instance;
        PlayerScript = PlayerMain.instance;
        CanShotFlag = true;
        ExitFlameCnt = 0;
        colPoint = Vector3.zero;
        isInside = false;

        fixedAdjust = Time.fixedDeltaTime * 50;
        InvisibleBullet();

        mode = new BulletReady(); // 初期ステート 
    }

    public void InvisibleBullet()
    {
        for (int i = 0; i < Part.Length; i++)
        {
            Part[i].enabled = false;         
        }

        //for (int i = 0; i < animPart.Length; i++)
        //{
        //    animPart[i].enabled = true;
        //}

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        StopVelChange = true;
    }

    public void VisibleBullet()
    {
        isInside = false;
        rb.isKinematic = false;
        co.enabled = true;
        for (int i = 0; i < Part.Length; i++)
        {
            Part[i].enabled = true;
        }

        //for (int i = 0; i < animPart.Length; i++)
        //{
        //    animPart[i].enabled = false;
        //}
    }

    // 錨のステート変更
    public void SetBulletState(EnumBulletState bulletState)
    {
    //    BulletState state = null;

    //    switch (NowBulletState)
    //    {
    //        case EnumBulletState.READY:

    //            if (bulletState == EnumBulletState.GO)
    //                state = GetBulletState(bulletState);

    //            break;
    //        case EnumBulletState.GO:

    //            if (bulletState == EnumBulletState.STOP ||
    //                bulletState == EnumBulletState.RETURN ||
    //                bulletState == EnumBulletState.BulletReturnFollow)
    //                state = GetBulletState(bulletState);

    //            break;
    //        case EnumBulletState.STOP:

    //            if (bulletState == EnumBulletState.RETURN ||
    //                bulletState == EnumBulletState.BulletReturnFollow)
    //                state = GetBulletState(bulletState);

    //            break;
    //        case EnumBulletState.RETURN:
    //        case EnumBulletState.BulletReturnFollow:
    //            if (bulletState == EnumBulletState.READY)
    //                state = GetBulletState(bulletState);
    //            break;
    //    }

    //    state = GetBulletState(bulletState);

    //    if (state != null)
    //    {
    //        mode = state;
    //    }

        mode = GetBulletState(bulletState);
    }

    // 錨のステート生成
    private BulletState GetBulletState(EnumBulletState bulletState)
    {
        switch (bulletState)
        {
            case EnumBulletState.READY:
                return new BulletReady();
            case EnumBulletState.GO:
                return new BulletGo();
            case EnumBulletState.STOP:
                return new BulletStop();
            case EnumBulletState.RETURN:
                return new BulletReturn();
        }
        return null;
    }

    public void ShotBullet()
    {
        rb.isKinematic = false;
        onceFlag = false;
        StopVelChange = false;
      
        
        colPoint = Vector3.zero;
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;

        //プレイヤー強制処理フラグクリア
        PlayerScript.ClearModeTransitionFlag();

        const float lateral_multiple = 1.05f;

        //弾の初期化
        rb.velocity = Vector3.zero;
        vel = Vector3.zero;
        isTouched = false;

        if(vec.y < 0.3f)
        {
            vel += vec * BULLET_SPEED * lateral_multiple * BULLET_SPEED_MULTIPLE;
        }
        else {
            vel += vec * BULLET_SPEED * BULLET_SPEED_MULTIPLE;
        }

        if (vel.x * PlayerScript.vel.x > 0) //飛ばす方向とプレイヤーの方向が同じだったら
        {
            vel.x += PlayerScript.vel.x;
        }

        if (vec.y < 0.3f)
        {
            if (vel.magnitude > (BULLET_SPEED * lateral_multiple + 5) * BULLET_SPEED_MULTIPLE)
            {
                float adjustVec = (BULLET_SPEED_MAX * BULLET_SPEED_MULTIPLE * lateral_multiple) / vel.magnitude;
                Debug.Log("BulletOverSpeed : adjust Max *= " + adjustVec);
                vel *= adjustVec;
            }
        }
        else
        {

            if (vel.magnitude > (BULLET_SPEED + 5) * BULLET_SPEED_MULTIPLE)
            {
                float adjustVec = (BULLET_SPEED_MAX * BULLET_SPEED_MULTIPLE) / vel.magnitude;
                Debug.Log("BulletOverSpeed : adjust Max *= " + adjustVec);
                vel *= adjustVec;
            }
        }

        if (vec.y < 0.3f)
        {
            PlayerScript.animator.Play("Shot.throw_15");

        }
        else
        {
            PlayerScript.animator.Play("Shot.throw_65");
        }
    }

    public void ReturnBullet()
    {
        if ((NowBulletState == EnumBulletState.STOP) || (NowBulletState == EnumBulletState.GO))
        {
            SetBulletState(EnumBulletState.RETURN);
        }
    }

    private void Update()
    {
        // 高平追加
        mode.Update();
    }

    void FixedUpdate()
    {
        if (ReferenceEquals(Player, null) == false)
        {
            // 高平追加
            mode.Move();

           
            rb.velocity = vel;
        }
    }

    //void FixedUpdate()
    //{
    //    if (ReferenceEquals(Player, null) == false)
    //    {
    //        if (StopVelChange == false)
    //        {
    //            RotateBullet();
    //            ExitFlameCnt++;
    //            //定数秒以上経ってたら
    //            if(ExitFlameCnt > STRAIGHT_FLAME_CNT)
    //            {
    //                //重力加算
    //                vel += Vector3.down * PlayerScript.STRAINED_GRAVITY * (fixedAdjust);
    //            }            

    //            Mathf.Max(vel.y, BULLET_MAXFALLSPEED * -1);
    //        }
    //        else
    //        {
    //            ExitFlameCnt = 0;
    //        }
    //        rb.velocity = vel;
    //    }
    //}



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

    public void StopBullet()
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
            Aspect_8 colAspect;

            if (onceFlag == false)
            { 
                //collsion先のtagで場合分け
                string tag = collision.gameObject.tag;

                switch (tag)
                {
                    case "Platform":
                        onceFlag = true;
                        isTouched = true;
                        //各種演出
                        CameraShake.instance.Shake(rb.velocity);
                        colPoint = collision.GetContact(0).point;
                        EffectManager.instance.StartShotEffect(colPoint, Quaternion.identity);

                        
                        //面計算
                        colAspect = DetectAspect.Detection8Pos(collision.gameObject.GetComponent<BoxCollider>(), this.rb.position);
   
                        if (colAspect == Aspect_8.UP)
                        {
                            //FOLLOW状態に移行
                            PlayerScript.ForciblyFollowMode(false);
                        }
                        else if(colAspect == Aspect_8.UP_RIGHT && this.vel.x >= 0) //右に進んでいるのに右上に当たったとき
                        {
                            //FOLLOW状態に移行
                            PlayerScript.ForciblyFollowMode(false);
                        }
                        else if (colAspect == Aspect_8.UP_LEFT && this.vel.x <= 0) //逆
                        {
                            //FOLLOW状態に移行
                            PlayerScript.ForciblyFollowMode(false);
                        }
                        else
                        {
                            //SWING状態に移行
                            PlayerScript.ForciblySwingMode(false);
                        }
                     

                        break;

                    case "Iron":
                        onceFlag = true;
                        isTouched = true;

                        if (PlayerScript.isOnGround)
                        {
                            PlayerScript.ForciblyReleaseMode(false);
                        }
                        else
                        {
                            PlayerScript.ForciblyReleaseMode(true);
                        }
                        break;

                    case "Conveyor":
                        onceFlag = true;
                        isTouched = true;
                        SetBulletState(EnumBulletState.STOP);


                        //面計算
                        colAspect = DetectAspect.Detection8Pos(collision.gameObject.GetComponent<BoxCollider>(), this.rb.position);
                        if ((colAspect == Aspect_8.DOWN) || (colAspect == Aspect_8.DOWN_LEFT) || (colAspect == Aspect_8.DOWN_RIGHT))
                        {
                            //SWING状態に移行
                            PlayerScript.ForciblySwingMode(false);                       
                        }
                        else if ((colAspect == Aspect_8.UP) || (colAspect == Aspect_8.UP_LEFT) || (colAspect == Aspect_8.UP_RIGHT))
                        {
                            //FOLLOW状態に移行
                            PlayerScript.ForciblyFollowMode(false);
                        }
                        else
                        {
                            PlayerScript.ForciblyReleaseMode(true);
                        }

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
        if (ReferenceEquals(Player, null) == false)
        {
            //錨が刺さる場所を壁ピッタリにする処理
            //AdjustColPoint(colAspect, colPoint);

            if (onceFlag == false)
            {
                //collsion先のtagで場合分け
                if(PlayerScript.refState == EnumPlayerState.SHOT)
                if (NowBulletState == EnumBulletState.GO)
                {
                    string tag = other.gameObject.tag;
                    switch (tag)
                    {
                        case "WireMesh":                     
                            isTouched = true;
                            onceFlag = true;
                            PlayerScript.ForciblySwingMode(true);
                            PlayerMain.instance.RecoverBullet();
                                break;

                        case "SpringBoard":
                            isTouched = true;
                            onceFlag = true;
                            PlayerScript.ForciblyFollowMode(true);
                                break;
                    }
                }
            }
        }
    }
}


