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

    private bool isDeath;

    //public PlayerState_Knockback(Vector3 HitObjectPos)
    //{
    //    PlayerScript.refState = EnumPlayerState.NOCKBACK;
    //    NowTime = 0.0f;
    //    HitPos = HitObjectPos;
    //    isDeath = false;
    //    PlayerScript.midairState = MidairState.NORMAL;

    //    PlayerScript.AnimVariableReset();
    //    PlayerScript.animator.SetTrigger(PlayerScript.animHash.NockBack);

    //    // �d�����߂�
    //    BulletScript.ReturnBullet();


    //    Knockback(); // �m�b�N�o�b�N����
    //}

    public PlayerState_Knockback(Vector3 HitObjectPos, bool is_death)
    {
        PlayerScript.refState = EnumPlayerState.NOCKBACK;
        NowTime = 0.0f;
        HitPos = HitObjectPos;
        isDeath = is_death;

        PlayerScript.AnimVariableReset();
        PlayerScript.animator.SetTrigger(PlayerScript.animHash.NockBack);


        if (is_death)
        {
            PlayerScript.animator.SetFloat("NockbackSpeed", 2.0f); 
            PlayerScript.animator.SetBool(PlayerScript.animHash.IsDead, true);
        }
        else
        {
            PlayerScript.animator.SetFloat("NockbackSpeed", 0.15f);
        }

        // �d�����߂�
        BulletScript.ReturnBullet();

        PlaySE();

        Knockback(); // �m�b�N�o�b�N����
    }

    public override void UpdateState()
    {

    }

    void PlaySE()
    {
        int seNum = Random.Range(0, 1);

        switch (seNum)
        {
            case 0:
                //SE
                SoundManager.Instance.PlaySound("CVoice_ (5)", 1.0f);
                break;

            default:
                Debug.LogWarning("random :Out OfRange");
                break;
        }

    }

    public override void Move()
    {
        // ��������
        PlayerSpeedDown();

        ////�����֒e�������߂�
        //if (BulletReturnFlag)
        //{
        //    float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
        //    Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
        //    vec = vec.normalized;
        //    BulletScript.vel = vec * 200.0f;
        //    //���������ȉ��ɂȂ�����e���A�N�e�B�u
        //    if (interval < 4.0f)
        //    { 
        //        BulletReturnFlag = false;
        //    }
        //}
        if (isDeath)
        { // ���Ԍo�߂ŃX�e�[�g�ύX
            if (NowTime > 0.3f)
            {
                //PlayerScript.mode = new PlayerStateDeath_Thorn();
            }
        }
        else
        {
            // ���Ԍo�߂ŃX�e�[�g�ύX
            if (NowTime > KnockbackTime)
            {


                if (PlayerScript.isOnGround)
                {
                    PlayerScript.mode = new PlayerStateOnGround();
                }
                else
                {
                    PlayerScript.mode = new PlayerStateMidair(true, MidairState.NORMAL);
                }

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
        //����S
        PlayerScript.vel.x *= PlayerScript.MIDAIR_FRICTION;
        if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * (fixedAdjust) * 0.2f;
        }
        else if (PlayerScript.adjustLeftStick.x < -PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * -1 * (fixedAdjust) * 0.2f;
        }

        PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * (fixedAdjust);
        PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
       
    }
}
