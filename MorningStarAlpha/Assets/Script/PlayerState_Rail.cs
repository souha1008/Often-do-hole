using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// レール移動時のクラス
/// </summary>
public class PlayerState_Rail : PlayerState
{

    float countTime;               //発射からの時間
    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //発射からの弾のvectorを保存する
    bool finishFlag;
    BulletMain BulletScript;

    GameObject anchor;

    const float STRAINED_END_RATIO = 1.0f;

    private void Init()
    {
        anchor = GameObject.FindGameObjectWithTag("Bullet");
        //countTime = 0.0f;
        //bulletVecs = new Queue<Vector3>();
        //finishFlag = false;
        ////releaseButton = false;

        //PlayerScript.refState = EnumPlayerState.SHOT;
        //PlayerScript.shotState = ShotState.GO;
        //PlayerScript.canShotState = false;
        //PlayerScript.forciblyReturnBulletFlag = false;
        //PlayerScript.addVel = Vector3.zero;
    }

    public PlayerState_Rail()
    {
        Init();
        Player.AddComponent<HingeJoint>().connectedBody = anchor.GetComponent<Rigidbody>();

        ////弾の発射
        //BulletScript.GetComponent<Collider>().isTrigger = false;
        //BulletScript.VisibleBullet();

        ////if (is_slide_jump)
        ////{
        ////    BulletScript.ShotSlideJumpBullet();
        ////    Debug.Log("Slide Shot");
        ////}
        
        //{
        //    BulletScript.ShotBullet();
        //    Debug.Log("Normal Shot");
        //}
    }

    ~PlayerState_Rail()
    {
        
    }

    public override void UpdateState()
    {
        //キー入力不可
    }

    public override void Move()
    {
        float interval;
        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);

        RotationPlayer();

        switch(PlayerScript.shotState)
        {
            case ShotState.GO:
                bulletVecs.Enqueue(BulletScript.vel * 0.6f);

                //紐の長さを超えたら引っ張られる状態にする
                if (interval > BulletScript.BULLET_ROPE_LENGTH)
                {
                    //BulletScript.vel *= 0.84f;

                    PlayerScript.shotState = ShotState.STRAINED;
                }
                break;

            case ShotState.STRAINED:
                bulletVecs.Enqueue(BulletScript.vel);
                bulletVecs.Dequeue();

                break;
        }

    }

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

    public override void StateTransition()
    {
        //終わったらステート

        ////ボールが触れたらスイング状態
        //if (BulletScript.isTouched)
        //{
        //    PlayerScript.shotState = ShotState.NONE;
        //    if (BulletScript.swingEnd)
        //    {
        //        BulletScript.swingEnd = false;
        //        PlayerScript.mode = new PlayerStateSwing();
        //    }
        //}

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
        Debug.Log("PlayerState:Rail");
    }
}


