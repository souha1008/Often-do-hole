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
    NONE,      //�󒆏�Ԃł͂Ȃ�
    NORMAL,   //�ʏ펞
    FALL, �@  //�}�~��
}


/// <summary>
/// �X�C���O���ׂ̍��ȏ��
/// </summary>
public enum SwingState
{
    NONE,      //�X�C���O��Ԃł͂Ȃ�
    TOUCHED,   //�߂܂��Ă�����
    RELEASED,  //�؂藣�������
    HANGING,   //�Ԃ牺������
}

/// <summary>
/// �e�ˏo���ׂ̍��ȏ��
/// </summary>
public enum ShotState
{
    NONE,       //�e�ˏo����Ă��Ȃ�
    GO,         //��������ꂸ�ɔ��ł���
    STRAINED,  //�v���C���[����������Ȃ�����ł���
    RETURN,     //�e���v���C���[�ɖ߂��ďI��
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
}

public struct ShortenSwing {
    public bool isShort;
    public float length;
}


public class PlayerMain : MonoBehaviour
{
    [System.NonSerialized] public Rigidbody rb;      // [System.NonSerialized] �C���X�y�N�^��ŕ\�����������Ȃ�
    [System.NonSerialized] public static PlayerMain instance;
    [System.NonSerialized] public Animator animator;
    public BulletMain BulletScript;
    public PlayerState mode;                         // �X�e�[�g
    private RaycastHit footHit;                      // Ge
    private float killVeltimer;                      //�ǂԂ���ő��x���E���Ƃ��̃`���^�����O�h�~�p

    [SerializeField, Tooltip("�`�F�b�N�������Ă�������͕���")] private bool SplitStick;        //����Ƀ`�F�b�N�������Ă����番��
    [SerializeField, Tooltip("�X�e�B�b�N������␳����i�v�f���ŕ����j\n�l�͏オ0�Ŏ��v���ɑ����B0~360�͈̔�")] private float[] AdjustAngles;   //�X�e�B�b�N������␳����i�v�f���ŕ����j�l�͏オ0�Ŏ��v���ɑ����B0~360�͈̔�
    [SerializeField, Tooltip("�`�F�b�N�������Ă�����{�^�������Ŕ���")] public bool ReleaseMode;
    [SerializeField, Tooltip("�`�F�b�N�������Ă�����U��q�����Ő؂藣��")] public bool AutoRelease;
    [SerializeField, Tooltip("�`�F�b�N�������Ă�����U��q���R�������Ȃ�")] public bool LongRope;

    [System.NonSerialized] public float colliderRadius = 1.42f;   //�ڒn����pray���a
    [System.NonSerialized] public float coliderDistance = 1.8f; //
                                                                 //
    [System.NonSerialized] public float HcolliderRadius = 2.0f;   //������pray���a
    [System.NonSerialized] public float HcoliderDistance = 0.6f; //������pray���S�_���瓪�܂ł̃I�t�Z�b�g

    [SerializeField] public  float SwingcolliderRadius = 1.5f;   //�X�C���O�X���C�h����pray���a
    [SerializeField] public  float SwingcoliderDistance = 1.75f; //�X�C���O�X���C�hray���S�_���瓪�܂ł̃I�t�Z�b�g

    //----------���v���C���[���������֘A�̒萔��----------------------
    [Range(0.1f, 1.0f), Tooltip("���E�ړ��J�n�̃X�e�B�b�N�������l")] public float  LATERAL_MOVE_THRESHORD;   // ���荶�E�ړ����̍��X�e�B�b�N�������l
    [Tooltip("����ō����x")] public float                      MAX_RUN_SPEED;           // ����ō����x
    [Tooltip("����Œᑬ�x�i��������瑬�x0�j")] public float   MIN_RUN_SPEED;�@�@�@�@�@ // ����Œᑬ�x�i��������瑬�x0�j
    [Tooltip("�����t���[���ŏオ��X�s�[�h")] public float    ADD_RUN_SPEED;           // �����t���[���ŏオ��X�s�[�h
    [Tooltip("�U��q�؂藣�������Z")] public float �@�@�@�@�@�@ RELEASE_FORCE;
    [Tooltip("�������x����")] public float                      MAX_FALL_SPEED;          // �d�͂ɂ��Œᑬ�x
    [Tooltip("�󒆂ɂ���Ƃ��̏d�͉����x")] public float        FALL_GRAVITY;            // �v���C���[���󒆂ɂ���Ƃ��̏d�͉����x
    [Tooltip("���������Ă���Ƃ��̏d�͉����x")] public float  STRAINED_GRAVITY;�@�@�@�@// �v���C���[�����������Ă���Ƃ��̏d�͉����x
    [Range(0.1f, 1.0f), Tooltip("�n�㑬�x������")] public float�@RUN_FRICTION;            // ����̌�����


    [Tooltip("�󒆈�t���[���ŏオ��X�s�[�h")] public float                      ADD_MIDAIR_SPEED;        // �󒆈�b�Ԃŏオ��X�s�[�h
    [Range(0.1f, 1.0f), Tooltip("�󒆑��x������")] public float                   MIDAIR_FRICTION;         // �󒆂̑��x������
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
    [ReadOnly, Tooltip("�n�ʂƐڐG���Ă��邩")] public bool isOnGround;                          // �n�ʂɐG��Ă��邩�ionCollision�ŕύX�j
    [ReadOnly, Tooltip("�łĂ�\�������邩")] public bool canShotState;                             // �łĂ��Ԃ�
    [ReadOnly, Tooltip("�X�e�B�b�N�̓��͂����ȏ゠�邩�F����ꍇ�͑łĂ�")] public bool stickCanShotRange;
    [ReadOnly, Tooltip("�ǂ̋߂��ɂ���ꍇ�͌��ĂȂ�")] public bool CanShotColBlock;                           // �X�e�B�b�N���͂̐�ɕǂ�
    [ReadOnly, Tooltip("�ŏI�I�ɑłĂ邩�ǂ���")] public bool canShot;                             // �łĂ��Ԃ�
    [ReadOnly, Tooltip("velocity�ł̈ړ���position���ڕύX�ɂ��ړ���")] public bool useVelocity;                         // �ړ���velocity������position�ύX���X�e�[�g�ɂ���Ă͒��ڈʒu��ύX���鎞�����邽��
    [ReadOnly, Tooltip("�����I�ɒe��߂�����t���O")] public bool forciblyReturnBulletFlag;            // �����I�ɒe��߂�����t���O
    [ReadOnly, Tooltip("�����I�ɒe��߂�����Ƃ��Ɍ��݂̑��x��ۑ����邩")] public bool forciblyReturnSaveVelocity;
    [ReadOnly, Tooltip("�X�C���O�����I���p")] public bool endSwing;
    [ReadOnly, Tooltip("�X�C���O�Z������p")] public bool SlideSwing;
    [ReadOnly, Tooltip("�X�C���O�Ԃ牺����p")] public bool hangingSwing;


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
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
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
        canShotState = true;
        stickCanShotRange = false;
        CanShotColBlock = false;
        canShot = false;
        isOnGround = true;
        useVelocity = true;

        forciblyReturnBulletFlag = false;
        forciblyReturnSaveVelocity = false;

        endSwing = false;
        SlideSwing = false;
        
        hangingSwing = false;
        killVeltimer = 0.0f;

        Ray footray = new Ray(rb.position, Vector3.down);
        Physics.SphereCast(footray, colliderRadius, out footHit, coliderDistance, LayerMask.GetMask("Platform"));



        rb.sleepThreshold = -1; //���W�b�h�{�f�B���Î~���Ă��Ă�onCollision�n���Ă΂�����

        mode = new PlayerStateOnGround(); //�����X�e�[�g

        if (mode != null)
        {
            mode.UpdateState();
            //mode.Animation();
            mode.StateTransition();
            mode.Move();
        }
    }

    private void Update()
    {
        if (GameStateManager.GetGameState() == GAME_STATE.PLAY && FadeManager.GetNowState() == FADE_STATE.FADE_NONE)
        {
            InputStick();
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
                addVel *= 0.96f;
            }
            else
            {
                addVel = Vector3.zero;
            }

            killVeltimer = Mathf.Clamp(killVeltimer += Time.fixedDeltaTime, 0.0f, 2.0f);

#if UNITY_EDITOR //unity�G�f�B�^�[��ł̓f�o�b�O���s���i�r���h���ɂ͖��������j
                //mode.DebugMessage();
#endif
            
        }
    }

    public RaycastHit getFootHit()
    {
        return footHit;
    }

    private void InputStick()
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
        }

        //�X�e�B�b�N�̊p�x�����߂�
        float rad = Mathf.Atan2(adjustLeftStick.x, adjustLeftStick.y);
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
            if (stickCanShotRange)
            {
                adjustLeftStick = vec;
            }
        }
        
    }

    /// <summary>
    /// �e�����Ă��ԂȂ̂����`�F�b�N����
    /// </summary>
    private void CheckCanShot()
    {
        //�f�o�b�O���O
        Vector3 StartPos;
        StartPos = rb.position;
        StartPos.y += 1.0f;

        RaycastHit hit;
        if (Physics.Raycast(StartPos, adjustLeftStick, out hit, 3.0f))
        {
            if (hit.collider.CompareTag("Platform"))
            {
                CanShotColBlock = false;
            }
            else
            {
                CanShotColBlock = true;
            }
        }
        else
        {
            CanShotColBlock = true;
        }
        StartPos.z += 2.0f;
        Debug.DrawRay(StartPos, adjustLeftStick * 3.0f, Color.red);


        //�ŏI�I�ɑłĂ邩�̌���
        if(canShotState && stickCanShotRange && CanShotColBlock)
        {
            canShot = true;
        }
        else
        {
            canShot = false;
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
        forciblyReturnBulletFlag = true;
        forciblyReturnSaveVelocity = saveVelocity;
    }


    private void OnCollisionEnter(Collision collision)
    {
        Aspect asp = DetectAspect.DetectionAspect(collision.contacts[0].normal);


        

        //�V���b�g���ɕǂɂ��������Ƃ��̏���
        if(refState == EnumPlayerState.SHOT)
        {
            if (collision.gameObject.CompareTag("Platform"))
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
                            ForciblyReturnBullet(false);
                        }
                        break;

                    case ShotState.GO:
                    case ShotState.RETURN:
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
                                vel.x *= 0.2f;
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
                if (collision.gameObject.CompareTag("Platform"))
                {
                    if (dir == PlayerMoveDir.RIGHT && asp == Aspect.LEFT)
                    {
                        hangingSwing = true;
                    }
                    else if (dir == PlayerMoveDir.LEFT && asp == Aspect.RIGHT)
                    {
                        hangingSwing = true;
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
                        hangingSwing = true;
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
                }
            //}
        }

        
        //�󒆂ŕǂɂԂ������Ƃ����x���Ȃ���
        if (refState == EnumPlayerState.MIDAIR)
        {
            if (killVeltimer > 0.1f)
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
                        vel.x *= 0.2f;
                        vel.y *= 0.0f;
                        break;
                }
                killVeltimer = 0.0f;
            }
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


    //�ڒn������v�Z
    private void CheckMidAir()
    {
        Ray ray = new Ray(rb.position, Vector3.down);
        if (isOnGround)
        {
            if (Physics.SphereCast(ray, colliderRadius, coliderDistance, LayerMask.GetMask("Platform")) == false)
            {
                isOnGround = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        //�ڒnray
        Ray footRay = new Ray(rb.position, Vector3.down);
        if (isOnGround)
        {
            Gizmos.color = Color.magenta;
        }
        else
        {
            Gizmos.color = Color.cyan;
        }
        Gizmos.DrawWireSphere(footRay.origin + (Vector3.down * (coliderDistance)), colliderRadius);


        //��
        //if (refState == EnumPlayerState.SHOT)
        //{
        //    if (shotState == ShotState.STRAINED)
        //    {
                Vector3 vecToPlayer = BulletScript.rb.position - rb.position;
                vecToPlayer = vecToPlayer.normalized;

                Ray headRay = new Ray(rb.position, vecToPlayer);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(headRay.origin + (vecToPlayer * (HcoliderDistance)), HcolliderRadius);
        //    }
        //}

        //�X�C���O�X���C�h����
        //if(refState == EnumPlayerState.SWING)
        //{
        //    if(swingState == SwingState.TOUCHED) 
        //    {
                Vector3 vecToPlayerR = rb.position - BulletScript.rb.position;
                vecToPlayerR = vecToPlayerR.normalized;

                Ray Ray = new Ray(rb.position, vecToPlayerR);
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(Ray.origin + (vecToPlayerR * SwingcoliderDistance), SwingcolliderRadius);
        //    }
        //}   
    }

}
