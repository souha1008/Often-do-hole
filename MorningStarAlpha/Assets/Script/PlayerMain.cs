#define SPLIT_LEFTSTICK

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
    public EnumPlayerState refState;                //�X�e�[�g�m�F�p(mode�̒��ɓ����Ă���h���N���X�Œl���ς��)
    public GameObject Bullet = null;
    public HingeJoint hinge = null;
    public PlayerMoveDir dir;
    public Vector3 vel;                              // �ړ����x(inspector��Ŋm�F)
    public Vector3 addVel;                           // �M�~�b�N���Œǉ�����鑬�x
    public Vector3 floorVel;                         // ���������ł̃x���V�e�B
    public Vector2 leftStick;                        // ���X�e�B�b�N
    public bool stickCanShotRange;                   // �łĂ��Ԃ�
    public bool canShot;                             // �łĂ��Ԃ�
    public bool isOnGround;                          // �n�ʂɐG��Ă��邩�ionCollision�ŕύX�j
    public bool useVelocity;                         // �ړ���velocity������position�ύX���X�e�[�g�ɂ���Ă͒��ڈʒu��ύX���鎞�����邽��
    public bool forciblyReturnBulletFlag;            // �����I�ɒe��߂�����t���O
    public bool forciblyReturnSaveVelocity;
    [SerializeField] private float[] AdjustAngles;   //�X�e�B�b�N������␳����i�v�f���ŕ����j�l�͏オ0�Ŏ��v���ɑ����B0~360�͈̔�



    //----------���v���C���[���������֘A�̒萔��----------------------
    [Range(0.1f, 1.0f)] public float  LATERAL_MOVE_THRESHORD;  // ���荶�E�ړ����̍��X�e�B�b�N�������l
    public float                      MAX_RUN_SPEED;           // ����ō����x
    public float                      MIN_RUN_SPEED;�@�@�@�@�@ // ����Œᑬ�x�i��������瑬�x0�j
    public float                      ADD_RUN_SPEED;           // �����b�Ԃŏオ��X�s�[�h
    public float                      MAX_FALL_SPEED;          // �d�͂ɂ��Œᑬ�x
    public float                      FALL_GRAVITY;            // �v���C���[���󒆂ɂ���Ƃ��̏d�͉����x
    public float                      STRAINED_GRAVITY;�@�@�@�@// �v���C���[�����������Ă���Ƃ��̏d�͉����x
    [Range(0.1f, 1.0f)] public float�@RUN_FRICTION;            // ����̌�����


    public float                      ADD_MIDAIR_SPEED;        // �󒆈�b�Ԃŏオ��X�s�[�h
    [Range(0.1f, 1.0f)] public float  MIDAIR_FRICTION;         // �󒆂̑��x������
    public float                      BULLET_RECAST_TIME;      // �󒆂ōĂы����łĂ�悤�ɂȂ鎞�ԁi�b�j
    //----------�v���C���[���������֘A�̒萔�I���----------------------

    

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
        Bullet = null;
        hinge = null;�@�@�@�@�@�@�@�@�@�@ 
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

        for (int i = 0;i < AdjustAngles.Length; i++)
        {
             dif = Mathf.Abs(AdjustAngles[i] - degree);
            if(dif< minDif)
            {
                minDif = dif;
                angle = AdjustAngles[i];
            }
        }

        //�p�x��ǂ߂�l�ɒ���
        if(angle > 180)
        {
            angle -= 360;
        }
        angle *= -1;
        angle += 90;
        rad = angle * Mathf.Deg2Rad;

        //�p�x����x�N�g���ɂ���
        Vector3 vec = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        vec = vec.normalized;

#if SPLIT_LEFTSTICK
        //��������
        if (canShot)
        {
            leftStick = vec;
        }
#endif
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
