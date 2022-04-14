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
    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //発射からの弾のvectorを保存する
    bool finishFlag;
    private bool releaseButton;

    const float STRAINED_END_RATIO = 1.0f;

    private void Init()
    {
        countTime = 0.0f;
        bulletVecs = new Queue<Vector3>();
        finishFlag = false;
        releaseButton = false;

        PlayerScript.refState = EnumPlayerState.SHOT;
        PlayerScript.shotState = ShotState.GO;
        PlayerScript.canShotState = false;
        PlayerScript.forciblyReturnBulletFlag = false;
        PlayerScript.addVel = Vector3.zero;

        PlayerScript.vel *= 0.4f;
        PlayerScript.animator.SetTrigger("shotTrigger");   
    }

    public PlayerStateShot(bool is_slide_jump)//コンストラクタ
    {
        Init();
        //弾の発射
        BulletScript.GetComponent<Collider>().isTrigger = false;
        BulletScript.VisibleBullet();

        if (is_slide_jump)
        {
            BulletScript.ShotSlideJumpBullet();
            Debug.Log("Slide Shot");
        }
        else
        {
            BulletScript.ShotBullet();
            Debug.Log("Normal Shot");
        }

    
    }

    /// <summary>
    /// 引っ張られているとき、プレイヤーを進行方向に対して回転
    /// </summary>
    public void RotationPlayer()
    {

        switch (PlayerScript.shotState)
        {
            case ShotState.STRAINED:

                Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;

                Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);
                Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //補正用クオータニオン
                quaternion *= adjustQua;
                PlayerScript.rb.rotation = quaternion;
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
    /// 引っ張られている時間にオブジェクトがあったら切り離し
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

    public override void UpdateState()
    {
        countTime += Time.deltaTime;

        if (countTime > 0.2)
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

                    PlayerScript.vel = bulletVecs.Dequeue() * STRAINED_END_RATIO;

                    PlayerScript.useVelocity = true;
                    PlayerScript.shotState = ShotState.RETURN;
                    PlayerScript.animator.SetTrigger("returnTrigger");
                }
            }
        }

        //アンカーが刺さらない壁にあたったときなど、外部契機で引き戻しに移行
        if (PlayerScript.forciblyReturnBulletFlag)
        {
            PlayerScript.forciblyReturnBulletFlag = false;

            if (PlayerScript.forciblyReturnSaveVelocity)
            {
                PlayerScript.vel = bulletVecs.Dequeue() * STRAINED_END_RATIO;
            }
            else
            {
                PlayerScript.vel = Vector3.zero;
            }

            BulletScript.ReturnBullet();

            PlayerScript.useVelocity = true;
            PlayerScript.shotState = ShotState.RETURN;
            PlayerScript.animator.SetTrigger("returnTrigger");
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

        if (BulletScript.isTouched)
        {
            if (BulletScript.followEnd)
            {
                BulletScript.FollowedPlayer();

                PlayerScript.vel = bulletVecs.Dequeue();
                PlayerScript.useVelocity = true;
                BulletScript.followEnd = false;
                PlayerScript.shotState = ShotState.FOLLOW;
            }
        }

    }

    public override void Move()
    {
        float interval;
        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);

        RotationPlayer();

        switch (PlayerScript.shotState)
        {

            case ShotState.GO:
                bulletVecs.Enqueue(BulletScript.vel * 0.6f);

                //紐の長さを超えたら引っ張られている状態にする
                if (interval > BulletScript.BULLET_ROPE_LENGTH)
                {
                    //引っ張られたタイミングでボール減速
                    //if(BulletScript.vel.magnitude > 60.0f)
                    //{
                    //    BulletScript.vel *= 0.64f;
                    //}
                    //else if(BulletScript.vel.magnitude > 40.0f)
                    //{
                    //    BulletScript.vel *= 0.92f;
                    //}

                    BulletScript.vel *= 0.84f;

                    PlayerScript.shotState = ShotState.STRAINED;
                    //PlayerScript.useVelocity = false;
                }
                break;

            case ShotState.STRAINED:

                bulletVecs.Enqueue(BulletScript.vel);
                bulletVecs.Dequeue();
                StrainedStop();
                //このとき、移動処理は直にposition変更しているため???????、update内に記述
                //ここに記述するとカメラがブレる

                if (interval < 6.0f)
                {
                    Debug.Log("aaa");
                    if (BulletScript.vel.y < -2.0f)
                    {
                        Debug.Log("eee");
                        PlayerScript.ForciblyReturnBullet(true);
                    }
                }

                break;

            case ShotState.RETURN:
                //自分へ弾を引き戻す
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

                PlayerScript.vel += vecToBullet * 3;

                if (interval < 4.0f)
                {
                    finishFlag = true;
                    PlayerScript.animator.SetTrigger("returnTrigger");
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
            
            if (BulletScript.swingEnd)
            {
                BulletScript.swingEnd = false;
                if (PlayerScript.AutoRelease)
                {
                    PlayerScript.mode = new PlayerStateSwing_2();
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

