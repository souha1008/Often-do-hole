using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�̌����Ă������
/// </summary>
public enum PlayerMoveDir
{
    LEFT,
    RIGHT,
}

public enum OnGroundState {
    NONE,
    NORMAL, //�ʏ펞
    SLIDE,  //�����Ă���
}

/// <summary>
/// �󒆎��ׂ̍��ȏ��
/// </summary>
public enum MidairState
{
    NONE,     //�󒆏�Ԃł͂Ȃ�
    NORMAL,   //�ʏ펞
    BOOST,    //�u�[�X�g
}


/// <summary>
/// �X�C���O���ׂ̍��ȏ��
/// </summary>
public enum SwingState
{
    NONE,      //�X�C���O��Ԃł͂Ȃ�
    TOUCHED,   //�߂܂��Ă�����
}

/// <summary>
/// �e�ˏo���ׂ̍��ȏ��
/// </summary>
public enum ShotState
{
    NONE,       //�e�ˏo����Ă��Ȃ�
    GO,         //��������ꂸ�ɔ��ł���
    STRAINED,  //�v���C���[����������Ȃ�����ł���
    FOLLOW,     //�e�ɐ����悭���ł����I��
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
    RAILING,   //���[�����
    NOCKBACK,  //�m�b�N�o�b�N���
    DEATH,     //���S���
    STAN,      //�X�^�����
    CLEAR,     //�N���A���
}

/// <summary>
///�A�j���[�V�����R���g���[���[�p�̕������index�ɂ��Ċi�[
/// </summary>
public struct AnimHash{
    public int onGround;
    public int isRunning;
    public int isShot;
    public int isSwing;
    public int wallKick;
    public int NockBack;
    public int isBoost;
    public int shotdirType;
    public int RunSpeed;
    public int rareWaitTrigger;
    public int rareWaitType;
    public int IsDead;
}


public class PlayerMain : MonoBehaviour
{
    [System.NonSerialized] public Rigidbody rb;      // [System.NonSerialized] �C���X�y�N�^��ŕ\�����������Ȃ�
    [System.NonSerialized] public static PlayerMain instance;
    [System.NonSerialized] public Animator animator;
    [System.NonSerialized] public AnimHash animHash;
    [System.NonSerialized] public GameObject[] animBullet = new GameObject[3];

    public BulletMain BulletScript;
    public PlayerState mode;                         // �X�e�[�g
    private RaycastHit footHit;                      // ���ɓ������Ă�����̂̏��i�[

    [System.NonSerialized] public float colliderRadius = 1.65f;   //�ڒn����pray���a
    [System.NonSerialized] public float coliderDistance = 1.8f; //
                                                                 //
    [System.NonSerialized] public float HcolliderRadius = 2.0f;   //������pray���a
    [System.NonSerialized] public float HcoliderDistance = 0.6f; //������pray���S�_���瓪�܂ł̃I�t�Z�b�g

    [System.NonSerialized] public  float SwingcolliderRadius = 1.5f;   //�X�C���O�X���C�h����pray���a
    [System.NonSerialized] public  float SwingcoliderDistance = 1.75f; //�X�C���O�X���C�hray���S�_���瓪�܂ł̃I�t�Z�b�g

    //----------���v���C���[���������֘A�̒萔��----------------------
    [System.NonSerialized, Tooltip("���E�ړ��J�n�̃X�e�B�b�N�������l")] public float LATERAL_MOVE_THRESHORD = 0.2f;   // ���荶�E�ړ����̍��X�e�B�b�N�������l
    [System.NonSerialized, Tooltip("����ō����x")] public float                      MAX_RUN_SPEED = 30.0f;           // ����ō����x
    [System.NonSerialized, Tooltip("����Œᑬ�x�i��������瑬�x0�j")] public float   MIN_RUN_SPEED = 2.0f;�@�@�@�@�@ // ����Œᑬ�x�i��������瑬�x0�j
    [System.NonSerialized, Tooltip("�����t���[���ŏオ��X�s�[�h")] public float    ADD_RUN_SPEED = 4.0f;           // �����t���[���ŏオ��X�s�[�h
    [System.NonSerialized, Tooltip("�������x����")] public float                      MAX_FALL_SPEED = 70.0f;          // �d�͂ɂ��Œᑬ�x
    [System.NonSerialized, Tooltip("�󒆂ɂ���Ƃ��̏d�͉����x")] public float        FALL_GRAVITY = 2.7f;            // �v���C���[���󒆂ɂ���Ƃ��̏d�͉����x
    [System.NonSerialized, Tooltip("���������Ă���Ƃ��̏d�͉����x")] public float  STRAINED_GRAVITY = 2.5f;�@�@�@�@// �v���C���[�����������Ă���Ƃ��̏d�͉����x
    [System.NonSerialized, Tooltip("�n�㑬�x������")] public float�@RUN_FRICTION = 0.92f;            // ����̌�����


    [System.NonSerialized, Tooltip("�󒆈�t���[���ŏオ��X�s�[�h")] public float                      ADD_MIDAIR_SPEED = 1.5f;        // �󒆈�b�Ԃŏオ��X�s�[�h
    [System.NonSerialized, Tooltip("�󒆑��x������")] public float                   MIDAIR_FRICTION = 0.982f;         // �󒆂̑��x������
    //----------�v���C���[���������֘A�̒萔�I���----------------------

    [ Header("[�ȉ����s���ϐ��m�F�p�F�ύX�s��]")]

    [ReadOnly, Tooltip("���݂̃X�e�[�g")] public EnumPlayerState refState;                //�X�e�[�g�m�F�p(mode�̒��ɓ����Ă���h���N���X�Œl���ς��)
    [ReadOnly, Tooltip("�n�㎞�ׂ̍��ȃX�e�[�g")] public OnGroundState onGroundState;                //�X�e�[�g�m�F�p(mode�̒��ɓ����Ă���h���N���X�Œl���ς��)
    [ReadOnly, Tooltip("�󒆎��ׂ̍��ȃX�e�[�g")] public MidairState midairState;
    [ReadOnly, Tooltip("�V���b�g��Ԃׂ̍��ȃX�e�[�g")] public ShotState shotState;
    [ReadOnly, Tooltip("swing��Ԃׂ̍���state")] public SwingState swingState;
    [ReadOnly, Tooltip("�v���C���[�̌���")] public PlayerMoveDir dir;
    [ReadOnly, Tooltip("�v���C���[�̑��x:���͂ɂ�����")] public Vector3 vel;                              // �ړ����x(inspector��Ŋm�F)
    [ReadOnly, Tooltip("�v���C���[�̑��x:�M�~�b�N�ł̔����ɂ�����")] public Vector3 addVel;                           // �M�~�b�N���Œǉ�����鑬�x
    [ReadOnly, Tooltip("�v���C���[�̑��x:�ړ����ɂ�����")] public Vector3 floorVel;                         // ���������ł̃x���V�e�B
    [ReadOnly, Tooltip("�X�e�B�b�N���͊p�i�����O�j")] public Vector2 sourceLeftStick;                        // ���X�e�B�b�N  
    [ReadOnly, Tooltip("�X�e�B�b�N���͊p�i������j")] public Vector2 adjustLeftStick;                        // ���X�e�B�b�N
    [ReadOnly, Tooltip("�Ō�̂ȓ��͊p")] public float oldStickAngle;                        // ���X�e�B�b�N  
    [ReadOnly, Tooltip("�n�ʂƐڐG���Ă��邩")] public bool isOnGround;                          // �n�ʂɐG��Ă��邩�ionCollision�ŕύX�j
    [ReadOnly, Tooltip("�łĂ�\�������邩")] public bool canShotState;                             // �łĂ��Ԃ�
    [ReadOnly, Tooltip("�X�e�B�b�N�̓��͂����ȏ゠�邩�F����ꍇ�͑łĂ�")] public bool stickCanShotRange;
    [ReadOnly, Tooltip("�ǂ̋߂��ɂ���ꍇ�͌��ĂȂ�")] public bool CanShotColBlock;                           // �X�e�B�b�N���͂̐�ɕǂ�
    [ReadOnly, Tooltip("�ŏI�I�ɑłĂ邩�ǂ���")] public bool canShot;                             // �łĂ��Ԃ�
    [ReadOnly, Tooltip("velocity�ł̈ړ���position���ڕύX�ɂ��ړ���")] public bool useVelocity;                         // �ړ���velocity������position�ύX���X�e�[�g�ɂ���Ă͒��ڈʒu��ύX���鎞�����邽��
    [ReadOnly, Tooltip("�����I�ɒe��߂�����t���O")] public bool forciblyRleaseFlag;            // �����I�ɒe��߂�����t���O
    [ReadOnly, Tooltip("�����I�ɒe��߂�����Ƃ��Ɍ��݂̑��x��ۑ����邩")] public bool forciblyReleaseSaveVelocity;
    [ReadOnly, Tooltip("�����I�ɒe�ɂ��Ă����Ƃ��̃t���O")] public bool forciblyFollowFlag;
    [ReadOnly, Tooltip("�����I�ɒe�ɂ��Ă����Ƃ���velocity�̌�����e�����ɕϊ�����")] public bool forciblyFollowVelToward;
    [ReadOnly, Tooltip("�����I��swing�J�n����t���O")] public bool forciblySwingFlag;
    [ReadOnly, Tooltip("�����I��swing�J�n����t���O")] public bool forciblySwingNextFollow;
    [ReadOnly, Tooltip("�����I��swing�J�n����t���O")] public bool forciblySwingSaveVelocity;
    [ReadOnly, Tooltip("�X�C���O�����I���p")] public bool endSwing;
    [ReadOnly, Tooltip("�X�C���O�Z������p")] public bool SlideSwing;
    [ReadOnly, Tooltip("�X�C���O�Ԃ牺����p")] public bool conuterSwing;
    [ReadOnly, Tooltip("���ˉ�")] public bool recoverBullet;
    public float GameSpeed = 1.0f;


    void Awake()
    {
        instance = this;
        PlayerState.PlayerScript = this;  //PlayerState���ŎQ�Ƃł���悤�ɂ���
        PlayerState.Player = gameObject;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        gameObject.tag = "Player";

        animBullet[0] = transform.Find("anchor_fix3:group12/anchor_fix3:body/anchor_fix3:Anchor_body/anchor_fix3:anchor_body").gameObject;
        animBullet[1] = transform.Find("anchor_fix3:group12/anchor_fix3:body/anchor_fix3:Anchor_body/anchor_fix3:anchor_L_needle").gameObject;
        animBullet[2] = transform.Find("anchor_fix3:group12/anchor_fix3:body/anchor_fix3:Anchor_body/anchor_fix3:anchor_R_needle").gameObject;

        Time.timeScale = GameSpeed;
    }

    private void Start()
    {
        ////�o���ʒu�̐ݒ�
        //transform.position = CheckPointManager.Instance.GetCheckPointPos();

        refState = EnumPlayerState.ON_GROUND;
        onGroundState = OnGroundState.NONE;
        midairState = MidairState.NONE;
        shotState = ShotState.NONE;
        swingState = SwingState.NONE;
        dir = PlayerMoveDir.RIGHT;        //���������ʒu
        rb.rotation = Quaternion.Euler(0, 90, 0);
        vel = Vector3.zero;
        addVel = Vector3.zero;
        floorVel = Vector3.zero;
        sourceLeftStick = adjustLeftStick = new Vector2(0.0f, 0.0f);
        oldStickAngle = -1;
        canShotState = true;
        stickCanShotRange = false;
        CanShotColBlock = false;
        canShot = false;
        isOnGround = true;
        useVelocity = true;

        ClearModeTransitionFlag();
        SetAnimHash();


        endSwing = false;
        SlideSwing = false;
        
        conuterSwing = false;

        Ray footray = new Ray(rb.position, Vector3.down);
        Physics.SphereCast(footray, colliderRadius, out footHit, coliderDistance, LayerMask.GetMask("Platform"));


        rb.sleepThreshold = -1; //���W�b�h�{�f�B���Î~���Ă��Ă�onCollision�n���Ă΂�����

        mode = new PlayerStateOnGround(); //�����X�e�[�g

        if (mode != null)
        {
            mode.UpdateState();
            mode.Animation();
            mode.StateTransition();
            mode.Move();
        }
    }

    public void VisibleAnimBullet(bool on_off)
    {
        for (int i = 0; i < 3; i++) 
        {
            animBullet[i].SetActive(on_off);
        }
    }

    private void Update()
    {
        if (GameStateManager.GetGameState() == GAME_STATE.PLAY && FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
        {
            InputStick_Fixed();
            //InputStick();
            CheckCanShot();
            CheckMidAir();

            if (mode != null)
            {
                mode.UpdateState();
                mode.Animation();
                mode.StateTransition(); 
            }
            else
            {
                Debug.LogError("STATE == NULL");
            }
            
        }
    }

    private void FixedUpdate()
    {
        if (GameStateManager.GetGameState() == GAME_STATE.PLAY && FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
        {
            if (mode != null)
            {
                mode.Move();
            }
            else
            {
                Debug.LogError("STATE == NULL");
            }
            rb.velocity = Vector3.zero;
            if (useVelocity)
            {
                rb.velocity = vel;
            }
            rb.velocity += addVel;
            rb.velocity += floorVel;


            Vector3 resetZo = rb.position;
            resetZo.z = 0.0f;
            rb.position = resetZo;

            if (Mathf.Abs(addVel.magnitude) > 10.0f)
            {
                addVel *= 0.98f;
            }
            else
            {
                addVel = Vector3.zero;
            }

#if UNITY_EDITOR //unity�G�f�B�^�[��ł̓f�o�b�O���s���i�r���h���ɂ͖��������j
                //mode.DebugMessage();
#endif
            
        }
    }

    private void SetAnimHash()
    {
        animHash.onGround = Animator.StringToHash("onGround");
        animHash.isRunning = Animator.StringToHash("isRunning");
        animHash.isShot = Animator.StringToHash("isShot");
        animHash.isSwing = Animator.StringToHash("isSwing");
        animHash.wallKick = Animator.StringToHash("wallKick");
        animHash.NockBack = Animator.StringToHash("NockBack");
        animHash.isBoost = Animator.StringToHash("isBoost");
        animHash.shotdirType = Animator.StringToHash("shotdirType");
        animHash.RunSpeed = Animator.StringToHash("RunSpeed");
        animHash.rareWaitTrigger = Animator.StringToHash("rareWaitTrigger");
        animHash.rareWaitType = Animator.StringToHash("rareWaitType");
        animHash.IsDead = Animator.StringToHash("IsDead");
    }   

    public void AnimVariableReset()
    {
        animator.SetBool(animHash.isShot, false);
        animator.SetBool(animHash.isSwing, false);
        animator.SetBool(animHash.isBoost, false);
        animator.SetBool(animHash.IsDead, false);
    }

    public void AnimTriggerReset()
    {
        animator.ResetTrigger(animHash.wallKick);
        animator.ResetTrigger(animHash.NockBack);
        animator.ResetTrigger(animHash.rareWaitTrigger);
    }

    public void ResetAnimation()
    {
        AnimVariableReset();
        AnimTriggerReset();
    }

    public RaycastHit getFootHit()
    {
        return footHit;
    }

    void InputStick_Fixed()
    {
        //������
        sourceLeftStick = adjustLeftStick = Vector2.zero;

        //���͎擾
        sourceLeftStick.x = adjustLeftStick.x = Input.GetAxis("Horizontal");
        sourceLeftStick.y = adjustLeftStick.y = Input.GetAxis("Vertical");

        //�X�e�B�b�N�̓��͂����ȏ�Ȃ��ꍇ�͌��ĂȂ�
        if (Mathf.Abs(sourceLeftStick.magnitude) > 0.7f)
        {
            stickCanShotRange = true;
        }
        else
        {
            stickCanShotRange = false;
            oldStickAngle = -1;
        }


        float angle = CalculationScript.TwoPointAngle360(Vector3.zero, sourceLeftStick);
        float adjustAngle = 0;
        //angle���Œ�(25.75.285.335)
        

        if(angle == 0)
        {
            if (oldStickAngle == -1)
            {
                if (dir == PlayerMoveDir.RIGHT)
                {
                    adjustAngle = 25;
                    oldStickAngle = 25;
                }
                else if (dir == PlayerMoveDir.LEFT)
                {
                    adjustAngle = 335;
                    oldStickAngle = 335;
                }
            }
            else
            {
                adjustAngle = oldStickAngle;
            }
        }
        else if(angle < 5)
        {
            if(oldStickAngle == -1)
            {
                adjustAngle = 25;
                oldStickAngle = 25;
            }
            else
            {
                adjustAngle = oldStickAngle;
            }
        }
        else if(angle < 45)
        {
            adjustAngle = 25;
            oldStickAngle = 25;
        }
        else if (angle <= 120)
        {
            adjustAngle = 75;
            oldStickAngle = 75;
        }
        else if (angle < 240)
        {
            oldStickAngle = -1;
            adjustAngle = oldStickAngle;
        }
        else if (angle < 315)
        {
            adjustAngle = 285;
            oldStickAngle = 285;
        }
        else if (angle <= 355)
        {
            adjustAngle = 335;
            oldStickAngle = 335;
        }
        else
        {
            if (oldStickAngle == -1)
            {
                adjustAngle = 335;
                oldStickAngle = 335;
            }
            else
            {
                adjustAngle = oldStickAngle;
            }
        }
    
        if(oldStickAngle < 0)
        {
            stickCanShotRange = false;
        }


        //�p�x��ǂ߂�l�ɒ���
        if (adjustAngle > 180)
        {
            adjustAngle -= 360;
        }
        adjustAngle *= -1;
        adjustAngle += 90;
        float rad = adjustAngle * Mathf.Deg2Rad;

        //�p�x����x�N�g���ɂ���
        Vector3 vec = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
        vec = vec.normalized;

        if (stickCanShotRange)
        {
            adjustLeftStick = (Vector2)vec;
        }
        else
        {
            adjustLeftStick = Vector2.zero;
        }
    }

    //private void InputStick()
    //{
    //    //������
    //    sourceLeftStick = adjustLeftStick = Vector2.zero;

    //    //���͎擾
    //    sourceLeftStick.x = adjustLeftStick.x = Input.GetAxis("Horizontal");
    //    sourceLeftStick.y = adjustLeftStick.y = Input.GetAxis("Vertical");

    //    //�X�e�B�b�N�̓��͂����ȏ�Ȃ��ꍇ�͌��ĂȂ�
    //    if (Mathf.Abs(sourceLeftStick.magnitude) > 0.7f)
    //    {
    //        stickCanShotRange = true;
    //    }
    //    else
    //    {
    //        stickCanShotRange = false;
    //    }

    //    //�X�e�B�b�N�̊p�x�����߂�
    //    float rad = Mathf.Atan2(adjustLeftStick.x, adjustLeftStick.y);
    //    float degree = rad * Mathf.Rad2Deg;
    //    if (degree < 0)//���������Ɏ��v����0~360�̒l�ɕ␳
    //    {
    //        degree += 360;
    //    }
    //    float angle = 0.0f;

    //    //AjustAngles���̈�ԋ߂��l�ɃX�e�B�b�N��␳
    //    float minDif = 9999.0f;
    //    float dif;

    //    for (int i = 0; i < AdjustAngles.Length; i++)
    //    {
    //        dif = Mathf.Abs(AdjustAngles[i] - degree);
    //        if (dif < minDif)
    //        {
    //            minDif = dif;
    //            angle = AdjustAngles[i];
    //        }

    //        dif = Mathf.Abs(AdjustAngles[i] + 360 - degree);
    //        if (dif < minDif)
    //        {
    //            minDif = dif;
    //            angle = AdjustAngles[i];
    //        }
    //    }

    //    //�p�x��ǂ߂�l�ɒ���
    //    if (angle > 180)
    //    {
    //        angle -= 360;
    //    }
    //    angle *= -1;
    //    angle += 90;
    //    rad = angle * Mathf.Deg2Rad;

    //    //�p�x����x�N�g���ɂ���
    //    Vector3 vec = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
    //    vec = vec.normalized;

    //    //��������
    //    if (SplitStick)
    //    {
    //        if (stickCanShotRange)
    //        {
    //            adjustLeftStick = (Vector2)vec;
    //        }
    //        else
    //        {
    //            adjustLeftStick = Vector2.zero;
    //        }
    //    }
    //}

    /// <summary>
    /// �e�����Ă��ԂȂ̂����`�F�b�N����
    /// </summary>
    private void CheckCanShot()
    {

        //�ŏI�I�ɑłĂ邩�̌���
        if (canShotState && stickCanShotRange && BulletScript.CanShotFlag)
        {
            canShot = true;
        }
        else
        {
            canShot = false;
        }
    } 

    public void ClearModeTransitionFlag()
    {
        forciblyRleaseFlag = false;
        forciblyFollowFlag = false;
        forciblySwingFlag = false;
        forciblyReleaseSaveVelocity = false;
        forciblyFollowVelToward = false;
        forciblySwingNextFollow = false;
        forciblySwingSaveVelocity = false;
    }

    /// <summary>
    /// �e�������߂�����
    /// </summary>
    /// <param name="saveVelocity">true:�����߂����ɂ��Ƃ̃x���V�e�B��ێ�
    ///�@false:�����߂����ɂ��Ƃ̃x���V�e�B���E��
    /// </param>
    public void ForciblyReleaseMode(bool saveVelocity)
    {
        ClearModeTransitionFlag();
        if (refState == EnumPlayerState.SHOT)
        {
            forciblyRleaseFlag = true;
            forciblyReleaseSaveVelocity = saveVelocity;

        }
        else if(refState == EnumPlayerState.SWING)
        {
            endSwing = true;
            forciblySwingSaveVelocity = saveVelocity;
        }
    }

    public void ForciblyFollowMode(bool velTowardBullet)
    {
        ClearModeTransitionFlag();
        if (refState == EnumPlayerState.SHOT)
        {
            forciblyFollowFlag = true;
            forciblyFollowVelToward = velTowardBullet;
        }
    }

    public void ForciblySwingMode(bool nextFollow)
    {
        ClearModeTransitionFlag();
        if (refState == EnumPlayerState.SHOT)
        {
            forciblySwingFlag = true;
            forciblySwingNextFollow = nextFollow;
        }
    }

    public void RecoverBullet()
    {
        recoverBullet = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Aspect asp = DetectAspect.DetectionAspect(collision.contacts[0].normal);


        //�󒆂ŕǂɂԂ������Ƃ����x���Ȃ���
        if (refState == EnumPlayerState.MIDAIR)
        {
                switch (asp)
                {
                    case Aspect.LEFT:
                    case Aspect.RIGHT:
                        vel.x *= 0.0f;
                        if (vel.y > 1.0f)
                        {
                            vel.y *= 0.0f;
                        }

                        break;

                    case Aspect.DOWN:
                        vel.x *= 1.0f;
                        vel.y *= 0.0f;

                        
                        break;
                }
        }


        //�V���b�g���ɕǂɂ��������Ƃ��̏���
        if (refState == EnumPlayerState.SHOT)
        {
            if (collision.gameObject.CompareTag("Platform") || 
                collision.gameObject.CompareTag("Conveyor_Tate") || collision.gameObject.CompareTag("Conveyor_Yoko"))
            {
                switch (shotState)
                {
                    case ShotState.STRAINED: //�R����l��
                        //RAY�Ɉڍs
                        //if (isOnGround == false)
                        //{
                        //    ForciblyReturnBullet(true);
                        //}

                        break;

                    case ShotState.FOLLOW: //�R�Ɉ��������
                        if (asp == Aspect.DOWN)
                        {
                            ForciblyReleaseMode(false);
                        }
                        break;

                    case ShotState.GO:
                        //�����E��
                        switch (asp)
                        {
                            case Aspect.LEFT:
                            case Aspect.RIGHT:
                                vel.x *= 0.0f;
                                if (vel.y > 1.0f)
                                {
                                    vel.y *= 0.1f;
                                }
                                break;

                            case Aspect.DOWN:
                                vel.x *= 1.0f;
                                vel.y *= 0.0f;
                                break;
                        }
                        break;
                }
            }
        }


        //swing���ɕǂɂԂ������Ƃ��̏���(���]�A�����I��)
        if (refState == EnumPlayerState.SWING)
        {
            if (swingState == SwingState.TOUCHED)
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
                {
                    if (dir == PlayerMoveDir.RIGHT && asp == Aspect.LEFT)
                    {
                        conuterSwing = true;
                    }
                    else if (dir == PlayerMoveDir.LEFT && asp == Aspect.RIGHT)
                    {
                        conuterSwing = true;
                    }
                    else if (dir == PlayerMoveDir.RIGHT && asp == Aspect.RIGHT)
                    {
                        Debug.Log("collision Platform : Wall Jump");
                    }
                    else if (dir == PlayerMoveDir.LEFT && asp == Aspect.LEFT)
                    {
                        Debug.Log("collision Platform : Wall Jump");
                    }
                    else�@if (asp == Aspect.DOWN)
                    {
                        Debug.Log("collision Platform down: swing end");
                        conuterSwing = true;
                    }

                }
            }
        }

    }



    private void OnCollisionStay(Collision collision)
    {
        Aspect asp = DetectAspect.DetectionAspect(collision.contacts[0].normal);
        Collider col = GetComponent<Collider>();

        //footHit�i�[�p
        Ray footray = new Ray(rb.position, Vector3.down);
        Physics.SphereCast(footray, colliderRadius, out footHit, coliderDistance, LayerMask.GetMask("Platform"));


        //���n����
        if (isOnGround == false)
        {
            //if (vel.y < 0)
            //{
                Ray ray = new Ray(rb.position, Vector3.down);
                if (Physics.SphereCast(ray, colliderRadius, coliderDistance, LayerMask.GetMask("Platform")))
                {
                    isOnGround = true;
                    animator.SetBool(animHash.onGround, true);

                    EffectManager.instance.landEffect(collision.contacts[0].point);
                 }
            //}
        }

        //swing���ɕǂɂԂ������Ƃ��̏���(�X���C�h)
        if (refState == EnumPlayerState.SWING)
        {
            if (swingState == SwingState.TOUCHED)
            {
                if (collision.gameObject.CompareTag("Platform"))
                {
                   if (asp == Aspect.UP)
                    {
                        Vector3 vecToPlayerR = rb.position - BulletScript.rb.position;
                        vecToPlayerR = vecToPlayerR.normalized;
                        Ray footRay = new Ray(rb.position, vecToPlayerR);

                        if (Physics.SphereCast(footRay, SwingcolliderRadius, SwingcoliderDistance, LayerMask.GetMask("Platform")))
                        {
                            Debug.Log("collision Platform : slide continue");
                            SlideSwing = true;
                        }
                        else
                        {
                            Debug.Log("collision Platform : swing end");
                            endSwing = true;
                        }
                    }
                }
            }
        }


        //FOLLOW���ɕǂɓ�����Ə�ɕ␳
        if (refState == EnumPlayerState.SHOT)
        {
            if (shotState == ShotState.FOLLOW)
            {
                if (asp == Aspect.LEFT || asp == Aspect.RIGHT)
                {
                    Vector3 tempPos = transform.position;
                    tempPos.y += 0.7f;
                    transform.position = tempPos;
                }
            }
        }

        
    }

    /// <summary>
    /// �g���K�[�ނ͂����ɑJ�ڂ��Ȃ��ꍇ���Z�b�g
    /// </summary>
    private void OnAnimatorMove()
    {
        AnimTriggerReset();
    }


    //�ڒn������v�Z
    private void CheckMidAir()
    {
        Ray ray = new Ray(rb.position, Vector3.down);
        if (isOnGround)
        {
            if (Physics.SphereCast(ray, colliderRadius, coliderDistance, LayerMask.GetMask("Platform")) == false)
            {
                isOnGround = false;
                animator.SetBool(animHash.onGround, false);
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    //�ڒnray
    //    Ray footRay = new Ray(rb.position, Vector3.down);

    //        if (isOnGround)
    //        {
    //            Gizmos.color = Color.magenta;
    //        }
    //        else
    //        {
    //            Gizmos.color = Color.cyan;
    //        }
    //        Gizmos.DrawWireSphere(footRay.origin + (Vector3.down * (coliderDistance)), colliderRadius);


    //        //��
    //        //if (refState == EnumPlayerState.SHOT)
    //        //{
    //        //    if (shotState == ShotState.STRAINED)
    //        //    {
    //        Vector3 vecToPlayer = BulletScript.rb.position - rb.position;
    //        vecToPlayer = vecToPlayer.normalized;

    //        Ray headRay = new Ray(rb.position, vecToPlayer);
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireSphere(headRay.origin + (vecToPlayer * (HcoliderDistance)), HcolliderRadius);
    //        //    }
    //        //}

    //        //�X�C���O�X���C�h����
    //        //if(refState == EnumPlayerState.SWING)
    //        //{
    //        //    if(swingState == SwingState.TOUCHED) 
    //        //    {
    //        Vector3 vecToPlayerR = rb.position - BulletScript.rb.position;
    //        vecToPlayerR = vecToPlayerR.normalized;

    //        Ray Ray = new Ray(rb.position, vecToPlayerR);
    //        Gizmos.color = Color.black;
    //        Gizmos.DrawWireSphere(Ray.origin + (vecToPlayerR * SwingcoliderDistance), SwingcolliderRadius);
    //        //    }
    //        //}   
    //    
    //}

}
