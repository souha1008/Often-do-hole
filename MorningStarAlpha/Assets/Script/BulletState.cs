using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnumBulletState
{
    READY = 0,
    GO,
    STOP,
    RETURN,
    //BulletReturnFollow
}


// �d�p�̃X�e�[�g
public abstract class BulletState
{
    virtual public void Update() { }      // Update�Ŏg��
    virtual public void Move() { } // FixedUpdate�Ŏg��
    virtual public void StateTransition() { }  //�p����ŃV�[���̈ړ������߂�
    virtual public void DebugMessage() { }     //�f�o�b�O�p�̃��b�Z�[�W�\��


    static public PlayerMain PlayerScript;
    static public BulletMain BulletScript;

    protected void AdjustBulletPos()
    {
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;
        vec = vec * 1.0f;
        vec.y += 3.0f;
        Vector3 adjustPos = PlayerScript.rb.position + vec;

        BulletScript.rb.position = adjustPos;
    }
}


// ��Ɏ����Ă�X�e�[�g(���̃X�e�[�g�ȊO���˕s��)
public class BulletReady : BulletState
{
    public BulletReady()
    {
        // ������
        BulletScript.NowBulletState = EnumBulletState.READY;
        BulletScript.InvisibleBullet();
        BulletScript.isTouched = false;
        BulletScript.co.enabled = false;
        BulletScript.co.isTrigger = true;
        BulletScript.CanShotFlag = true;

        BulletScript.co.radius = BulletScript.DefaultAnchorRadius * 0.5f;
    }

    public override void Move()
    {
        // �o���b�g�̈ʒu����ɃX�e�B�b�N�����ɒ���
        AdjustBulletPos();
    }
}


// ���˒��X�e�[�g
public class BulletGo : BulletState
{
    private int ExitFlameCnt;//���݂��n�߂Ă���̃J�E���g
    float radiusBigger;
    public BulletGo()
    {
        // ������
        ExitFlameCnt = 0;//���݂��n�߂Ă���̃J�E���g
        radiusBigger = 0.5f;
        BulletScript.NowBulletState = EnumBulletState.GO;
        BulletScript.VisibleBullet();
        BulletScript.co.enabled = true;
        BulletScript.co.isTrigger = false;
        BulletScript.CanShotFlag = false;

        AdjustBulletPos();

        BulletScript.ShotBullet();   
        // ���ˉ��Đ�
        SoundManager.Instance.PlaySound("sound_12_�`�F�[���L�т�SE", 1.0f, 0.03f);
    }

    public override void Move()
    {
        radiusBigger = Mathf.Min(radiusBigger + 0.1f, 1.0f);
        BulletScript.co.radius = BulletScript.DefaultAnchorRadius * radiusBigger;

        Debug.LogWarning(radiusBigger + "aaaa");
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

    // BulletMain�� OnCollisionEnter ����
    // ���ABulletGo�X�e�[�g�œ��������^�O��" "�Ȃ�X�e�[�g�ς���(BulletStop  or BulletReturn)
}


// ���������X�e�[�g
public class BulletStop : BulletState
{
    public BulletStop()
    {
        // ������
        
        BulletScript.NowBulletState = EnumBulletState.STOP;
        BulletScript.co.enabled = false;
        BulletScript.CanShotFlag = false;
        BulletScript.co.isTrigger = true;
        BulletScript.StopBullet();
    }

    public override void Move()
    {
        //BulletScript.RotateBullet();
    }
}


// �����߂��X�e�[�g
public class BulletReturn : BulletState
{
    float ratio;
    Vector3 maxPos;

    public BulletReturn()
    {
        // ������
        BulletScript.NowBulletState = EnumBulletState.RETURN;

        BulletScript.rb.velocity = Vector3.zero;
        BulletScript.vel = Vector3.zero;
        BulletScript.rb.isKinematic = false;
        BulletScript.co.enabled = false;
        BulletScript.co.isTrigger = true;
        BulletScript.StopVelChange = true;
        BulletScript.CanShotFlag = false;

        ratio = 0.0f;

        float dist = Vector3.Distance(BulletScript.rb.position, PlayerScript.rb.position);
        maxPos = PlayerScript.rb.position + ((BulletScript.rb.position - PlayerScript.rb.position).normalized * dist);

        // �����������Đ�
        SoundManager.Instance.PlaySound("sound_18_��������SE", 1.0f, 0.03f);
    }

    public override void Move()
    {

#if true
        ratio += 0.06f;
        float easeRatio = Easing.Linear(ratio, 1.0f, 0.0f, 1.0f);
        //�e�Ǝ����̈ʒu�ŕ⊮ 
        Vector3 BulletPosition = maxPos * (1 - easeRatio) + PlayerScript.rb.position * easeRatio;
        BulletScript.rb.position = BulletPosition;

        //���������ȉ��ɂȂ�����I��
        if (easeRatio >= 0.8f)
        {
            BulletScript.SetBulletState(EnumBulletState.READY);
        }

#else
        //�����֒e�������߂�
        Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
        vecToPlayer = vecToPlayer.normalized;
        BulletScript.vel = vecToPlayer * 200;

        //���������ȉ��ɂȂ�����I��
        if (Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position) < 4.0f)
        {
            BulletScript.SetBulletState(EnumBulletState.READY);
        }

#endif
    }
}


//// �d�Ƀv���C���[�����������ĉ��
//public class BulletReturnFollow : BulletState
//{
//    public BulletReturnFollow()
//    {
//        // ������
//        BulletScript.NowBulletState = EnumBulletState.BulletReturnFollow;
//        BulletScript.ReturnBullet();
//    }

//    public override void Move()
//    {
//        //�����֒e�������߂�
//        Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
//        vecToPlayer = vecToPlayer.normalized;
//        BulletScript.vel = vecToPlayer * 4;


//        //���������ȉ��ɂȂ�����I��
//        if (Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position) < 4.0f)
//        {
//            BulletScript.SetBulletState(EnumBulletState.READY);
//        }
//    }
//}
