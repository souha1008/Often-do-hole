using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �e�����������(��x�R���L�ѐ؂����璷���Œ�̂���)
/// �e�̓I�u�W�F�N�g�ɂ͐ڐG���Ă��Ȃ�
/// �X�e�B�b�N�ł̈ړ��s�A�e�������߂����Ƃ̂݉\
/// </summary>
public class PlayerStateShot : PlayerState
{
    float countTime;               //���˂���̎���

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

        //�A�j���[�V�����p
        PlayerScript.ResetAnimation();
        PlayerScript.animator.SetBool(PlayerScript.animHash.isShot, true);
    }

    public PlayerStateShot(bool isFollow)//�R���X�g���N�^
    {
        Init();
        //�e�̔���
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
    /// ���������Ă���Ƃ��A�v���C���[��i�s�����ɑ΂��ĉ�]
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
                    Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //�␳�p�N�I�[�^�j�I��
                    quaternion *= adjustQua;
                    PlayerScript.rb.rotation = quaternion;
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

            //returnVec�����̒l�ɕ␳
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

        //�A���J�[���h����Ȃ��ǂɂ��������Ƃ��ȂǁA�O���_�@�ň����߂��Ɉڍs
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

        //follow�J�n
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
                if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
                {
                    releaseButton = true;
                }
                

                if (releaseButton) //�{�^��������Ă�����
                {
                    releaseButton = false;
                    BulletScript.SetBulletState(EnumBulletState.RETURN);

                    PlayerScript.vel = ReleaseForceCalicurate();

                    PlayerScript.useVelocity = true;
                    finishFlag = true;
                }
            }
        }

        //���Ă�������
        if (PlayerScript.shotState == ShotState.STRAINED)
        {
            //�e����v���C���[������BULLET_ROPE_LENGTH�������ꂽ�ʒu�ɏ�ɕ␳
            if (Vector3.Magnitude(PlayerScript.rb.position - BulletScript.rb.position) > BulletScript.BULLET_ROPE_LENGTH)
            {
                Vector3 diff = (PlayerScript.rb.position - BulletScript.rb.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
                PlayerScript.rb.position = BulletScript.rb.position + diff;
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


                //���̂Ƃ��A�ړ������͒���position�ύX���Ă��邽��???????�Aupdate���ɋL�q
                //�����ɋL�q����ƃJ�������u����

                //�^��p�L���b�`����
                if (interval < 6.0f)
                {
                    PlayerScript.ForciblyReleaseMode(true);
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
                    BulletScript.SetBulletState(EnumBulletState.RETURN);
                    Debug.Log("FOLLOW END : over 80");
                    finishFlag = true;
                }
                //�{�[���Ɏ������Ȃ�����������؂藣���i��]�o�O�h�~�j
                Vector3 nowDiff = BulletScript.colPoint - PlayerScript.rb.position;
                if (followStartdiff.x * nowDiff.x < 0 || followStartdiff.y * nowDiff.y < 0)
                {

                    BulletScript.SetBulletState(EnumBulletState.RETURN);
                    Debug.Log("FOLLOW END : �������Ȃ�");
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
        //�{�[�����G�ꂽ��X�C���O���
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

            //���n�����痧���Ă����ԂɈڍs
            if (PlayerScript.isOnGround)
            {
                PlayerScript.mode = new PlayerStateOnGround();
            }
            else //�����łȂ��Ȃ��
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

