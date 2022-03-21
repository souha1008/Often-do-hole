using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerMoveDir
{
    LEFT,
    RIGHT,
}

/// <summary>
/// �v���C���[�̃X�e�[�g
/// �ق��I�u�W�F�N�g�̓ǂݎ��p
/// </summary>
public enum EnumPlayerState 
{ 
    ON_GROUND, //�n��ɂ���
    SHOT,      //�e�������Ă�����
    MIDAIR,�@�@//�󒆂ɂ��Ēe�������Ă��Ȃ�
    SWING,     //�U��q���
    DEATH,     //���S���
}


public class PlayerMain : MonoBehaviour
{
    [System.NonSerialized] public Rigidbody rb;      // [System.NonSerialized] �C���X�y�N�^��ŕ\�����������Ȃ�
    [System.NonSerialized] public static PlayerMain instance;
    public GameObject BulletPrefab;
    public PlayerState mode;                         // �X�e�[�g
   

    [SerializeField, Tooltip("�`�F�b�N�������Ă�������͕���")] private bool SplitStick;        //����Ƀ`�F�b�N�������Ă����番��
    [SerializeField, Tooltip("�X�e�B�b�N������␳����i�v�f���ŕ����j\n�l�͏オ0�Ŏ��v���ɑ����B0~360�͈̔�")] private float[] AdjustAngles;   //�X�e�B�b�N������␳����i�v�f���ŕ����j�l�͏オ0�Ŏ��v���ɑ����B0~360�͈̔�



    //----------���v���C���[���������֘A�̒萔��----------------------
    [Range(0.1f, 1.0f), Tooltip("���E�ړ��J�n�̃X�e�B�b�N�������l")] public float  LATERAL_MOVE_THRESHORD;   // ���荶�E�ړ����̍��X�e�B�b�N�������l
    [Tooltip("����ō����x")] public float                      MAX_RUN_SPEED;           // ����ō����x
    [Tooltip("����Œᑬ�x�i��������瑬�x0�j")] public float   MIN_RUN_SPEED;�@�@�@�@�@ // ����Œᑬ�x�i��������瑬�x0�j
    [Tooltip("�����t���[���ŏオ��X�s�[�h")] public float    ADD_RUN_SPEED;           // �����t���[���ŏオ��X�s�[�h
    [Tooltip("�������x����")] public float                      MAX_FALL_SPEED;          // �d�͂ɂ��Œᑬ�x
    [Tooltip("�󒆂ɂ���Ƃ��̏d�͉����x")] public float        FALL_GRAVITY;            // �v���C���[���󒆂ɂ���Ƃ��̏d�͉����x
    [Tooltip("���������Ă���Ƃ��̏d�͉����x")] public float  STRAINED_GRAVITY;�@�@�@�@// �v���C���[�����������Ă���Ƃ��̏d�͉����x
    [Range(0.1f, 1.0f), Tooltip("�n�㑬�x������")] public float�@RUN_FRICTION;            // ����̌�����


    [Tooltip("�󒆈�t���[���ŏオ��X�s�[�h")] public float                      ADD_MIDAIR_SPEED;        // �󒆈�b�Ԃŏオ��X�s�[�h
    [Range(0.1f, 1.0f), Tooltip("�󒆑��x������")] public float  MIDAIR_FRICTION;         // �󒆂̑��x������
    [Tooltip("�󒆂ōĂы����łĂ�悤�ɂȂ鎞��")]public float                      BULLET_RECAST_TIME;      // �󒆂ōĂы����łĂ�悤�ɂȂ鎞�ԁi�b�j
                                                                                             //----------�v���C���[���������֘A�̒萔�I���----------------------
    [ Header("[�ȉ����s���ϐ��m�F�p�F�ύX�s��]")]

    [ReadOnly, Tooltip("���݂̃X�e�[�g")] public EnumPlayerState refState;                //�X�e�[�g�m�F�p(mode�̒��ɓ����Ă���h���N���X�Œl���ς��)
    [ReadOnly, Tooltip("�C�J���̏��")] public GameObject Bullet = null;
    [ReadOnly, Tooltip("�v���C���[�̌���")] public PlayerMoveDir dir;
    [ReadOnly, Tooltip("�v���C���[�̑��x:���͂ɂ�����")] public Vector3 vel;                              // �ړ����x(inspector��Ŋm�F)
    [ReadOnly, Tooltip("�v���C���[�̑��x:�M�~�b�N�ł̔����ɂ�����")] public Vector3 addVel;                           // �M�~�b�N���Œǉ�����鑬�x
    [ReadOnly, Tooltip("�v���C���[�̑��x:�ړ����ɂ�����")] public Vector3 floorVel;                         // ���������ł̃x���V�e�B
    [ReadOnly, Tooltip("�X�e�B�b�N���͊p")] public Vector2 leftStick;                        // ���X�e�B�b�N
    [ReadOnly, Tooltip("�X�e�B�b�N�̓��͂����ȏ゠�邩�F����ꍇ�͑łĂ�")] public bool stickCanShotRange;                   // �łĂ��Ԃ�
    [ReadOnly, Tooltip("�ˏo�N�[���^�C�����񕜂��Ă��邩")] public bool canShot;                             // �łĂ��Ԃ�
    [ReadOnly, Tooltip("�n�ʂƐڐG���Ă��邩")] public bool isOnGround;                          // �n�ʂɐG��Ă��邩�ionCollision�ŕύX�j
    [ReadOnly, Tooltip("velocity�ł̈ړ���position���ڕύX�ɂ��ړ���")] public bool useVelocity;                         // �ړ���velocity������position�ύX���X�e�[�g�ɂ���Ă͒��ڈʒu��ύX���鎞�����邽��
    [ReadOnly, Tooltip("�����I�ɒe��߂�����t���O")] public bool forciblyReturnBulletFlag;            // �����I�ɒe��߂�����t���O
    [ReadOnly, Tooltip("�����I�ɒe��߂�����Ƃ��Ɍ��݂̑��x��ۑ����邩")] public bool forciblyReturnSaveVelocity;

    void Awake()
    {
        instance = this;
        PlayerState.PlayerScript = this;  //PlayerState���ŎQ�Ƃł���悤�ɂ���
        PlayerState.Player = gameObject;

        //�o���ʒu�̐ݒ�
        if (CheckPointManager.isTouchCheckPos() == true) {
            transform.position = CheckPointManager.GetCheckPointPos();
         }
        rb = GetComponent<Rigidbody>();
        mode = new PlayerStateOnGround(); //�����X�e�[�g
        refState = EnumPlayerState.ON_GROUND;
        Bullet = null;�@�@�@�@�@�@�@�@�@�@ 
        dir = PlayerMoveDir.RIGHT;        //���������ʒu
        vel = Vector3.zero;
        addVel = Vector3.zero;
        floorVel = Vector3.zero;
        leftStick = new Vector2(0.0f, 0.0f);
        stickCanShotRange = false;
        canShot = true;
        isOnGround = false;
        useVelocity = true;

        forciblyReturnBulletFlag = false;
        forciblyReturnSaveVelocity = false;
        rb.sleepThreshold = -1; //���W�b�h�{�f�B���Î~���Ă��Ă�onCollision�n���Ă΂�����
    }

    private void Update()
    {
        InputStick();
        MidAirCheck();
      
        mode.UpdateState();
        mode.StateTransition();
    }

    private void FixedUpdate()
    {
        mode.Move();
        rb.velocity = Vector3.zero;
        if (useVelocity)
        {
            rb.velocity = vel;
        }
        rb.velocity += addVel;
        rb.velocity += floorVel;


        if (Mathf.Abs(addVel.magnitude) > 10.0f)
        {
            addVel *= 0.96f;
        }
        else
        {
            addVel = Vector3.zero;
        }



#if UNITY_EDITOR //unity�G�f�B�^�[��ł̓f�o�b�O���s���i�r���h���ɂ͖��������j
        //mode.DebugMessage();
#endif
    }

    private void LateUpdate()
    {
     
    }

    private void InputStick()
    {
        //������
        leftStick = Vector2.zero;

        //���͎擾
        leftStick.x = Input.GetAxis("Horizontal");
        leftStick.y = Input.GetAxis("Vertical");

        //�X�e�B�b�N�̓��͂����ȏ�Ȃ��ꍇ�͌��ĂȂ�
        if (leftStick.sqrMagnitude > 0.8f)
        {
            stickCanShotRange = true;
        }
        else
        {
            stickCanShotRange = false;
        }



        //�X�e�B�b�N�̊p�x�����߂�
        float rad = Mathf.Atan2(leftStick.x, leftStick.y);
        float degree = rad * Mathf.Rad2Deg;
        if (degree < 0)//���������Ɏ��v����0~360�̒l�ɕ␳
        {
            degree += 360;
        }
        float angle = 0.0f;

        //AjustAngles���̈�ԋ߂��l�ɃX�e�B�b�N��␳
        float minDif = 9999.0f;
        float dif;

        for (int i = 0; i < AdjustAngles.Length; i++)
        {
            dif = Mathf.Abs(AdjustAngles[i] - degree);
            if (dif < minDif)
            {
                minDif = dif;
                angle = AdjustAngles[i];
            }

            dif = Mathf.Abs(AdjustAngles[i] + 360 - degree);
            if (dif < minDif)
            {
                minDif = dif;
                angle = AdjustAngles[i];
            }
        }

        //�p�x��ǂ߂�l�ɒ���
        if (angle > 180)
        {
            angle -= 360;
        }
        angle *= -1;
        angle += 90;
        rad = angle * Mathf.Deg2Rad;

        //�p�x����x�N�g���ɂ���
        Vector3 vec = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        vec = vec.normalized;

        //��������
        if (SplitStick)
        {
            leftStick = vec;
        }
    }

    /// <summary>
    /// �����I�ɒe�������߂�����
    /// </summary>
    /// <param name="saveVelocity">true:�����߂����ɂ��Ƃ̃x���V�e�B��ێ�
    ///�@false:�����߂����ɂ��Ƃ̃x���V�e�B���E��
    /// </param>
    public void ForciblyReturnBullet(bool saveVelocity)
    {
        if (Bullet != null)
        {
            forciblyReturnBulletFlag = true;
            forciblyReturnSaveVelocity = saveVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //�󒆂ŕǂɂԂ������Ƃ����x���Ȃ���
        if(refState == EnumPlayerState.MIDAIR)
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (Mathf.Abs(collision.contacts[i].point.x - transform.position.x) > 0.3f)
                {
                    vel.x *= 0.2f;
                    if(vel.y > 1.0f)
                    {
                        vel.y = 0;
                    }
                    Debug.Log("KILL");
                }
            }
        }
    }


    private void OnCollisionStay(Collision collision)
    { 
        //�ڐG�_�̂����A��ł�����������Β��n����
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].point.y < transform.position.y - 0.6f)
            {
                isOnGround = true;
            }
        }   
    }

    //�󒆂ɂ��邩�𔻒肷��
    //�΂߂̏����Ȃ���ΕK�v�Ȃ�����
    private void MidAirCheck()
    {
        if (isOnGround)
        {
            Ray downRay = new Ray(rb.position, Vector3.down);
            if (Physics.Raycast(downRay, 1.2f) == false)
            {
                isOnGround = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isOnGround)
        {
            isOnGround = false;
        }
    }
}
