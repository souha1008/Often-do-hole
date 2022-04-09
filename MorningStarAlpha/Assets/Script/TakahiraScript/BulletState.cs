using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumBulletState
{
    BulletReady = 0,
    BulletGo,
    BulletStop,
    BulletReturn,
    BulletReturnFollow
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
        // ����
        if (BulletScript.CanShotFlag &&
            Input.GetButtonDown("Button_R") &&
            PlayerScript.CanShotColBlock &&
            PlayerScript.stickCanShotRange)
        {
            BulletScript.SetBulletState(EnumBulletState.BulletGo);
            BulletScript.CanShotFlag = false;
        }

        // �o���b�g�̈ʒu����ɃX�e�B�b�N�����ɒ���
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;
        vec = vec * 3;
        vec.y += 1.0f;
        Vector3 adjustPos = PlayerScript.transform.position + vec;

        BulletScript.transform.position = adjustPos;
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
        BulletScript.co.isTrigger = false;


        // ����
        if (Mathf.Abs(PlayerScript.vel.x) > PlayerScript.MAX_RUN_SPEED &&
           ((PlayerScript.vel.x > 0 && PlayerScript.adjustLeftStick.x > 0) ||
           (PlayerScript.vel.x < 0 && PlayerScript.adjustLeftStick.x < 0)))
        {
            BulletScript.ShotSlideJumpBullet(); // �X���C�h�W�����v
            //BulletScript.ShotBullet();
        }
        else
        {
            BulletScript.ShotBullet();
        }      
    }

    public override void Update()
    {
        if (Input.GetButton("Button_R"))
        {
            // �d�̓���
            BulletScript.RotateBullet();
            ExitFlameCnt++;
            //�萔�b�ȏ�o���Ă���
            if (ExitFlameCnt > BulletScript.STRAIGHT_FLAME_CNT)
            {
                //�d�͉��Z
                BulletScript.vel += Vector3.down * PlayerScript.STRAINED_GRAVITY * BulletScript.fixedAdjust;
            }

            Mathf.Max(BulletScript.vel.y, BulletScript.BULLET_MAXFALLSPEED * -1);
        }
        else
        {
            BulletScript.SetBulletState(EnumBulletState.BulletReturn);
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
        BulletScript.vel = Vector3.zero;
        BulletScript.rb.isKinematic = true;
        BulletScript.CanShotFlag = true;
    }

    public override void FixedUpdate()
    {
        if (!Input.GetButton("Button_R"))
        {
            BulletScript.SetBulletState(EnumBulletState.BulletReturn);
        }
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
            BulletScript.SetBulletState(EnumBulletState.BulletReady);
        }
    }
}


// �d�Ƀv���C���[�����������ĉ��
public class BulletReturnFollow : BulletState
{
    public BulletReturnFollow()
    {
        // ������
        BulletScript.NowBulletState = EnumBulletState.BulletReturnFollow;
        BulletScript.ReturnBullet();
    }

    public override void FixedUpdate()
    {
        //�����֒e�������߂�
        Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
        vecToPlayer = vecToPlayer.normalized;
        BulletScript.vel = vecToPlayer * 4;


        //���������ȉ��ɂȂ�����I��
        if (Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position) < 4.0f)
        {
            BulletScript.SetBulletState(EnumBulletState.BulletReady);
        }
    }
}
