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
/// �e�����������(��x�R���L�ѐ؂����璷���Œ�̂���)
/// �e�̓I�u�W�F�N�g�ɂ͐ڐG���Ă��Ȃ�
/// �X�e�B�b�N�ł̈ړ��s�A�e�������߂����Ƃ̂݉\
/// </summary>
public class PlayerStateShot : PlayerState
{
    float countTime;               //���˂���̎���

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
        
        //�A�j���[�V�����p
        if (Mathf.Abs(PlayerScript.adjustLeftStick.y) < 0.1f)
        {
            //������
            PlayerScript.animator.SetInteger("shotdirType", 1);
        }
        else
        {
            //�΂ߓ���
            PlayerScript.animator.SetInteger("shotdirType", 2);
        }

        //�A�j���[�V�����E��]�p�@�^��
        if(Mathf.Abs(PlayerScript.adjustLeftStick.x) < 0.1f)
        {
            shotDir = ShotDir.UP;
        }
        else
        {
            shotDir = ShotDir.LATERAL;
        }
    }

    //����
    public PlayerStateShot()//�R���X�g���N�^
    {
        Init();
        //�e�̔���
        BulletScript.GetComponent<Collider>().isTrigger = false;
        BulletScript.VisibleBullet();

      
        BulletScript.ShotBullet();

        Vecs.Enqueue(BulletScript.vel / BulletScript.BULLET_SPEED_MULTIPLE);
    }

    /// <summary>
    /// ���������Ă���Ƃ��A�v���C���[��i�s�����ɑ΂��ĉ�]
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
                    Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //�␳�p�N�I�[�^�j�I��
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
    /// ���������Ă��鎞�A�ԂɃI�u�W�F�N�g����������؂藣��
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

            //returnVec�����̒l�ɕ␳
            float minVecPower = Mathf.Min(returnVec.magnitude, 60.0f);
            returnVec = returnVec * (60.0f / minVecPower);

            //���͕����ɂ��␳
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
                    if (Input.GetButtonUp("Button_R")) //�{�^��������Ă�����
                    {
                        releaseButton = true;
                    }
                }
                else
                {
                    if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
                    {
                        releaseButton = true;
                    }
                }

                if (releaseButton) //�{�^��������Ă�����
                {
                    releaseButton = false;
                    BulletScript.ReturnBullet();

                    PlayerScript.vel = ReleaseForceCalicurate();

                    PlayerScript.useVelocity = true;
                    PlayerScript.shotState = ShotState.RETURN;
                }
            }
        }

        //�A���J�[���h����Ȃ��ǂɂ��������Ƃ��ȂǁA�O���_�@�ň����߂��Ɉڍs
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

        //���Ă�������
        if (PlayerScript.shotState == ShotState.STRAINED)
        {
#if false
            //�e����v���C���[������BULLET_ROPE_LENGTH�������ꂽ�ʒu�ɏ�ɕ␳
            Vector3 diff = (PlayerScript.transform.position - BulletScript.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
            Player.transform.position = BulletScript.transform.position + diff;
#else
            //�e����v���C���[������BULLET_ROPE_LENGTH�������ꂽ�ʒu�ɏ�ɕ␳
            if (Vector3.Magnitude(Player.transform.position - BulletScript.transform.position) > BulletScript.BULLET_ROPE_LENGTH)
            {
                Vector3 diff = (PlayerScript.transform.position - BulletScript.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
                Player.transform.position = BulletScript.transform.position + diff;
                //�e���v���C���[��苭�������������Ă���Ƃ��̂�
                if (PlayerScript.vel.magnitude < BulletScript.vel.magnitude * 0.8f)
                {
                    PlayerScript.vel = BulletScript.vel * 0.8f;
                }
            }

            //STRAINED�����ǎ��R�ړ��̃^�C�~���O
            else
            {
                //��߂̏d�� 
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 0.1f * (fixedAdjust);
                PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
            }
#endif
        }

        //follow�J�n
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
                //�R�̒����𒴂�������������Ă����Ԃɂ���
                if (interval > BulletScript.BULLET_ROPE_LENGTH)
                {
                    //��������ꂽ�^�C�~���O�Ń{�[������
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
                //���̂Ƃ��A�ړ������͒���position�ύX���Ă��邽��???????�Aupdate���ɋL�q
                //�����ɋL�q����ƃJ�������u����

                //�^��p�L���b�`����
                if (interval < 6.0f)
                {
                    PlayerScript.ForciblyReturnBullet(true);
                }

                break;

            case ShotState.RETURN:
                //�����֒e�������߂�
                if (onceAnimReturn == false)
                {
                    onceAnimReturn = true;
                    PlayerScript.animator.SetBool("isShot", false);
                }

                Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
                vecToPlayer = vecToPlayer.normalized;

                BulletScript.vel = vecToPlayer * 100;

                //���������ȉ��ɂȂ�����I�������t���O�����Ă�
                if (interval < 4.0f)
                {
                    finishFlag = true;
                }
                break;

            case ShotState.FOLLOW:
                //�����֒e�������߂�
                Vector3 vecToBullet = BulletScript.rb.position - PlayerScript.rb.position;
                vecToBullet = vecToBullet.normalized;

                PlayerScript.vel += vecToBullet * 2;
                maxFollowAddvec += vecToBullet * 2;

                //���܂��ăx�N�g�������܂��Ă��܂����ꍇ�؂藣��
                if(maxFollowAddvec.magnitude > 80)
                {
                    PlayerScript.vel *= 0.5f;
                    PlayerScript.ForciblyReturnBullet(true);
                    Debug.Log("FOLLOW END : over 80");
                }
                //�{�[���Ɏ������Ȃ�����������؂藣���i��]�o�O�h�~�j
                Vector3 nowDiff = BulletScript.colPoint - PlayerScript.rb.position;
                if (followStartdiff.x * nowDiff.x < 0 || followStartdiff.y * nowDiff.y < 0)
                {
                    PlayerScript.ForciblyReturnBullet(true);
                    Debug.Log("FOLLOW END : �������Ȃ�");
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
        //�{�[�����G�ꂽ��X�C���O���
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

            //���n�����痧���Ă����ԂɈڍs
            if (PlayerScript.isOnGround)
            {
                PlayerScript.mode = new PlayerStateOnGround();
            }
            else //�����łȂ��Ȃ��
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

