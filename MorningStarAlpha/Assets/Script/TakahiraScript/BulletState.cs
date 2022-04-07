using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumBulletState
{
    BulletReady = 0,
    BulletGo,
    BulletStop,
    BulletReturn
}


// �d�p�̃X�e�[�g
public abstract class BulletState
{
    virtual public void Update() { }      // Update�Ŏg��
    virtual public void FixedUpdate() { } // FixedUpdate�Ŏg��
    virtual public void StateTransition() { }  //�p����ŃV�[���̈ړ������߂�
    virtual public void DebugMessage() { }     //�f�o�b�O�p�̃��b�Z�[�W�\��

    static public GameObject Bullet;
    static public PlayerMain PlayerScript;
    static public BulletMain BulletScript;
}


// ��Ɏ����Ă�X�e�[�g(���̃X�e�[�g�ȊO���˕s��)
public class BulletReady : BulletState
{
    public BulletReady()
    {
        // ������
        BulletScript.NowBulletState = EnumBulletState.BulletReady;
        BulletScript.InvisibleBullet();
    }

    public override void Update()
    {
        if (Input.GetButtonDown("Button_R"))
        {
            BulletScript.mode = new BulletGo();
        }
    }
}


// ���˒��X�e�[�g
public class BulletGo : BulletState
{
    private int ExitFlameCnt = 0;//���݂��n�߂Ă���̃J�E���g

    public BulletGo()
    {
        // ������
        BulletScript.NowBulletState = EnumBulletState.BulletGo;
        BulletScript.VisibleBullet();


        // �v���C���[�̈ړ��ʁ@+�@���˃x�N�g���ʁ@�������x�N�g���ʂɂ���
        //if (PlayerScript.vel.x < )

        BulletScript.ShotSlideJumpBullet();
    }

    public override void Update()
    {
        if (Input.GetButton("Button_R"))
        {
            // �d�̓���
            if (!BulletScript.StopVelChange)
            {
                BulletScript.RotateBullet();
                ExitFlameCnt++;
                //�萔�b�ȏ�o���Ă���
                if (ExitFlameCnt > BulletScript.STRAIGHT_FLAME_CNT)
                {
                    //�d�͉��Z
                    BulletScript.vel += Vector3.down * PlayerScript.STRAINED_GRAVITY * (BulletMain.fixedAdjust);
                }

                Mathf.Max(BulletScript.vel.y, BulletMain.BULLET_MAXFALLSPEED * -1);
            }
        }
        else
        {
            BulletScript.mode = new BulletReturn();
        }
    }

    // BulletMain�� OnCollisionEnter ����
    // ���ABulletGo�X�e�[�g�œ��������^�O��" "�Ȃ�X�e�[�g�ς���(BulletStop  or BulletReturn)
}


// ���������X�e�[�g
public class BulletStop : BulletState
{
    public BulletStop()
    {
        // ������
        BulletScript.NowBulletState = EnumBulletState.BulletStop;
    }
}


// �����߂��X�e�[�g
public class BulletReturn : BulletState
{
    public BulletReturn()
    {
        // ������
        BulletScript.NowBulletState = EnumBulletState.BulletReturn;
        BulletScript.ReturnBullet();       
    }

    public override void FixedUpdate()
    {
        //�����֒e�������߂�
        Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
        vecToPlayer = vecToPlayer.normalized;
        BulletScript.vel = vecToPlayer * 100;


        //���������ȉ��ɂȂ�����I��
        if (Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position) < 4.0f)
        {
            BulletScript.mode = new BulletReady();
        }
    }
}
