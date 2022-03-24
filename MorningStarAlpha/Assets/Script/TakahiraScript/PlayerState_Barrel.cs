using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerState_Barrel : PlayerState
{
    private float WaitTime;     // ���˂܂ł̑ҋ@����
    private Vector3 JumpPower;  // �W�����v��
    private Vector3 GimmickPos; // �M�~�b�N�̍��W
    private bool isPlayerTouch; // �v���C���[���G�������e���G������
    private float NowTime;      // ���݂̌o�ߎ���
    private Vector3 StartPlayerPos; // �M�~�b�N�ɐG�ꂽ�u�Ԃ̃v���C���[���W
    private Vector3 StartBulletPos; // �M�~�b�N�ɐG�ꂽ�u�Ԃ̒e���W
    private bool StateChangeFlag;   // �X�e�[�g�ڍs�t���O
    BulletMain BulletScript;        // �o���b�g�X�N���v�g
    private static float WaitTimeMin = 0.25f;    // �Œ�ҋ@����


    //==================================================================
    // �����Fwaittime = ���˂܂ł̑ҋ@����
    //     �Fjumppower = �W�����v��
    //     �Fgimmickpos = �����񂹂���W
    //     �Fisplayertouch = true:�v���C���[���G����, false:�e���G����
    //==================================================================
    public PlayerState_Barrel(float waittime, Vector3 jumppower, Vector3 gimmickpos, bool isplayertouch) // �R���X�g���N�^
    {
        if (waittime <= WaitTimeMin)                           // ���˂܂ł̑ҋ@���ԃZ�b�g( WaitTimeMin �ȉ��� WaitTimeMin �ɂ���)
            WaitTime = WaitTimeMin;          
        else 
            WaitTime = waittime;
        JumpPower = jumppower;                          // �W�����v�̓Z�b�g
        GimmickPos = gimmickpos;                        // �M�~�b�N�̍��W�Z�b�g
        isPlayerTouch = isplayertouch;                  // �v���C���[���e���Z�b�g
        NowTime = 0.0f;                                 // ���ԃ��Z�b�g
        StartPlayerPos = PlayerScript.transform.position;   // �G�ꂽ�u�Ԃ̍��W�Z�b�g
        StateChangeFlag = false;
        PlayerScript.vel = Vector3.zero;                // �ړ��ʃ[��
        PlayerScript.canShotState = false;              // �e�łĂȂ�
        PlayerScript.addVel = Vector3.zero;             // �M�~�b�N�ł̈ړ��ʃ[��
        PlayerScript.forciblyReturnBulletFlag = true;   // �e�̈����߂�
        if (PlayerScript.Bullet != null)
            BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //�o���b�g���̃X�i�b�v(�e�����Ƃ��͎Q�Ƃ��Ȃ�)
        else
            BulletScript = null;    // �enull
    }

    public override void UpdateState() // Update
    {
        NowTime += Time.deltaTime;
    }

    public override void Move() // FixedUpdate
    {
        // �v���C���[�ړ�
        if (NowTime <= WaitTimeMin) // �Œ�ҋ@���Ԃ������S�Ɍ������Ĉړ�
        {
            PlayerScript.transform.position =
               new Vector3(Easing.QuadInOut(NowTime, WaitTimeMin, StartPlayerPos.x, GimmickPos.x),
               Easing.QuadInOut(NowTime, WaitTimeMin, StartPlayerPos.y, GimmickPos.y), GimmickPos.z);
            Debug.Log("���S�Ɉړ�");
        }
        else // ����ȊO�͒��S�łƂǂ܂�
        {
            PlayerScript.transform.position = GimmickPos;
            PlayerScript.GetComponent<MeshRenderer>().enabled = false; // ���b�V���؂�ւ�
            Debug.Log("���S�łƂǂ܂�");
        }

        // ����
        if (NowTime > WaitTime)
        {
            PlayerScript.addVel = JumpPower;
            PlayerScript.GetComponent<MeshRenderer>().enabled = true; // ���b�V���؂�ւ�
            StateChangeFlag = true;
            Debug.Log("����");
        }

        //�A���J�[���h����Ȃ��ǂɂ��������Ƃ��ȂǁA�O���_�@�ň����߂��Ɉڍs
        if (PlayerScript.forciblyReturnBulletFlag)
        {
            PlayerScript.forciblyReturnBulletFlag = false;
            if (PlayerScript.Bullet != null)
            {
                PlayerScript.vel = Vector3.zero;

                StartBulletPos = BulletScript.transform.position;
                BulletScript.ReturnBullet();
                PlayerScript.useVelocity = true;
                PlayerScript.shotState = ShotState.RETURN;
                //Debug.Log("�e���^�[��");
            }
            else
            {
                PlayerScript.shotState = ShotState.NONE;
                //Debug.Log("�e�m��");
            }
        }

        // �e�̈����߂�
        switch (PlayerScript.shotState)
        {
            case ShotState.RETURN:
                float interval;
                interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);


                if (isPlayerTouch) // �v���C���[����ɐG�ꂽ��
                {
                    //�����֒e�������߂�
                    Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
                    vecToPlayer = vecToPlayer.normalized;

                    BulletScript.vel = vecToPlayer * 100;
                }
                else
                {
                    // �e�ړ�
                    if (NowTime <= WaitTimeMin / 2) // �Œ�ҋ@���Ԃ̔����������S�Ɍ������Ĉړ�
                    {
                        BulletScript.transform.position =
                           new Vector3(Easing.QuadInOut(NowTime, WaitTimeMin / 2, StartBulletPos.x, GimmickPos.x),
                           Easing.QuadInOut(NowTime, WaitTimeMin / 2, StartBulletPos.y, GimmickPos.y), GimmickPos.z);
                    }
                    else // �M�~�b�N�̍��W�ɌŒ�
                    {
                        BulletScript.transform.position = GimmickPos;
                    }
                }

                if (interval < 4.0f || NowTime > WaitTime)
                {
                    if (PlayerScript.Bullet != null)
                        GameObject.Destroy(PlayerScript.Bullet); // �e�j��
                    PlayerScript.shotState = ShotState.NONE;
                }
                break;

            case ShotState.NONE:
                break;

            default:
                break;
        }
    }

    public override void StateTransition() // �V�[���ړ�
    {
        if (StateChangeFlag)
        {
            PlayerScript.mode = new PlayerStateMidair(1.0f);
        } 
    }
}
