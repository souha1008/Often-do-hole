using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_CannonChild_Killer : Gimmick_CannonChild
{
    private bool BulletMoveFlag;

    [Label("������")]
    public FALL_TYPE FallType;

    [Label("��������")]
    public float FallLength = 15.0f;    // �����鋗��

    [Label("���b�����ė������邩")]
    public float FallTime = 3.0f;       // ���b�����ė������邩


    private bool NowFall;               // ��������
    private float NowTime;              // �o�ߎ���
    private float StartPosY;            // �������W
    private float FallPosY;            // �������W
    private float OldFallPosY;            // �P�O�̗������W

    public override void Init()
    {
        // ������
        Cd.isTrigger = true;
        BulletMoveFlag = false;

        NowFall = false;
        NowTime = 0.0f;
        StartPosY = FallPosY = OldFallPosY = this.gameObject.transform.position.y;

        Rb.mass = 1000000000;   // �d���ύX
    }

    public override void FixedMove()
    {
        base.FixedMove();
       
        // �����ړ�
        if (NowFall)
        {
            switch (FallType)
            {
                case FALL_TYPE.SINE_IN:
                    FallPosY = Easing.SineIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.QUAD_IN:
                    FallPosY = Easing.QuadIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.CUBIC_IN:
                    FallPosY = Easing.CubicIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.QUART_IN:
                    FallPosY = Easing.QuartIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.QUINT_IN:
                    FallPosY = Easing.QuintIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.EXPO_IN:
                    FallPosY = Easing.ExpIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.BOUNCE_IN:
                    FallPosY = Easing.BounceIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
                case FALL_TYPE.ELASTIC_IN:
                    FallPosY = Easing.ElasticIn(NowTime, FallTime, StartPosY, StartPosY - FallLength);
                    break;
            }

            // �ړ��ʍX�V
            Vel.y += (FallPosY - OldFallPosY) * 1 / Time.fixedDeltaTime;

            OldFallPosY = FallPosY;

            // ���ԍX�V
            NowTime += Time.fixedDeltaTime;

            if (NowTime > FallTime)
            {
                Death();
                //Debug.LogWarning("�����o�߂Ŏ��S");
            }
        }


        // �d�I�u�W�F�N�g�̈ړ�
        if (BulletMoveFlag)
        {
            if (PlayerMain.instance.BulletScript.isTouched)
            {
                PlayerMain.instance.BulletScript.transform.position += Vel * Time.fixedDeltaTime;
                PlayerMain.instance.addVel = Vel;

                //PlayerMain.instance.addVel =
                //    new Vector3(this.gameObject.transform.position.x - OldPos.x, this.gameObject.transform.position.y - OldPos.y, 0) * 1 / Time.deltaTime;
            }
            else
            {
                BulletMoveFlag = false;
                PlayerMain.instance.addVel = Vector3.zero;
            }
        }
    }

    public override void Death()
    {
        // �v���C���[�̕d�����߂�
        if (BulletMoveFlag)
        {
            PlayerMain.instance.mode = new PlayerStateMidair(true, MidairState.NORMAL);
            PlayerMain.instance.ForciblyReleaseMode(true);
            PlayerMain.instance.endSwing = true;
            PlayerMain.instance.floorVel = Vector3.zero;
        }

        Init();

        // �G�t�F�N�g
        EffectManager.Instance.SharkExplosionEffect(this.transform.position, 4.0f);

        // ��A�N�e�B�u��
        this.gameObject.SetActive(false);
    }

    public override void OnTriggerEnter(Collider collider)
    {
        // �v���C���[�ƏՓ�(���̃I�u�W�F�N�g�Ɏh�����ĂȂ�)
        if (collider.gameObject.CompareTag("Player") && !BulletMoveFlag)
        {
            if (PlayerMain.instance.refState == EnumPlayerState.SWING)
            {
                // ���g�����S
                Death();
            }
            // �m�b�N�o�b�N
            else
            {
                // �q�b�g�X�g�b�v
                //GameSpeedManager.Instance.StartHitStop(0.1f);

                // �U��
                VibrationManager.Instance.StartVibration(0.7f, 0.7f, 0.2f);

                // �v���C���[���m�b�N�o�b�N��ԂɕύX
                PlayerMain.instance.mode = new PlayerState_Knockback(this.transform.position, false);

                //Debug.LogWarning("���S:" + collider.gameObject.name);

                // ���g�����S
                Death();
            }
            return;
        }
        
        // �ǂƂ��ɏՓ�
        if (!(collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Bullet") || collider.gameObject.CompareTag("Chain")))
        {
            //Debug.LogWarning("���S:" + collider.gameObject.name);

            // ���g�����S
            Death();
            return;
        }
    }

    public override void GimmickBulletStart(GameObject collision)
    {
        if (collision.gameObject == this.gameObject)
        {
            BulletMoveFlag = true;
            StartPosY = FallPosY = this.gameObject.transform.position.y;
            NowFall = true;
        }
    }
}
