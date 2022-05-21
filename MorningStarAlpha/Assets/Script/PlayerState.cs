using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// �v���C���[�̏�ԊǗ�������C���^�[�t�F�[�X
/// MonoBehaviour�͌p�����Ȃ�
/// </summary>
public class PlayerState
{
    protected float fixedAdjust = Time.fixedDeltaTime * 50; 

    virtual public void UpdateState() { }      //�p����ŃR���g���[���[�̓���
    virtual public void Move() { }             //�p����ŕ��������irigidbody���g�p�������́joverride����
    virtual public void StateTransition() { }  //�p����ŃV�[���̈ړ������߂�
    virtual public void DebugMessage() { }     //�f�o�b�O�p�̃��b�Z�[�W�\��
    virtual public void Animation() { }

    static public GameObject Player;
    static public PlayerMain PlayerScript;
    static public BulletMain BulletScript;

    ///// <summary>
    ///// �o���b�g�̈ʒu����ɃX�e�B�b�N�����ɒ���
    ///// </summary>
    //protected void BulletAdjust()
    //{
    //    Vector3 vec = PlayerScript.adjustLeftStick.normalized;
    //    vec = vec * 2;
    //    Vector3 adjustPos = PlayerScript.transform.position + vec;

    //    BulletScript.transform.position = adjustPos;
    //}

    protected void RotationStand()
    {
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 90, 0));
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            PlayerScript.rb.MoveRotation(Quaternion.Euler(0, -90, 0));
        }
    }

    protected void NoFloorVel()
    {
        PlayerScript.floorVel = Vector3.zero;

    }
}



///�ߋ��̃X�N���v�g
///
/// 
///// <summary>
///// �e���V��ŃX�C���O���Ă�����
///// �ړ����x����ۑ�
///// </summary>
//public class PlayerStateSwing_leftstick : PlayerState
//{
//    private bool finishFlag;
//    private bool releaseButton;
//    private Vector3 BulletPosition; //�{�[���̈ʒu


//    private float betweenLength; //�J�n����_�Ԃ̋���(�����̓X�C���Ostate�ʂ��ČŒ�)
//    private Vector3 startPlayerVel;�@�@�@�@�@�@ //�˓���velocity
//    private float startAngle;    //�J�n���̓�_�ԃA���O��
//    private float endAngle;      //�����؂藣�������p�x(start�p�x�ˑ�)
//    private float minAnglerVel;  //�Œ�p���x�i�����؂藣���n�_�ɂ��鎞�j
//    private float maxAnglerVel;�@//�ō��p���x (�^���Ƀv���C���[�����鎞�j
//    private float nowAnglerVel;  //���݊p���x

//    private List<Vector2> leftSticks = new List<Vector2>(); //swing�J�n�����leftStick��ێ�

//    public PlayerStateSwing_leftstick()  //�R���X�g���N�^
//    {
//        BulletPosition = BulletScript.gameObject.transform.position;

//        //�v�Z�p���i�[
//        startPlayerVel = BulletScript.vel;
//        betweenLength = Vector3.Distance(Player.transform.position, BulletPosition);
//        betweenLength = Vector3.Distance(Player.transform.position, BulletPosition);
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);
//        startAngle = endAngle = degree;

//        PlayerScript.refState = EnumPlayerState.SWING;
//        PlayerScript.swingState = SwingState.TOUCHED;
//        PlayerScript.canShotState = false;
//        PlayerScript.endSwing = false;
//        PlayerScript.counterSwing = false;
//        finishFlag = false;
//        releaseButton = false;
//        BulletScript.rb.isKinematic = true;
//        PlayerScript.rb.velocity = Vector3.zero;
//        PlayerScript.vel = Vector3.zero;

//        CalculateStartVariable();
//    }

//    ~PlayerStateSwing_leftstick()
//    {

//    }

//    /// <summary>
//    /// �U��q����p�̊e��ϐ����v�Z
//    /// </summary>
//    public void CalculateStartVariable()
//    {

//        //�R�̒����ƃX�s�[�h����p���x���v�Z
//        float angler_velocity;
//        angler_velocity = (Mathf.Abs(startPlayerVel.x) * 6.0f);
//        angler_velocity /=  (betweenLength * 2.0f * Mathf.PI);

//        //�͈͓��ɕ␳
//        angler_velocity = Mathf.Clamp(angler_velocity, 1.0f, 15.0f);

//        nowAnglerVel = maxAnglerVel = minAnglerVel = angler_velocity;

//        Debug.Log("AnglerVelocity: " + angler_velocity);

//        //�؂藣���A���O���̌v�Z
//        float diff_down = Mathf.Abs(startAngle - 180.0f); //�^���Ɠ˓��p�̍�
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
//            endAngle -= (diff_down + diff_down);
//            //�J�n�_���͍����ʒu�ɂ���
//            endAngle -= 10;

//            //�͈͓��ɕ␳
//            endAngle = Mathf.Clamp(endAngle, 90, 140);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
//            endAngle += (diff_down + diff_down);
//            //�J�n�_���͍����ʒu�ɂ���
//            endAngle += 10;

//            //�͈͓��ɕ␳
//            endAngle = Mathf.Clamp(endAngle, 220, 270);
//        }




//        //�Œᑬ�͓˓����v���C���[velocity
//        maxAnglerVel = minAnglerVel = angler_velocity;
//        //�ō����x�͓˓��p���傫���قǑ���
//        maxAnglerVel += (diff_down / 90) * 2.0f; 
//    }

//    /// <summary>
//    /// �ǒ��˕Ԃ莞�̊e��v�Z
//    /// </summary>
//    public void CalculateCounterVariable()
//    {
//        Debug.Log("counter:");

//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //�v���C���[��]����
//            PlayerScript.dir = PlayerMoveDir.LEFT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //�v���C���[��]����
//            PlayerScript.dir = PlayerMoveDir.RIGHT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
//        }

//        //�؂藣���A���O���̌v�Z
//        float diff_down = Mathf.Abs(endAngle - 180.0f); //�^���ƏI���p�̍�
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
//            endAngle -= (diff_down + diff_down);
//            //�J�n�_���͍����ʒu�ɂ���
//            endAngle -= 20;

//            //�͈͓��ɕ␳
//            endAngle = Mathf.Clamp(endAngle, 90, 140);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
//            endAngle += (diff_down + diff_down);
//            //�J�n�_��荂���ʒu�ɂ���
//            endAngle += 20;

//            //�͈͓��ɕ␳
//            endAngle = Mathf.Clamp(endAngle, 220, 270);
//        }

//    }






//    /// <summary>
//    /// swing���̍��X�e�B�b�N�ɂ���Đ؂藣���_�𒲐�
//    /// </summary>
//    public void ReleasePointAlternate()
//    {
//        leftSticks.Add(PlayerScript.sourceLeftStick);


//    }

//    public override void UpdateState()
//    {
//        ReleasePointAlternate();

//        //if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
//        //{
//        //    BulletScript.rb.isKinematic = false;
//        //    shotButton = true;
//        //}

//        //�{�[���v���C���[�Ԃ̊p�x�����߂�
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);

//        //�؂藣��
//        if (PlayerScript.swingState == SwingState.TOUCHED)
//        {
//            if (PlayerScript.endSwing)
//            {
//                PlayerScript.endSwing = false;
//                PlayerScript.useVelocity = true;
//                BulletScript.ReturnBullet();
//                PlayerScript.swingState = SwingState.RELEASED;
//            }

//            if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//            {
//                if (degree < endAngle)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    PlayerScript.swingState = SwingState.RELEASED;

//                    //�����ǉ�
//                    //�e�ƃv���C���[�Ԃ̃x�N�g���ɒ��s����x�N�g��
//                    Vector3 addVec = BulletPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, -90) * addVec;
//                    PlayerScript.vel += addVec * 35.0f;
//                }
//            }
//            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//            {
//                if (degree > endAngle)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    PlayerScript.swingState = SwingState.RELEASED;

//                    //�����ǉ�
//                    //�e�ƃv���C���[�Ԃ̃x�N�g���ɒ��s����x�N�g��
//                    Vector3 addVec = BulletPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, 90) * addVec;
//                    PlayerScript.vel += addVec * 35.0f;
//                }
//            }
//        }

//        if (PlayerScript.counterSwing)
//        {
//            PlayerScript.counterSwing = false;
//            CalculateCounterVariable();
//        }
//    }

//    public override void Move()
//    {
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);
//        float deg180dif = Mathf.Abs(degree - 180);
//        switch (PlayerScript.swingState)
//        {
//            case SwingState.TOUCHED:
//                //�p���x�v�Z
//                float deg180Ratio = deg180dif / Mathf.Abs(endAngle - 180); //�^���ƍō����B�_�̔䗦
//                deg180Ratio = Mathf.Clamp01(deg180Ratio); //�ꉞ�͈͓��ɕ␳
//                deg180Ratio = 1 - deg180Ratio; //�^����1,�ō����B�_��0�Ƃ���

//                float easeDeg180Ratio = Easing.Linear(deg180Ratio, 1.0f, 0.0f, 1.0f);

//                nowAnglerVel = ((maxAnglerVel - minAnglerVel) * easeDeg180Ratio) + minAnglerVel;

//                //�����ɂ���ĉ�]�������Ⴄ
//                Quaternion angleAxis = Quaternion.Euler(Vector3.forward);
//                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//                {
//                    angleAxis = Quaternion.AngleAxis(nowAnglerVel, Vector3.forward);
//                }
//                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//                {
//                    angleAxis = Quaternion.AngleAxis(nowAnglerVel * -1, Vector3.forward);
//                }

//                Vector3 pos = Player.transform.position;

//                pos -= BulletPosition;
//                pos = angleAxis * pos;
//                pos += BulletPosition;
//                PlayerScript.transform.position = pos;
//                break;

//            case SwingState.RELEASED:
//                //�����֒e�������߂�
//                float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
//                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
//                vec = vec.normalized;
//                BulletScript.vel = vec * 100;

//                //���������ȉ��ɂȂ�����e���A�N�e�B�u
//                if (interval < 4.0f)
//                {
//                    finishFlag = true;

//                }
//                break;

//            default:
//                break;
//        }
//    }

//    public override void StateTransition()
//    {
//        if (finishFlag)
//        {
//            PlayerScript.swingState = SwingState.NONE;
//            PlayerScript.mode = new PlayerStateMidair(0.0f);
//        }
//    }

//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Swing");
//    }
//}

///// <summary>
///// �v���C���[�������~�܂��Ă�����
///// �X�e�B�b�N���͂ł̈ړ��A�e�̔��˂��ł���
///// </summary>
//public class PlayerStateStand : PlayerState
//{
//    private bool shotButton;
//    public PlayerStateStand()//�R���X�g���N�^
//    {
//        shotButton = false;
//    }

//    public override void UpdateState()
//    {
//        if (Input.GetButtonDown("Button_R"))
//        {
//            if (PlayerScript.canShot)
//            {
//                shotButton = true;
//            }
//        }

//        if (PlayerScript.leftStick.x > 0.2f)
//        {
//            PlayerScript.dir = PlayerMoveDir.RIGHT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
//        }
//        else if (PlayerScript.leftStick.x < -0.2f)
//        {
//            PlayerScript.dir = PlayerMoveDir.LEFT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
//        }
//    }

//    public override void Move()
//    {
//        //����
//        if (PlayerScript.vel.x > PlayerScript.MIN_RUN_SPEED)
//        {
//            PlayerScript.vel *= PlayerScript.RUN_FRICTION;
//            if (PlayerScript.vel.x < PlayerScript.MIN_RUN_SPEED)
//            {
//                PlayerScript.vel.x = 0;
//            }
//        }
//        if (PlayerScript.vel.x < PlayerScript.MIN_RUN_SPEED * -1)
//        {
//            PlayerScript.vel *= PlayerScript.RUN_FRICTION;
//            if (PlayerScript.vel.x > PlayerScript.MIN_RUN_SPEED * -1)
//            {
//                PlayerScript.vel.x = 0;
//            }
//        }

//        //�d��
//        PlayerScript.vel += Vector3.down * 0.8f;
//        PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
//    }


//    public override void StateTransition()
//    {
//        //�E�ړ��Ɉڍs
//        if (PlayerScript.leftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD)
//        {
//            PlayerScript.dir = PlayerMoveDir.RIGHT;
//            PlayerScript.mode = new PlayerStateMove();
//        }

//        //���ړ��Ɉڍs
//        if (PlayerScript.leftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1)
//        {
//            PlayerScript.dir = PlayerMoveDir.LEFT;
//            PlayerScript.mode = new PlayerStateMove();
//        }

//        if (shotButton)
//        {
//            PlayerScript.mode = new PlayerStateShot();
//        }
//    }

//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Stand");
//    }
//}

///// <summary>
///// �v���C���[���ړ����Ă�����
///// �X�e�B�b�N���͂ł̈ړ��A�e�̔��˂��ł���
///// </summary>
//public class PlayerStateMove : PlayerState
//{
//    bool shotButton;

//    public PlayerStateMove()//�R���X�g���N�^
//    {
//        shotButton = false;

//        //�v���C���[�̉�]
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 0, 0);
//        }
//        if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 180, 0);
//        }
//    }

//    public override void UpdateState()
//    {
//        if (Input.GetButtonDown("Button_R"))
//        {
//            if (PlayerScript.canShot)
//            {
//                shotButton = true;
//            }
//        }
//    }

//    public override void Move()
//    {
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED;
//            PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);
//        }
//        if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * -1;
//            PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
//        }

//        //�d��
//        PlayerScript.vel += Vector3.down * 0.8f;
//        PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
//    }

//    public override void StateTransition()
//    {
//        //�X�e�B�b�N���������l�ȉ��Ȃ�stand��ԂɈڍs
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            if (PlayerScript.leftStick.x < (PlayerScript.LATERAL_MOVE_THRESHORD) - 0.1)
//            {
//                PlayerScript.mode = new PlayerStateStand();
//            }
//        }
//        if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            if (PlayerScript.leftStick.x > (PlayerScript.LATERAL_MOVE_THRESHORD * -1) + 0.1)
//            {
//                PlayerScript.mode = new PlayerStateStand();
//            }
//        }

//        if (shotButton)
//        {
//            PlayerScript.mode = new PlayerStateShot();
//        }
//    }


//    public override void DebugMessage()
//    {
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            Debug.Log("PlayerState:RunRight");
//        }
//        if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            Debug.Log("PlayerState:RunLeft");
//        }
//    }
//}

///// <summary>
///// �e�����������
///// �e�̓I�u�W�F�N�g�ɂ͐ڐG���Ă��Ȃ�
///// �X�e�B�b�N�ł̈ړ��s�A�e�������߂����Ƃ̂݉\
///// </summary>
//public class PlayerStateShot : PlayerState
//{
//    private enum SHOT_STATE
//    {
//        GO,         //��������ꂸ�ɔ��ł���
//        STRAINED,�@ //�v���C���[����������Ȃ�����ł���
//        RETURN,     //�v���C���[�ɖ߂��Ă���
//    }

//    SHOT_STATE shotState;               //�R������l�߂Ă��邩�i����l�߂Ă�����e�ɂ��Ă����j
//    float countTime;               //���˂���̎���
//    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //���˂���̒e��vector��ۑ�����
//    bool finishFlag;
//    BulletMain BulletScript;
//    public PlayerStateShot()//�R���X�g���N�^
//    {
//        PlayerScript.refState = EnumPlayerState.SHOT;
//        PlayerScript.canShot = false;
//        //�e�̐����Ɣ���
//        //���ˎ��ɂԂ���Ȃ��悤�ɔ��ˈʒu��������ɂ��炷
//        Vector3 vec = PlayerScript.leftStick.normalized;
//        Vector3 popPos = PlayerScript.transform.position + (vec * 5);
//        PlayerScript.Bullet = Object.Instantiate(PlayerScript.BulletPrefab, popPos, Quaternion.identity);
//        BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //�o���b�g���̃X�i�b�v
//    }

//    public override void UpdateState()
//    {
//        countTime += Time.deltaTime;

//        if (countTime > 0.3)
//        {
//            if (shotState == SHOT_STATE.STRAINED)
//            {
//                if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
//                {
//                    if (PlayerScript.Bullet != null)
//                    {
//                        BulletScript.ReturnBullet();
//                    }
//                    shotState = SHOT_STATE.RETURN;
//                }
//            }
//        }
//    }

//    public override void Move()
//    {

//        float interval;

//        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
//        switch (shotState)
//        {

//            case SHOT_STATE.GO:

//                bulletVecs.Enqueue(BulletScript.vel);

//                //�R�̒����𒴂�������������Ă����Ԃɂ���
//                if (interval > BulletScript.BULLET_ROPE_LENGTH)
//                {
//                    shotState = SHOT_STATE.STRAINED;
//                }

//                break;

//            case SHOT_STATE.STRAINED:
//                //�e�̐i�s�����̋t�����Ɉʒu�␳
//                bulletVecs.Enqueue(BulletScript.vel);
//                PlayerScript.vel = bulletVecs.Dequeue();

//                //����
//                //if (interval > BulletScript.BULLET_ROPE_LENGTH)
//                //{
//                //    Vector3 ballVel = BulletScript.rb.velocity;
//                //    Vector3 reverceVel = (ballVel * -1).normalized;

//                //    PlayerScript.transform.position = BulletScript.transform.position + reverceVel * BulletScript.BULLET_ROPE_LENGTH;
//                //}
//                break;

//            case SHOT_STATE.RETURN:
//                //�����֒e�������߂�
//                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
//                vec = vec.normalized;

//                BulletScript.vel = vec * 50;

//                //���������ȉ��ɂȂ�����I�������t���O�����Ă�
//                if (interval < 4.0f)
//                {
//                    finishFlag = true;
//                }

//                break;
//        }


//    }

//    public override void StateTransition()
//    {
//        if (finishFlag)
//        {
//            //���n�����痧���Ă����ԂɈڍs
//            if (PlayerScript.isOnGround)
//            {
//                PlayerScript.mode = new PlayerStateOnGround();
//            }
//            else //�����łȂ��Ȃ��
//            {
//                PlayerScript.mode = new PlayerStateMidair();
//            }
//        }

//        //�{�[�����G�ꂽ��X�C���O���
//        if (BulletScript.isTouched)
//        {
//            PlayerScript.mode = new PlayerStateSwing();
//        }
//    }
//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Shot");
//    }
//}



///// <summary>
///// �e���V��ŃX�C���O���Ă�����
///// </summary>
//public class PlayerStateSwing : PlayerState
//{

//    enum SwingMode
//    {
//        Touced,   //�߂܂��Ă�����
//        Rereased, //�؂藣�������
//    }

//    SwingMode swingMode;
//    private bool finishFlag;
//    private bool shotButton;
//    private Vector3 ballPosition;
//    private const float SWING_ANGLER_VELOCITY = 2.7f; //�U��q�p���x ��const�l�ł͂Ȃ��O��state��velocity�ŉς��ǂ�����
//    private const float SWING_REREASE_ANGLE = 140.0f; //�U��q���̂Ȃ��p�����ȏ�ɂȂ����狭������

//    public PlayerStateSwing()//�R���X�g���N�^
//    {
//        PlayerScript.refState = EnumPlayerState.SWING;
//        PlayerScript.canShotState = false;
//        finishFlag = false;
//        swingMode = SwingMode.Touced;
//        shotButton = false;
//        BulletScript.rb.isKinematic = true;
//        ballPosition = BulletScript.gameObject.transform.position;
//        PlayerScript.rb.velocity = Vector3.zero;
//        PlayerScript.vel = Vector3.zero;
//    }

//    public override void UpdateState()
//    {
//        //if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
//        //{
//        //    BulletScript.rb.isKinematic = false;
//        //    shotButton = true;
//        //}

//        //�{�[���v���C���[�Ԃ̊p�x�����߂�
//        Vector3 dt = Player.transform.position - ballPosition;
//        float rad = Mathf.Atan2(dt.x, dt.y);
//        float degree = rad * Mathf.Rad2Deg;
//        if (degree < 0)//���������Ɏ��v����0~360�̒l�ɕ␳
//        {
//            degree += 360;
//        }

//        //���p�x�ȏ�Ő؂藣��
//        if (swingMode == SwingMode.Touced)
//        {
//            if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//            {
//                if (degree < SWING_REREASE_ANGLE)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    swingMode = SwingMode.Rereased;

//                    //�����ǉ�
//                    //�e�ƃv���C���[�Ԃ̃x�N�g���ɒ��s����x�N�g��
//                    Vector3 addVec = ballPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, -110) * addVec;
//                    PlayerScript.vel += addVec * 35.0f;
//                }
//            }
//            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//            {
//                if (degree > 360 - SWING_REREASE_ANGLE)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    swingMode = SwingMode.Rereased;

//                    //�����ǉ�
//                    //�e�ƃv���C���[�Ԃ̃x�N�g���ɒ��s����x�N�g��
//                    Vector3 addVec = ballPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, 110) * addVec;
//                    PlayerScript.vel += addVec * 35.0f;
//                }
//            }
//        }
//    }

//    public override void Move()
//    {
//        ballPosition = BulletScript.transform.position;

//        switch (swingMode)
//        {
//            case SwingMode.Touced:
//                //�����ɂ���ĉ�]�������Ⴄ
//                Quaternion angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY, Vector3.forward);
//                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//                {
//                    angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY, Vector3.forward);
//                }
//                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//                {
//                    angleAxis = Quaternion.AngleAxis(SWING_ANGLER_VELOCITY * -1, Vector3.forward);
//                }

//                Vector3 pos = Player.transform.position;

//                pos -= ballPosition;
//                pos = angleAxis * pos;
//                pos += ballPosition;
//                PlayerScript.transform.position = pos;
//                break;

//            case SwingMode.Rereased:
//                //�����֒e�������߂�
//                float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
//                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
//                vec = vec.normalized;

//                BulletScript.vel = vec * 100;

//                //���������ȉ��ɂȂ�����e���A�N�e�B�u

//                if (interval < 4.0f)
//                {
//                    finishFlag = true;
//                }

//                break;

//            default:
//                break;
//        }
//    }

//    public override void StateTransition()
//    {
//        if (finishFlag)
//        {
//            PlayerScript.mode = new PlayerStateMidair(0.0f);
//        }
//    }

//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Swing");
//    }
//}