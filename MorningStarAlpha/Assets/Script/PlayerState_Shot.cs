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
    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //���˂���̒e��vector��ۑ�����
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

    public PlayerStateShot(bool is_slide_jump)//�R���X�g���N�^
    {
        Init();
        //�e�̔���
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
    /// ���������Ă���Ƃ��A�v���C���[��i�s�����ɑ΂��ĉ�]
    /// </summary>
    public void RotationPlayer()
    {

        switch (PlayerScript.shotState)
        {
            case ShotState.STRAINED:

                Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;

                Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);
                Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //�␳�p�N�I�[�^�j�I��
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
    /// ���������Ă��鎞�ԂɃI�u�W�F�N�g����������؂藣��
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

                    PlayerScript.vel = bulletVecs.Dequeue() * STRAINED_END_RATIO;

                    PlayerScript.useVelocity = true;
                    PlayerScript.shotState = ShotState.RETURN;
                    PlayerScript.animator.SetTrigger("returnTrigger");
                }
            }
        }

        //�A���J�[���h����Ȃ��ǂɂ��������Ƃ��ȂǁA�O���_�@�ň����߂��Ɉڍs
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

                //�R�̒����𒴂�������������Ă����Ԃɂ���
                if (interval > BulletScript.BULLET_ROPE_LENGTH)
                {
                    //��������ꂽ�^�C�~���O�Ń{�[������
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
                //���̂Ƃ��A�ړ������͒���position�ύX���Ă��邽��???????�Aupdate���ɋL�q
                //�����ɋL�q����ƃJ�������u����

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
                //�����֒e�������߂�
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
        //�{�[�����G�ꂽ��X�C���O���
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

