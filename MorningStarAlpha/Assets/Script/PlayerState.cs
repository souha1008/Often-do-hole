using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// �v���C���[�̏�ԊǗ�������C���^�[�t�F�[�X
/// MonoBehaviour�͌p�����Ȃ�
/// </summary>
/// 

//���񂿁I
//���񂿂�I
//������[�[�[�[�[�[�I�I
//���������Ă���


public class PlayerState
{
    virtual public void UpdateState() { }      //�p����ŃR���g���[���[�̓���
    virtual public void Move() { }             //�p����ŕ��������irigidbody���g�p�������́joverride����
    virtual public void StateTransition() { }  //�p����ŃV�[���̈ړ������߂�
    virtual public void DebugMessage() { }     //�f�o�b�O�p�̃��b�Z�[�W�\��

    static public GameObject Player;
    static public PlayerMain PlayerScript;
}


/// <summary>
/// �v���C���[�������~�܂��Ă�����
/// �X�e�B�b�N���͂ł̈ړ��A�e�̔��˂��ł���
/// </summary>
public class PlayerStateStand : PlayerState
{  
    private bool shotButton;
    public PlayerStateStand()//�R���X�g���N�^
    {
        shotButton = false;
    }

    public override void UpdateState() 
    {
        if (Input.GetButtonDown("Button_R"))
        {
            if (PlayerScript.canShot)
            {
                shotButton = true;
            }
        }

        if (PlayerScript.leftStick.x > 0.2f)
        {
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (PlayerScript.leftStick.x < -0.2f)
        {
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public override void Move()
    {
        //����
        if (PlayerScript.vel.x > PlayerScript.MIN_RUN_SPEED)
        {
            PlayerScript.vel *= PlayerScript.RUN_FRICTION;
            if(PlayerScript.vel.x < PlayerScript.MIN_RUN_SPEED)
            {
                PlayerScript.vel.x = 0;
            }
        }
        if (PlayerScript.vel.x < PlayerScript.MIN_RUN_SPEED * -1)
        {
            PlayerScript.vel *= PlayerScript.RUN_FRICTION;
            if (PlayerScript.vel.x > PlayerScript.MIN_RUN_SPEED * -1)
            {
                PlayerScript.vel.x = 0;
            }
        }

        PlayerScript.rb.AddForce(Vector3.down * 20.8f, ForceMode.Acceleration);
        PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.rb.velocity.y, 0);
    }


    public override void StateTransition()
    {
        //�E�ړ��Ɉڍs
        if (PlayerScript.leftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.mode = new PlayerStateMove();
        }

        //���ړ��Ɉڍs
        if (PlayerScript.leftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1)
        {
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.mode = new PlayerStateMove();
        }

        if (shotButton)
        {
            PlayerScript.mode = new PlayerStateShot();
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Stand");
    }
}

/// <summary>
/// �v���C���[���ړ����Ă�����
/// �X�e�B�b�N���͂ł̈ړ��A�e�̔��˂��ł���
/// </summary>
public class PlayerStateMove : PlayerState
{
    bool shotButton;

    public PlayerStateMove()//�R���X�g���N�^
    {
        shotButton = false;

        //�v���C���[�̉�]
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public override void UpdateState()
    {
        if (Input.GetButtonDown("Button_R"))
        {
            if (PlayerScript.canShot)
            {
                shotButton = true;
            }
        }
    }

    public override void Move()
    {
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED;
            PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);           
        }
        if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * -1;
            PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
        }

        PlayerScript.rb.AddForce(Vector3.down * 20.8f, ForceMode.Acceleration);
        PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.rb.velocity.y, 0);
    }

    public override void StateTransition()
    {
        //�X�e�B�b�N���������l�ȉ��Ȃ�stand��ԂɈڍs
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            if (PlayerScript.leftStick.x < (PlayerScript.LATERAL_MOVE_THRESHORD) - 0.1)
            {
                PlayerScript.mode = new PlayerStateStand();
            }
        }
        if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            if (PlayerScript.leftStick.x > (PlayerScript.LATERAL_MOVE_THRESHORD * -1) + 0.1)
            {
                PlayerScript.mode = new PlayerStateStand();
            }
        }

        if (shotButton)
        {
            PlayerScript.mode = new PlayerStateShot();
        }
    }


    public override void DebugMessage()
    {
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            Debug.Log("PlayerState:RunRight");
        }
        if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            Debug.Log("PlayerState:RunLeft");
        }
    }
}


/// <summary>
/// �e�����������
/// �e�̓I�u�W�F�N�g�ɂ͐ڐG���Ă��Ȃ�
/// �X�e�B�b�N�ł̈ړ��s�A�e�������߂����Ƃ̂݉\
/// </summary>
public class PlayerStateShot : PlayerState
{
    private enum SHOT_STATE{
        GO,         //��������ꂸ�ɔ��ł���
        STRAINED,�@ //�v���C���[����������Ȃ�����ł���
        RETURN,     //�v���C���[�ɖ߂��Ă���
    }

    SHOT_STATE shotState;               //�R������l�߂Ă��邩�i����l�߂Ă�����e�ɂ��Ă����j
    float countTime;               //���˂���̎���
    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //���˂���̒e��vector��ۑ�����
    bool finishFlag;
    BulletMain BulletScript;
    public PlayerStateShot()//�R���X�g���N�^
    {    
        //�e�̐����Ɣ���
        //���ˎ��ɂԂ���Ȃ��悤�ɔ��ˈʒu��������ɂ��炷
        Vector3 vec = PlayerScript.leftStick.normalized;
        Vector3 popPos = PlayerScript.transform.position + (vec * 5);
        PlayerScript.Bullet = Object.Instantiate(PlayerScript.BulletPrefab, popPos, Quaternion.identity);
        BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //�o���b�g���̃X�i�b�v
    }

    public override void UpdateState()
    {
        countTime += Time.deltaTime;

        if (countTime > 0.3)
        {
            if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
            {
                shotState = SHOT_STATE.RETURN;                                          
            }
        }
    }

    public override void Move()
    {
      
        float interval;

        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
        switch (shotState) {

            case SHOT_STATE.GO:
               
                bulletVecs.Enqueue(BulletScript.rb.velocity);

                //�R�̒����𒴂�������������Ă����Ԃɂ���
                if (interval > BulletScript.BULLET_ROPE_LENGTH)
                {
                    shotState = SHOT_STATE.STRAINED;
                }

                PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.vel.y, 0);
                break;

            case SHOT_STATE.STRAINED:
                //�e�̐i�s�����̋t�����Ɉʒu�␳
                bulletVecs.Enqueue(BulletScript.rb.velocity);
                PlayerScript.vel = bulletVecs.Dequeue();
                PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.vel.y, 0);

                //����
                //if (interval > BulletScript.BULLET_ROPE_LENGTH)
                //{
                //    Vector3 ballVel = BulletScript.rb.velocity;
                //    Vector3 reverceVel = (ballVel * -1).normalized;

                //    PlayerScript.transform.position = BulletScript.transform.position + reverceVel * BulletScript.BULLET_ROPE_LENGTH;
                //}
                break;

            case SHOT_STATE.RETURN:
                //�����֒e�������߂�
                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
                vec = vec.normalized;

                BulletScript.rb.velocity = vec * 30;

                //���������ȉ��ɂȂ�����e���A�N�e�B�u

                if (interval < 4.0f)
                {
                    finishFlag = true;
                    Object.Destroy(BulletScript.gameObject);
                }

                PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.vel.y, 0);
                break;
        }

        
    }

    public override void StateTransition()
    {
        if (finishFlag)
        {
            //���n�����痧���Ă����ԂɈڍs
            Ray downRay = new Ray(PlayerScript.rb.position, Vector3.down);
            if (Physics.Raycast(downRay, 2.0f))
            {
                PlayerScript.mode = new PlayerStateStand();
            }
            else //�����łȂ��Ȃ��
            {
                PlayerScript.mode = new PlayerStateMidair();
            }
        } 
        
        //BulletMain BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>();

        //�{�[�����G�ꂽ��X�C���O���
        if (BulletScript.isTouched)
        {
            PlayerScript.mode = new PlayerStateSwing();
        }
    }
    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Shot");
    }
}



/// <summary>
/// �v���C���[���󒆂ɂ�����
/// �N�[���^�C�����o�āA�e�̔��˂��ł���
/// 
/// </summary>
public class PlayerStateMidair : PlayerState
{
    private bool shotButton;
    private float countTime;


    public PlayerStateMidair()//�R���X�g���N�^
    {
        shotButton = false;
        countTime = 0.0f;
    }

    public override void UpdateState()
    {
        countTime += Time.deltaTime;

        if (PlayerScript.leftStick.x > 0.2f)
        {
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (PlayerScript.leftStick.x < -0.2f)
        {
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (countTime > PlayerScript.BULLET_RECAST_TIME)
        {
            if (Input.GetButtonDown("Button_R"))
            {
                shotButton = true;
            }
        }
    }

    public override void Move()
    {
        //����
        PlayerScript.vel.x *= PlayerScript.MIDAIR_FRICTION;
        if (PlayerScript.leftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED;
        }
        else if (PlayerScript.leftStick.x < -PlayerScript.LATERAL_MOVE_THRESHORD)
        {
            PlayerScript.vel.x += PlayerScript.ADD_MIDAIR_SPEED * -1;
        }


        PlayerScript.rb.AddForce(Vector3.down * 30.8f, ForceMode.Acceleration);
        PlayerScript.rb.velocity = new Vector3(PlayerScript.vel.x, PlayerScript.rb.velocity.y, 0);
    }


    public override void StateTransition()
    { 
        if (shotButton)
        {
            PlayerScript.mode = new PlayerStateShot();
        }

        //���n�����痧���Ă����ԂɈڍs
        Ray downRay = new Ray(PlayerScript.rb.position, Vector3.down);
        if (Physics.Raycast(downRay, 2.0f))
        {
            PlayerScript.mode = new PlayerStateStand();
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:MidAir");
    }
}

/// <summary>
/// �e���V��ŃX�C���O���Ă�����
/// </summary>
public class PlayerStateSwing : PlayerState
{

    enum SwingMode { 
        Touced,   //�߂܂��Ă�����
        Rereased, //�؂藣�������
    }

    SwingMode swingMode;
    BulletMain BulletScript;
    private bool finishFlag;
    private bool shotButton;
    private Vector3 ballPosition;
    private const float SWING_ANGLER_VELOCITY = 2.7f; //�U��q�p���x ��const�l�ł͂Ȃ��O��state��velocity�ŉς��ǂ�����
    private const float SWING_REREASE_ANGLE = 140.0f; //�U��q���̂Ȃ��p�����ȏ�ɂȂ����狭������

    public PlayerStateSwing()//�R���X�g���N�^
    {
        finishFlag = false;
        swingMode = SwingMode.Touced;
        BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>();�@
        shotButton = false;
        BulletScript.rb.isKinematic = true;
        ballPosition = BulletScript.gameObject.transform.position;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel = Vector3.zero;
    }

    public override void UpdateState()
    {
        //if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
        //{
        //    BulletScript.rb.isKinematic = false;
        //    shotButton = true;
        //}

        //�{�[���v���C���[�Ԃ̊p�x�����߂�
        Vector3 dt = Player.transform.position - ballPosition;
        float rad = Mathf.Atan2(dt.x, dt.y);
        float degree = rad * Mathf.Rad2Deg;
        if (degree < 0)//���������Ɏ��v����0~360�̒l�ɕ␳
        {
            degree += 360;
        }

        //���p�x�ȏ�Ő؂藣��
        if (swingMode == SwingMode.Touced)
        {
            if (PlayerScript.dir == PlayerMoveDir.RIGHT)
            {
                if (degree < SWING_REREASE_ANGLE)
                {
                    BulletScript.gameObject.GetComponent<Collider>().isTrigger = true;
                    BulletScript.rb.isKinematic = false;
                    swingMode = SwingMode.Rereased;

                    //�����ǉ�
                    //�e�ƃv���C���[�Ԃ̃x�N�g���ɒ��s����x�N�g��
                    Vector3 addVec = ballPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, -110) * addVec;
                    PlayerScript.rb.AddForce(addVec * 35.0f, ForceMode.VelocityChange);
                    
                }
            }
            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
            {
                if (degree > 360 - SWING_REREASE_ANGLE)
                {
                    BulletScript.gameObject.GetComponent<Collider>().isTrigger = true;
                    BulletScript.rb.isKinematic = false;
                    swingMode = SwingMode.Rereased;

                    //�����ǉ�
                    //�e�ƃv���C���[�Ԃ̃x�N�g���ɒ��s����x�N�g��
                    Vector3 addVec = ballPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, 110) * addVec;
                    PlayerScript.rb.AddForce(addVec * 35.0f, ForceMode.VelocityChange);
                    
                }
            }
        }
    }

    public override void Move()
    {

        switch (swingMode) 
        {
            case SwingMode.Touced:
                //�����ɂ���ĉ�]�������Ⴄ
                Quaternion angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY, Vector3.forward);
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY, Vector3.forward);
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY * -1, Vector3.forward);
                }

                Vector3 pos = Player.transform.position;

                pos -= ballPosition;
                pos = angleAxis * pos;
                pos += ballPosition;
                PlayerScript.transform.position = pos;
                break;

            case SwingMode.Rereased:
                //�����֒e�������߂�
                float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
                vec = vec.normalized;

                BulletScript.rb.velocity = vec * 100;

                //���������ȉ��ɂȂ�����e���A�N�e�B�u

                if (interval < 4.0f)
                {
                    PlayerScript.vel = PlayerScript.rb.velocity; //���x�󂯓n��
                    finishFlag = true;
                    Object.Destroy(BulletScript.gameObject);
                }

                break;

            default:
                break;
        }

        

    }


    public override void StateTransition()
    {
        if (finishFlag)
        {
            PlayerScript.mode = new PlayerStateMidair();
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Swing");
    }
}