using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �m�b�N�o�b�N�X�e�[�g
public class PlayerState_Knockback : PlayerState
{
    private static float KnockbackTime = 0.8f;       // �m�b�N�o�b�N����
    private static float KnockbackPowerX = 60.0f;    // �m�b�N�o�b�N�� X
    private static float KnockbackPowerY = 50.0f;    // �m�b�N�o�b�N�� Y


    private float NowTime = 0.0f;               // �o�ߎ���
    private Vector3 HitPos;                     // �q�b�g�����I�u�W�F�N�g�̍��W

    private bool BulletReturnFlag;              // �d�����߂��t���O

    public PlayerState_Knockback(Vector3 HitObjectPos)
    {
        PlayerScript.refState = EnumPlayerState.NOCKBACK;
        NowTime = 0.0f;
        HitPos = HitObjectPos;

        PlayerScript.midairState = MidairState.NORMAL;


        PlayerScript.animator.SetTrigger("NockBack");

        // �d�����߂�
        BulletScript.ReturnBullet();
        PlayerScript.useVelocity = true;
        BulletReturnFlag = true;


        Knockback(); // �m�b�N�o�b�N����
    }

    public override void UpdateState()
    {

    }

    public override void Move()
    {
        // ��������
        PlayerSpeedDown();

        //�����֒e�������߂�
        if (BulletReturnFlag)
        {
            float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
            Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
            vec = vec.normalized;
            BulletScript.vel = vec * 200.0f;
            //���������ȉ��ɂȂ�����e���A�N�e�B�u
            if (interval < 4.0f)
            {
                BulletReturnFlag = false;
            }
        }


        // ���Ԍo�߂ŃX�e�[�g�ύX
        if (NowTime > KnockbackTime && !BulletReturnFlag)
        {
            if (PlayerScript.isOnGround)
            {
                PlayerScript.mode = new PlayerStateOnGround();
            }
            else
            {
                PlayerScript.mode = new PlayerStateMidair(true);
            }
        }
        NowTime += Time.fixedDeltaTime;
    }


    // �m�b�N�o�b�N����
    private void Knockback()
    {
        // �m�b�N�o�b�N�����w��
        Vector3 Vec = Player.transform.position - HitPos;

        // �L�����̉�]
        if (Vec.x < 0)
        {
            PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 90, 0));
        }
        else
        {
            PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 270, 0));
        }

        // �m�b�N�o�b�N�ړ�
        if (Vec.x < 0)
        {
            PlayerScript.vel = new Vector3(-KnockbackPowerX, KnockbackPowerY, 0);
        }
        else
        {
            PlayerScript.vel = new Vector3(KnockbackPowerX, KnockbackPowerY, 0);
        }
    }


    // ��������(PlayerStateMidair�@Move()�@������p)
    private void PlayerSpeedDown()
    {
        //�}�~�����͉��H
        if (PlayerScript.sourceLeftStick.y < -0.7f && Mathf.Abs(PlayerScript.sourceLeftStick.x) < 0.3f)
        {
            //��x�ł����͂��ꂽ��i�v��
            PlayerScript.midairState = MidairState.FALL;
        }


        //����S
        PlayerScript.vel.x *= PlayerScript.MIDAIR_FRICTION;
        if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * (fixedAdjust);
        }
        else if (PlayerScript.adjustLeftStick.x < -PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * -1 * (fixedAdjust);
        }

        //�}�~����
        if (PlayerScript.midairState == MidairState.FALL)
        {
            //�v���C���[����Ɍ������Ă���Ƃ��͑���
            if (PlayerScript.vel.y > 0.0f)
            {
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 2.0f * (fixedAdjust);
            }
            else�@//���̂Ƃ�����������
            {
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 1.5f * (fixedAdjust);
            }

            PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1 * 1.3f);
        }
        //���R����
        else if (PlayerScript.midairState == MidairState.NORMAL)
        {
            PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * (fixedAdjust);
            PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
        }
    }
}
