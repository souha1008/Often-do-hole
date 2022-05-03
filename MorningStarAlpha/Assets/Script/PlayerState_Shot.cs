using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 弾を撃った状態(一度紐が伸び切ったら長さ固定のもの)
/// 弾はオブジェクトには接触していない
/// スティックでの移動不可、弾を引き戻すことのみ可能
/// </summary>
public class PlayerStateShot : PlayerState
{
    float countTime;               //発射からの時間

    bool finishFlag;
    private bool releaseButton;
    private Vector3 followStartdiff;
    private Vector3 maxFollowAddvec;
    private float debug_timer;
    private Queue<Vector3> Vecs = new Queue<Vector3>();
    private int beforeFrame;
    bool recoverCanShot;
    bool onceAnim;


    private void Init()
    {
        countTime = 0.0f;
        finishFlag = false;
        releaseButton = false;
        followStartdiff = Vector3.zero;
        maxFollowAddvec = Vector3.zero;
        debug_timer = 0.0f;
        beforeFrame = 0;
        recoverCanShot = false;
        onceAnim = false;

        PlayerScript.refState = EnumPlayerState.SHOT;

        PlayerScript.canShotState = false;
        PlayerScript.ClearModeTransitionFlag();
        PlayerScript.addVel = Vector3.zero;
        PlayerScript.vel.x *= 0.4f;

        //アニメーション用
        PlayerScript.ResetAnimation();
        PlayerScript.animator.SetBool(PlayerScript.animHash.isShot, true);
    }

    public PlayerStateShot(bool isFollow)//コンストラクタ
    {
        Init();
        //弾の発射
        BulletScript.GetComponent<Collider>().isTrigger = false;
        BulletScript.VisibleBullet();


        if (isFollow)
        {
            BulletScript.SetBulletState(EnumBulletState.STOP);
            PlayerScript.shotState = ShotState.FOLLOW;
        }
        else
        {
            BulletScript.SetBulletState(EnumBulletState.GO);
            PlayerScript.shotState = ShotState.GO;
        }

        Vecs.Enqueue(BulletScript.vel / BulletScript.BULLET_SPEED_MULTIPLE);
    }

    /// <summary>
    /// 引っ張られているとき、プレイヤーを進行方向に対して回転
    /// </summary>
    public void RotationPlayer()
    {
        switch (PlayerScript.shotState)
        {
            case ShotState.STRAINED:
            case ShotState.FOLLOW:
                if (PlayerScript.isOnGround == false)
                {
                    Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;
                    Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);
                    Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //補正用クオータニオン
                    quaternion *= adjustQua;
                    PlayerScript.rb.rotation = quaternion;
                }
                break;     
        }
    }

    /// <summary>
    /// 引っ張られている時、間にオブジェクトがあったら切り離し
    /// </summary>
    private void StrainedStop()
    {
        Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;
        Ray ray = new Ray(PlayerScript.rb.position, vecToPlayer.normalized);

        if (Physics.SphereCast(ray, PlayerScript.HcolliderRadius, PlayerScript.HcoliderDistance, LayerMask.GetMask("Platform")))
        {
            if (BulletScript.isTouched == false)
            {
                Debug.Log("collision PlayerHead : Forcibly return");
                PlayerScript.ForciblyReleaseMode(true);
            }
        }
    }

    private Vector3 ReleaseForceCalicurate()
    {
        if (Vecs.Count == 0)
        {
            Debug.LogWarning("No Vecs IN Queue");
            return BulletScript.vel;
        }
        else
        {
            Vector3 returnVec = Vecs.Peek();

            //returnVecを一定の値に補正
            float minVecPower = Mathf.Min(returnVec.magnitude, 60.0f);
            returnVec = returnVec * (60.0f / minVecPower);

            return returnVec;
        }
    }

    private void intoVecsQueue()
    {
        const int VEC_SAVE_NUM = 26;
        if (PlayerScript.shotState == ShotState.GO)
        {
            Vecs.Enqueue(BulletScript.vel / BulletScript.BULLET_SPEED_MULTIPLE);
            if (beforeFrame > VEC_SAVE_NUM)
            {
                Vecs.Dequeue();
            }
            else
            {
                beforeFrame++;
            }
        }
        else if (PlayerScript.shotState == ShotState.STRAINED)
        {
            Vecs.Enqueue(BulletScript.vel);
            if (beforeFrame > VEC_SAVE_NUM)
            {
                Vecs.Dequeue();
            }
            else
            {
                beforeFrame++;
            }
        } 
    }

    public override void UpdateState()
    {
        countTime += Time.deltaTime;

        //アンカーが刺さらない壁にあたったときなど、外部契機で引き戻しに移行
        if (PlayerScript.forciblyRleaseFlag)
        {
            if (BulletScript.isTouched)
            {
                PlayerScript.forciblyRleaseFlag = false;

                if (PlayerScript.forciblyReleaseSaveVelocity)
                {
                    PlayerScript.vel = ReleaseForceCalicurate();
                }
                else
                {
                    PlayerScript.vel = Vector3.zero;
                }

                BulletScript.SetBulletState(EnumBulletState.RETURN);
                PlayerScript.useVelocity = true;
                finishFlag = true;
            }
        }

        //follow開始
        if (PlayerScript.forciblyFollowFlag)
        {
            if (BulletScript.isTouched)
            {
                BulletScript.SetBulletState(EnumBulletState.STOP);

                PlayerScript.vel = ReleaseForceCalicurate();

                PlayerScript.vel.y += 30.0f;

                if (PlayerScript.forciblyFollowVelToward)
                {
                    Vector3 towardVec = BulletScript.rb.position - PlayerScript.rb.position;
                    PlayerScript.vel = towardVec.normalized * PlayerScript.vel.magnitude;
                    recoverCanShot = true;
                }

                PlayerScript.useVelocity = true;
                PlayerScript.forciblyFollowFlag = false;
                PlayerScript.forciblyFollowVelToward = false;
                followStartdiff = BulletScript.colPoint - PlayerScript.rb.position;

                PlayerScript.shotState = ShotState.FOLLOW;
            }
        }
        

        if (countTime > 0.1f)
        {
            if (PlayerScript.shotState == ShotState.STRAINED)
            {
                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
                {
                    releaseButton = true;
                }
                

                if (releaseButton) //ボタンが離れていたら
                {
                    releaseButton = false;
                    BulletScript.SetBulletState(EnumBulletState.RETURN);

                    PlayerScript.vel = ReleaseForceCalicurate();

                    PlayerScript.useVelocity = true;
                    finishFlag = true;
                }
            }
        }

        //ついていく処理
        if (PlayerScript.shotState == ShotState.STRAINED)
        {
            //弾からプレイヤー方向へBULLET_ROPE_LENGTHだけ離れた位置に常に補正
            if (Vector3.Magnitude(PlayerScript.rb.position - BulletScript.rb.position) > BulletScript.BULLET_ROPE_LENGTH)
            {
                Vector3 diff = (PlayerScript.rb.position - BulletScript.rb.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
                PlayerScript.rb.position = BulletScript.rb.position + diff;
                //弾がプレイヤーより強い勢いを持っているときのみ
                if (PlayerScript.vel.magnitude < BulletScript.vel.magnitude * 0.8f)
                {
                    PlayerScript.vel = BulletScript.vel * 0.8f;
                }
            }

            //STRAINEDだけど自由移動のタイミング
            else
            {
                //弱めの重力 
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 0.1f * (fixedAdjust);
                PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
            }
        }

      
    }

    public override void Move()
    {
        float interval;
        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
  
        RotationPlayer();
        intoVecsQueue();

        switch (PlayerScript.shotState)
        {           
            case ShotState.GO:         
                //紐の長さを超えたら引っ張られている状態にする
                if (interval > BulletScript.BULLET_ROPE_LENGTH)
                {
                    //引っ張られたタイミングでボール減速
                    BulletScript.vel /= BulletScript.BULLET_SPEED_MULTIPLE;

                    PlayerScript.shotState = ShotState.STRAINED;
                    PlayerScript.vel = Vector3.zero;            
                }

                debug_timer += Time.fixedDeltaTime;
                Debug.Log(debug_timer);
                break;

            case ShotState.STRAINED:
                Debug.Log(debug_timer);
                StrainedStop();
                if(onceAnim == false)
                {
                    onceAnim = true;
                    RotationPlayer();
                    PlayerScript.animator.Play("Shot.midair_roop");
                }


                //このとき、移動処理は直にposition変更しているため???????、update内に記述
                //ここに記述するとカメラがブレる

                //真上用キャッチ処理
                if (interval < 6.0f)
                {
                    PlayerScript.ForciblyReleaseMode(true);
                }

                break;

            case ShotState.FOLLOW:
                //自分へ弾を引き戻す
                Vector3 vecToBullet = BulletScript.rb.position - PlayerScript.rb.position;
                vecToBullet = vecToBullet.normalized;

                PlayerScript.vel += vecToBullet * 2;
                maxFollowAddvec += vecToBullet * 2;

                //留まってベクトルが溜まってしまった場合切り離し
                if(maxFollowAddvec.magnitude > 80)
                {
                    PlayerScript.vel *= 0.5f;
                    BulletScript.SetBulletState(EnumBulletState.RETURN);
                    Debug.Log("FOLLOW END : over 80");
                    finishFlag = true;
                }
                //ボールに収束しなそうだったら切り離し（回転バグ防止）
                Vector3 nowDiff = BulletScript.colPoint - PlayerScript.rb.position;
                if (followStartdiff.x * nowDiff.x < 0 || followStartdiff.y * nowDiff.y < 0)
                {

                    BulletScript.SetBulletState(EnumBulletState.RETURN);
                    Debug.Log("FOLLOW END : 収束しない");
                    finishFlag = true;
                }

                if (interval < 4.0f)
                {
                    finishFlag = true;
                    BulletScript.SetBulletState(EnumBulletState.READY);
                }

                break;
        }
    }

    public override void StateTransition()
    {
        //ボールが触れたらスイング状態
        if (PlayerScript.forciblySwingFlag)
        {
            if (BulletScript.isTouched)
            {
                PlayerScript.shotState = ShotState.NONE;
                PlayerScript.animator.SetBool(PlayerScript.animHash.isShot, false);
                BulletScript.SetBulletState(EnumBulletState.STOP);

                PlayerScript.forciblySwingFlag = false;
               
                PlayerScript.mode = new PlayerStateSwing_Vel();
                
            }
        }

        if (finishFlag)
        {
            PlayerScript.shotState = ShotState.NONE;
            PlayerScript.animator.SetBool(PlayerScript.animHash.isShot, false);

            //着地したら立っている状態に移行
            if (PlayerScript.isOnGround)
            {
                PlayerScript.mode = new PlayerStateOnGround();
            }
            else //そうでないなら空中
            {
                PlayerScript.mode = new PlayerStateMidair(recoverCanShot ,MidairState.NORMAL);
            }
        }

    }
    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Shot");
    }
}

