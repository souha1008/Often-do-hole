using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShotDir
{
    UP,
    LATERAL,
    DIAGONAL_60,
    DIAGONAL_30
}


/// <summary>
/// 弾を撃った状態(一度紐が伸び切ったら長さ固定のもの)
/// 弾はオブジェクトには接触していない
/// スティックでの移動不可、弾を引き戻すことのみ可能
/// </summary>
public class PlayerStateShot : PlayerState
{
    float countTime;               //発射からの時間

    bool finishFlag;
    private ShotDir shotDir;
    private bool releaseButton;
    private bool onceAnimReturn;
    private Vector3 followStartdiff;
    private Vector3 maxFollowAddvec;
    private float debug_timer;
    private Queue<Vector3> Vecs = new Queue<Vector3>();
    private int beforeFrame;

    const float STRAINED_END_POWER = 70.0f;

    private void Init()
    {
        countTime = 0.0f;
        shotDir = ShotDir.UP;
        finishFlag = false;
        releaseButton = false;
        onceAnimReturn = false;
        followStartdiff = Vector3.zero;
        maxFollowAddvec = Vector3.zero;
        debug_timer = 0.0f;
        beforeFrame = 0;

        PlayerScript.refState = EnumPlayerState.SHOT;
        PlayerScript.shotState = ShotState.GO;
        PlayerScript.canShotState = false;
        PlayerScript.forciblyReturnBulletFlag = false;
        PlayerScript.addVel = Vector3.zero;

        PlayerScript.vel.x *= 0.4f;
        PlayerScript.animator.SetBool("isShot", true);
        
        //アニメーション用
        if (Mathf.Abs(PlayerScript.adjustLeftStick.y) < 0.1f)
        {
            //横投げ
            PlayerScript.animator.SetInteger("shotdirType", 1);
        }
        else
        {
            //斜め投げ
            PlayerScript.animator.SetInteger("shotdirType", 2);
        }

        //アニメーション・回転用　真上
        if(Mathf.Abs(PlayerScript.adjustLeftStick.x) < 0.1f)
        {
            shotDir = ShotDir.UP;
        }
        else
        {
            shotDir = ShotDir.LATERAL;
        }
    }

    //消去
    public PlayerStateShot()//コンストラクタ
    {
        Init();
        //弾の発射
        BulletScript.GetComponent<Collider>().isTrigger = false;
        BulletScript.VisibleBullet();

      
        BulletScript.ShotBullet();

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
                if (PlayerScript.isOnGround == false)
                {
                    Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;
                    Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);
                    Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //補正用クオータニオン
                    quaternion *= adjustQua;
                    PlayerScript.rb.rotation = quaternion;
                }
                break;

            case ShotState.RETURN:
            case ShotState.FOLLOW:
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 90, 0));
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, -90, 0));
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
                PlayerScript.ForciblyReturnBullet(true);
            }
        }
    }

    private Vector3 ReleaseForceCalicurate()
    {
        if(Vecs.Count == 0) 
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

            //入力方向にやや補正
            //if(PlayerScript.isOnGround == false)
            //{
            //    if((PlayerScript.dir == PlayerMoveDir.RIGHT) && PlayerScript.sourceLeftStick.x > 0.1f)
            //    {
            //        returnVec.x *= 1.1f;
            //    }
            //    if ((PlayerScript.dir == PlayerMoveDir.LEFT) && PlayerScript.sourceLeftStick.x < -0.1f)
            //    {
            //        returnVec.x *= 1.1f;
            //    }
            //}

            return returnVec;
        }
      
    }


    private void intoVecsQueue()
    {
        
        if (PlayerScript.shotState == ShotState.GO)
        {
            Vecs.Enqueue(BulletScript.vel / BulletScript.BULLET_SPEED_MULTIPLE);
            if (beforeFrame > 30)
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
            if (beforeFrame > 30)
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

        if (countTime > 0.1f)
        {
            if (PlayerScript.shotState == ShotState.STRAINED)
            {
                if (PlayerScript.ReleaseMode)
                {
                    if (Input.GetButtonUp("Button_R")) //ボタンが離れていたら
                    {
                        releaseButton = true;
                    }
                }
                else
                {
                    if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
                    {
                        releaseButton = true;
                    }
                }

                if (releaseButton) //ボタンが離れていたら
                {
                    releaseButton = false;
                    BulletScript.ReturnBullet();

                    PlayerScript.vel = ReleaseForceCalicurate();

                    PlayerScript.useVelocity = true;
                    PlayerScript.shotState = ShotState.RETURN;
                }
            }
        }

        //アンカーが刺さらない壁にあたったときなど、外部契機で引き戻しに移行
        if (PlayerScript.forciblyReturnBulletFlag)
        {
            PlayerScript.forciblyReturnBulletFlag = false;

            if (PlayerScript.forciblyReturnSaveVelocity)
            {
                PlayerScript.vel = ReleaseForceCalicurate();
                ;
            }
            else
            {
                PlayerScript.vel = Vector3.zero;
            }

            BulletScript.ReturnBullet();

            PlayerScript.useVelocity = true;
            PlayerScript.shotState = ShotState.RETURN;
        }

        //ついていく処理
        if (PlayerScript.shotState == ShotState.STRAINED)
        {
#if false
            //弾からプレイヤー方向へBULLET_ROPE_LENGTHだけ離れた位置に常に補正
            Vector3 diff = (PlayerScript.transform.position - BulletScript.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
            Player.transform.position = BulletScript.transform.position + diff;
#else
            //弾からプレイヤー方向へBULLET_ROPE_LENGTHだけ離れた位置に常に補正
            if (Vector3.Magnitude(Player.transform.position - BulletScript.transform.position) > BulletScript.BULLET_ROPE_LENGTH)
            {
                Vector3 diff = (PlayerScript.transform.position - BulletScript.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
                Player.transform.position = BulletScript.transform.position + diff;
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
#endif
        }

        //follow開始
        if (BulletScript.isTouched)
        {
            if (BulletScript.followEnd)
            {
                BulletScript.FollowedPlayer();

                PlayerScript.vel = ReleaseForceCalicurate();

                PlayerScript.useVelocity = true;
                BulletScript.followEnd = false;
                PlayerScript.shotState = ShotState.FOLLOW;
                followStartdiff = BulletScript.colPoint - PlayerScript.rb.position;
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

                    //PlayerScript.useVelocity = false;
                }

                debug_timer += Time.fixedDeltaTime;
                Debug.Log(debug_timer);
                break;

            case ShotState.STRAINED:
                Debug.Log(debug_timer);
                StrainedStop();
                //このとき、移動処理は直にposition変更しているため???????、update内に記述
                //ここに記述するとカメラがブレる

                //真上用キャッチ処理
                if (interval < 6.0f)
                {
                    PlayerScript.ForciblyReturnBullet(true);
                }

                break;

            case ShotState.RETURN:
                //自分へ弾を引き戻す
                if (onceAnimReturn == false)
                {
                    onceAnimReturn = true;
                    PlayerScript.animator.SetBool("isShot", false);
                }

                Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
                vecToPlayer = vecToPlayer.normalized;

                BulletScript.vel = vecToPlayer * 100;

                //距離が一定以下になったら終了処理フラグを建てる
                if (interval < 4.0f)
                {
                    finishFlag = true;
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
                    PlayerScript.ForciblyReturnBullet(true);
                    Debug.Log("FOLLOW END : over 80");
                }
                //ボールに収束しなそうだったら切り離し（回転バグ防止）
                Vector3 nowDiff = BulletScript.colPoint - PlayerScript.rb.position;
                if (followStartdiff.x * nowDiff.x < 0 || followStartdiff.y * nowDiff.y < 0)
                {
                    PlayerScript.ForciblyReturnBullet(true);
                    Debug.Log("FOLLOW END : 収束しない");
                }

                if (interval < 4.0f)
                {
                    finishFlag = true;
                }

                break;
        }
    }

    public override void StateTransition()
    {
        //ボールが触れたらスイング状態
        if (BulletScript.isTouched)
        {
            PlayerScript.shotState = ShotState.NONE;
            PlayerScript.animator.SetBool("isShot", false);

            if (BulletScript.swingEnd)
            {
                BulletScript.swingEnd = false;
                if (PlayerScript.AutoRelease)
                {
                    PlayerScript.mode = new PlayerStateSwing_Vel();
                }
                else
                {
                    PlayerScript.mode = new PlayerStateSwing_Vel();
                }
            }
        }

        if (finishFlag)
        {
            PlayerScript.shotState = ShotState.NONE;
            PlayerScript.animator.SetBool("isShot", false);

            //着地したら立っている状態に移行
            if (PlayerScript.isOnGround)
            {
                PlayerScript.mode = new PlayerStateOnGround();
            }
            else //そうでないなら空中
            {
                PlayerScript.mode = new PlayerStateMidair(false);
            }
        }

    }
    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Shot");
    }
}

