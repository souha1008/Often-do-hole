using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Barrel : PlayerState
{
    private float WaitTime;     // ���˂܂ł̑ҋ@����
    private Vector3 JumpPower;  // �W�����v��
    private Vector3 GimmickPos; // �M�~�b�N�̍��W
    private float NowTime;      // ���݂̌o�ߎ���
    private Vector3 StartPlayerPos; // �M�~�b�N�ɐG�ꂽ�u�Ԃ̃v���C���[���W
    private bool StateChangeFlag;   // �X�e�[�g�ڍs�t���O
    BulletMain BulletScript;        // �o���b�g�X�N���v�g
    private static float WaitTimeMin = 0.25f;    // �Œ�ҋ@����
    public PlayerState_Barrel(float waittime, Vector3 jumppower, Vector3 gimmickpos) // �R���X�g���N�^
    {
        if (waittime <= WaitTimeMin)                           // ���˂܂ł̑ҋ@���ԃZ�b�g( WaitTimeMin �ȉ��� WaitTimeMin �ɂ���)
            WaitTime = WaitTimeMin;          
        else 
            WaitTime = waittime;
        JumpPower = jumppower;                          // �W�����v�̓Z�b�g
        GimmickPos = gimmickpos;                        // �M�~�b�N�̍��W�Z�b�g
        NowTime = 0.0f;                                 // ���ԃ��Z�b�g
        StartPlayerPos = PlayerScript.transform.position;     // �G�ꂽ�u�Ԃ̍��W�Z�b�g
        StateChangeFlag = false;
        PlayerScript.vel = Vector3.zero;                // �ړ��ʃ[��
        PlayerScript.canShotState = false;              // �e�łĂȂ�
        PlayerScript.addVel = Vector3.zero;             // �M�~�b�N�ł̈ړ��ʃ[��
        PlayerScript.ForciblyReturnBullet(false);       // �e�̈����߂�
        if (PlayerScript.Bullet != null)
            BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //�o���b�g���̃X�i�b�v(�e�����Ƃ��͎Q�Ƃ��Ȃ�)
        else
            BulletScript = null;
    }

    public override void UpdateState() // Update
    {
        NowTime += Time.deltaTime;
    }

    public override void Move() // FixedUpdate
    {
        // �ړ�
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
            if (BulletScript != null)
                GameObject.Destroy(BulletScript.gameObject);
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
            if (BulletScript != null)
            {
                //if (PlayerScript.forciblyReturnSaveVelocity)
                //{
                //    PlayerScript.vel = bulletVecs.Dequeue();
                //}
                //else
                //{
                //    PlayerScript.vel = Vector3.zero;
                //}
                PlayerScript.vel = Vector3.zero;

                BulletScript.ReturnBullet();
                PlayerScript.useVelocity = true;
                PlayerScript.shotState = ShotState.RETURN;
            }
        }
    }

    public override void StateTransition() // �V�[���ړ�
    {
        if (StateChangeFlag)
        {
            PlayerScript.shotState = ShotState.NONE;
            PlayerScript.mode = new PlayerStateMidair(1.0f);
        } 
    }
}
