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

    static public GameObject Player;
    static public PlayerMain PlayerScript;
    static public BulletMain BulletScript;

    /// <summary>
    /// �o���b�g�̈ʒu����ɃX�e�B�b�N�����ɒ���
    /// </summary>
    protected void BulletAdjust()
    {
        Vector3 vec = PlayerScript.adjustLeftStick.normalized;
        vec = vec * 3;
        vec.y += 1.0f;
        Vector3 adjustPos = PlayerScript.transform.position + vec;

        BulletScript.transform.position = adjustPos;
    }

}

/// <summary>
/// �v���C���[���n��ɂ�����
/// �X�e�B�b�N�ňړ��A�e�̔��˂��ł���
/// </summary>
public class PlayerStateOnGround : PlayerState
{
    private bool shotButton;
    private bool jumpButton;
    private const float SLIDE_END_TIME = 0.3f; 
    private float slideEndTimer;

    public PlayerStateOnGround()//�R���X�g���N�^
    {
        PlayerScript.refState = EnumPlayerState.ON_GROUND;
        shotButton = false;
        jumpButton = false;
        PlayerScript.vel.y = 0;
        PlayerScript.canShotState = true;
        slideEndTimer = 0.0f;

        //�{�[���֘A
        BulletScript.InvisibleBullet();


        //�X���C�h���ˏ���
        if (Mathf.Abs(PlayerScript.vel.x) > 30.0f)
        {
            PlayerScript.onGroundState = OnGroundState.SLIDE;
        }
        else
        {
            PlayerScript.onGroundState = OnGroundState.NORMAL;
        }
    }

    public override void UpdateState()
    {
        BulletAdjust();

        if (Input.GetButtonDown("Button_R"))
        {
            if (PlayerScript.canShot)
            {
                shotButton = true;
            }
        }
        else if (Input.GetButtonDown("Jump"))
        {
            jumpButton = true;
            Debug.Log("Press Jump Button");
        }

        //�v���C���[������]����
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            if (PlayerScript.adjustLeftStick.x < -0.01f)
            {
                PlayerScript.dir = PlayerMoveDir.LEFT;
                PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
            }
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            if (PlayerScript.adjustLeftStick.x > 0.01f)
            {
                PlayerScript.dir = PlayerMoveDir.RIGHT;
                PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
    }

    public override void Move()
    {

        if (PlayerScript.onGroundState == OnGroundState.SLIDE)
        {
            float slide_Weaken = 0.5f;

            if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD) //�E�ړ�
            {
                if (PlayerScript.vel.x < -0.2f)//�^�[�����Ă�Ƃ��͑���
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * 2 * slide_Weaken * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * slide_Weaken * 0.4f * (fixedAdjust); 
                }

                //PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);
            }
            else if (PlayerScript.adjustLeftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1) //���ړ�
            {

                if (PlayerScript.vel.x > 0.2f)
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * -1 * 2 * slide_Weaken * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * -1 * slide_Weaken * 0.4f * (fixedAdjust);
                }
                //PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
            }


            //����
            {
                PlayerScript.vel *= 0.97f;
            }

            //�X���C�h�I�������i���Ԃɂ�����
            slideEndTimer += Time.fixedDeltaTime;
            if(slideEndTimer > SLIDE_END_TIME)
            {
                PlayerScript.onGroundState = OnGroundState.NORMAL;
                PlayerScript.canShotState = true;
            }
        }
        else //!isSlide
        {
            if (PlayerScript.adjustLeftStick.x > PlayerScript.LATERAL_MOVE_THRESHORD) //�E�ړ�
            {
                if (PlayerScript.vel.x < -0.2f)
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * 2 * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * (fixedAdjust);
                }

                PlayerScript.vel.x = Mathf.Min(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED);
            }
            else if (PlayerScript.adjustLeftStick.x < PlayerScript.LATERAL_MOVE_THRESHORD * -1) //���ړ�
            {

                if (PlayerScript.vel.x > 0.2f)
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * -1 * 2 * (fixedAdjust);
                }
                else
                {
                    PlayerScript.vel.x += PlayerScript.ADD_RUN_SPEED * Mathf.Pow(Mathf.Abs(PlayerScript.adjustLeftStick.x)  , 3) * -1 * (fixedAdjust);
                }
                PlayerScript.vel.x = Mathf.Max(PlayerScript.vel.x, PlayerScript.MAX_RUN_SPEED * -1);
            }
            else //����
            {
                PlayerScript.vel *= PlayerScript.RUN_FRICTION; 
            }
        }
    }


    public override void StateTransition()
    {
        if (PlayerScript.isOnGround == false)
        {
            PlayerScript.onGroundState = OnGroundState.NONE;
            PlayerScript.mode = new PlayerStateMidair(true);
        }

        if (shotButton)
        {
            //�X���C�h���œ�����������i�s�����Ɠ����Ȃ�
            if ((PlayerScript.onGroundState == OnGroundState.SLIDE) && (PlayerScript.adjustLeftStick.x * PlayerScript.vel.x > 0))
            {
                PlayerScript.onGroundState = OnGroundState.NONE;
                PlayerScript.mode = new PlayerStateShot_2(true);
            }
            else
            {
                PlayerScript.onGroundState = OnGroundState.NONE;
                PlayerScript.mode = new PlayerStateShot_2(false);
            }
        }

        if (jumpButton)
        {
            Debug.Log("Jump!!!!");
            PlayerScript.onGroundState = OnGroundState.NONE;
            PlayerScript.mode = new PlayerStateMidair(true, true);
        }
    }
}

/// <summary>
/// �e�����������(��x�R���L�ѐ؂����璷���Œ�̂���)
/// �e�̓I�u�W�F�N�g�ɂ͐ڐG���Ă��Ȃ�
/// �X�e�B�b�N�ł̈ړ��s�A�e�������߂����Ƃ̂݉\
/// </summary>
public class PlayerStateShot_2 : PlayerState
{
    float countTime;               //���˂���̎���
    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //���˂���̒e��vector��ۑ�����
    bool finishFlag;

    const float STRAINED_END_RATIO = 1.0f;

    private void Init()
    {
        countTime = 0.0f;
        bulletVecs = new Queue<Vector3>();
        finishFlag = false;

        PlayerScript.refState = EnumPlayerState.SHOT;
        PlayerScript.shotState = ShotState.GO;
        PlayerScript.canShotState = false;
        PlayerScript.forciblyReturnBulletFlag = false;
        PlayerScript.addVel = Vector3.zero;
        
    }

    public PlayerStateShot_2(bool is_slide_jump)//�R���X�g���N�^
    {
        Init();
        //�e�̔���
        BulletScript.GetComponent<Collider>().isTrigger = false;
        BulletScript.VisibleBullet();

        if (is_slide_jump)
        {
            BulletScript.ShotSlideJumpBullet();
            Debug.Log("Slide Shot");
        }
        else
        {
            BulletScript.ShotBullet();
            Debug.Log("Normal Shot");
        }
    }

    /// <summary>
    /// ���������Ă���Ƃ��A�v���C���[��i�s�����ɑ΂��ĉ�]
    /// </summary>
    public void RotationPlayer()
    {

        switch (PlayerScript.shotState)
        {
            case ShotState.STRAINED:

                Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;

                Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);
                Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //�␳�p�N�I�[�^�j�I��
                quaternion *= adjustQua;
                PlayerScript.rb.rotation = quaternion;
                break;

            case ShotState.RETURN:
            case ShotState.FOLLOW:
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 90, 0));         
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, -90, 0));
                }
                break;
        }

      
    }

    /// <summary>
    /// ���������Ă��鎞�ԂɃI�u�W�F�N�g����������؂藣��
    /// </summary>
    private void StrainedStop()
    {
        Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;
        Ray ray = new Ray(PlayerScript.rb.position, vecToPlayer.normalized);

        if (Physics.SphereCast(ray, PlayerMain.HcolliderRadius, PlayerMain.HcoliderDistance, LayerMask.GetMask("Platform")))
        { 
            if(BulletScript.isTouched == false)
            {
                Debug.Log("collision PlayerHead : Forcibly return");
                PlayerScript.ForciblyReturnBullet(true);
            }
        }
    }

    public override void UpdateState()
    {
        countTime += Time.deltaTime;

        if (countTime > 0.2)
        {
            if (PlayerScript.shotState == ShotState.STRAINED)
            {
                if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
                {
                   
                    BulletScript.ReturnBullet();
                    
                    PlayerScript.vel = bulletVecs.Dequeue() * STRAINED_END_RATIO;
                    
                    PlayerScript.useVelocity = true;
                    PlayerScript.shotState = ShotState.RETURN;           
                }
            }
        }

        //�A���J�[���h����Ȃ��ǂɂ��������Ƃ��ȂǁA�O���_�@�ň����߂��Ɉڍs
        if (PlayerScript.forciblyReturnBulletFlag)
        {
            PlayerScript.forciblyReturnBulletFlag = false;
           
            if (PlayerScript.forciblyReturnSaveVelocity)
            {
                PlayerScript.vel = bulletVecs.Dequeue() * STRAINED_END_RATIO;
            }
            else
            {
                PlayerScript.vel = Vector3.zero;
            }

            BulletScript.ReturnBullet();
            
            PlayerScript.useVelocity = true;
            PlayerScript.shotState = ShotState.RETURN;
        }

        //���Ă�������
        if (PlayerScript.shotState == ShotState.STRAINED)
        {
#if false
            //�e����v���C���[������BULLET_ROPE_LENGTH�������ꂽ�ʒu�ɏ�ɕ␳
            Vector3 diff = (PlayerScript.transform.position - BulletScript.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
            Player.transform.position = BulletScript.transform.position + diff;
#else
            //�e����v���C���[������BULLET_ROPE_LENGTH�������ꂽ�ʒu�ɏ�ɕ␳
            if(Vector3.Magnitude(Player.transform.position - BulletScript.transform.position) > BulletScript.BULLET_ROPE_LENGTH)
            {
                Vector3 diff = (PlayerScript.transform.position - BulletScript.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
                Player.transform.position = BulletScript.transform.position + diff;
                //�e���v���C���[��苭�������������Ă���Ƃ��̂�
                if (PlayerScript.vel.magnitude < BulletScript.vel.magnitude * 0.8f)
                {
                    PlayerScript.vel = BulletScript.vel * 0.8f;
                }
            }

            //STRAINED�����ǎ��R�ړ��̃^�C�~���O
            else
            {
                //��߂̏d�� 
                PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * 0.1f * (fixedAdjust);
                PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
            }
#endif
        }

        if (BulletScript.isTouched)
        {
            if (BulletScript.followEnd)
            {
                BulletScript.FollowedPlayer();

                PlayerScript.vel = bulletVecs.Dequeue();
                PlayerScript.useVelocity = true;
                BulletScript.followEnd = false;
                PlayerScript.shotState = ShotState.FOLLOW;
            }
        }

    }

    public override void Move()
    {
        float interval;
        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);

        RotationPlayer();

        switch (PlayerScript.shotState) {

            case ShotState.GO: 
                bulletVecs.Enqueue(BulletScript.vel * 0.6f);

                //�R�̒����𒴂�������������Ă����Ԃɂ���
                if (interval > BulletScript.BULLET_ROPE_LENGTH)
                {
                    //��������ꂽ�^�C�~���O�Ń{�[������
                    //if(BulletScript.vel.magnitude > 60.0f)
                    //{
                    //    BulletScript.vel *= 0.64f;
                    //}
                    //else if(BulletScript.vel.magnitude > 40.0f)
                    //{
                    //    BulletScript.vel *= 0.92f;
                    //}

                    BulletScript.vel *= 0.84f;

                    PlayerScript.shotState = ShotState.STRAINED;
                    //PlayerScript.useVelocity = false;
                }
                break;

            case ShotState.STRAINED:
               
                bulletVecs.Enqueue(BulletScript.vel);
                bulletVecs.Dequeue();
                StrainedStop();
                //���̂Ƃ��A�ړ������͒���position�ύX���Ă��邽��???????�Aupdate���ɋL�q
                //�����ɋL�q����ƃJ�������u����
                break;

            case ShotState.RETURN:
                //�����֒e�������߂�
                Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
                vecToPlayer = vecToPlayer.normalized;

                BulletScript.vel = vecToPlayer * 100;

                //���������ȉ��ɂȂ�����I�������t���O�����Ă�
                if (interval < 4.0f)
                {
                    finishFlag = true;
                }
                break;

            case ShotState.FOLLOW:
                //�����֒e�������߂�
                Vector3 vecToBullet = BulletScript.rb.position - PlayerScript.rb.position;
                vecToBullet = vecToBullet.normalized;

                PlayerScript.vel += vecToBullet * 3;

                if (interval < 4.0f)
                {
                    finishFlag = true;
                }

                break;
        }

        
    }

    public override void StateTransition()
    {
        //�{�[�����G�ꂽ��X�C���O���
        if (BulletScript.isTouched)
        {
            PlayerScript.shotState = ShotState.NONE;
            if (BulletScript.swingEnd)
            {
                BulletScript.swingEnd = false;
                PlayerScript.mode = new PlayerStateSwing_R_Release();
            }
        }

        if (finishFlag)
        {
            PlayerScript.shotState = ShotState.NONE;
            //���n�����痧���Ă����ԂɈڍs
            if (PlayerScript.isOnGround)
            {
                PlayerScript.mode = new PlayerStateOnGround();
            }
            else //�����łȂ��Ȃ��
            {
                PlayerScript.mode = new PlayerStateMidair(false);
            }
        } 
  
    }
    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Shot");
    }
}


/// <summary>
/// �e�����������(��x�R���L�ѐ؂����璷���Œ�̂���)
/// �e�̓I�u�W�F�N�g�ɂ͐ڐG���Ă��Ȃ�
/// �X�e�B�b�N�ł̈ړ��s�A�e�������߂����Ƃ̂݉\
/// </summary>
//public class PlayerStateShot_3 : PlayerState
//{
//    float countTime;               //���˂���̎���
//    Queue<Vector3> bulletVecs = new Queue<Vector3>();     //���˂���̒e��vector��ۑ�����
//    bool finishFlag;
//    BulletMain BulletScript;
//    public PlayerStateShot_3()//�R���X�g���N�^
//    {
//        PlayerScript.refState = EnumPlayerState.SHOT;
//        PlayerScript.shotState = ShotState.GO;
//        PlayerScript.canShotState = false;
//        PlayerScript.forciblyReturnBulletFlag = false;
//        PlayerScript.addVel = Vector3.zero;
//        //�e�̐����Ɣ���
//        //���ˎ��ɂԂ���Ȃ��悤�ɔ��ˈʒu��������ɂ��炷
//        Vector3 vec = PlayerScript.leftStick.normalized;
//        vec = vec * 5;
//        vec.y += 1.0f;
//        Vector3 popPos = PlayerScript.transform.position + vec;

//        if (ReferenceEquals(PlayerScript.Bullet, null) == false)
//        {
//            GameObject.Destroy(PlayerScript.Bullet);
//            PlayerScript.Bullet = null;
//        }

//        PlayerScript.Bullet = Object.Instantiate(PlayerScript.BulletPrefab, popPos, Quaternion.identity);
//        BulletScript = PlayerScript.Bullet.GetComponent<BulletMain>(); //�o���b�g���̃X�i�b�v
//    }

//    public override void UpdateState()
//    {
//        countTime += Time.deltaTime;

//        if (countTime > 0.3)
//        {
//            if (PlayerScript.shotState == ShotState.STRAINED)
//            {
//                if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
//                {
//                    if (PlayerScript.Bullet != null)
//                    {
//                        BulletScript.ReturnBullet();
//                    }
//                    PlayerScript.vel = bulletVecs.Dequeue();
//                    PlayerScript.useVelocity = true;
//                    PlayerScript.shotState = ShotState.RETURN;
//                }
//            }
//        }

//        //�A���J�[���h����Ȃ��ǂɂ��������Ƃ��ȂǁA�O���_�@�ň����߂��Ɉڍs
//        if (PlayerScript.forciblyReturnBulletFlag)
//        {
//            PlayerScript.forciblyReturnBulletFlag = false;
//            if (PlayerScript.Bullet != null)
//            {
//                if (PlayerScript.forciblyReturnSaveVelocity)
//                {
//                    PlayerScript.vel = bulletVecs.Dequeue();
//                }
//                else
//                {
//                    PlayerScript.vel = Vector3.zero;
//                }

//                BulletScript.ReturnBullet();
//            }
//            PlayerScript.useVelocity = true;
//            PlayerScript.shotState = ShotState.RETURN;
//        }

//        if (PlayerScript.shotState == ShotState.STRAINED)
//        {
//            float interval;
//            interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);

           
//            if (interval > BulletScript.BULLET_ROPE_LENGTH)
//            {
//                //�e����v���C���[������BULLET_ROPE_LENGTH�������ꂽ�ʒu�ɏ�ɕ␳
//                PlayerScript.useVelocity = false;
//                Vector3 diff = (PlayerScript.transform.position - PlayerScript.Bullet.transform.position).normalized * BulletScript.BULLET_ROPE_LENGTH;
//                Player.transform.position = PlayerScript.Bullet.transform.position + diff;
//            }
//            else
//            {
//                PlayerScript.useVelocity = true;
//            }
//        }

//        if (BulletScript.isTouched)
//        {
//            if (BulletScript.followEnd)
//            {
//                if (PlayerScript.Bullet != null)
//                {
//                    BulletScript.FollowedPlayer();
//                }
//                PlayerScript.vel = bulletVecs.Dequeue();
//                PlayerScript.useVelocity = true;
//                BulletScript.followEnd = false;
//                PlayerScript.shotState = ShotState.FOLLOW;
//            }
//        }
//    }

//    public override void Move()
//    {

//        float interval;
//        interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
//        switch (PlayerScript.shotState)
//        {

//            case ShotState.GO:
//                bulletVecs.Enqueue(BulletScript.vel);

//                //�R�̒����𒴂�������������Ă����Ԃɂ���
//                if (interval > BulletScript.BULLET_ROPE_LENGTH)
//                {
//                    PlayerScript.shotState = ShotState.STRAINED;
//                    PlayerScript.useVelocity = false;
//                }
//                break;

//            case ShotState.STRAINED:
//                bulletVecs.Enqueue(BulletScript.vel);
//                if (PlayerScript.useVelocity == true)
//                {
//                    PlayerScript.vel = bulletVecs.Peek(); //* (1 /Time.fixedDeltaTime); //PlayerScript.FALL_GRAVITY; //
//                }

//                bulletVecs.Dequeue();
//                //���̂Ƃ��A�ړ������͒���position�ύX���Ă��邽��???????�Aupdate���ɋL�q
//                //�����ɋL�q����ƃJ�������u����
                
//                break;

//            case ShotState.RETURN:
//                //�����֒e�������߂�
//                Vector3 vecToPlayer = PlayerScript.rb.position - BulletScript.rb.position;
//                vecToPlayer = vecToPlayer.normalized;

//                BulletScript.vel = vecToPlayer * 100;

//                //���������ȉ��ɂȂ�����I�������t���O�����Ă�
//                if (interval < 4.0f)
//                {
//                    finishFlag = true;
//                }
//                break;

//            case ShotState.FOLLOW:
//                //�����֒e�������߂�
//                Vector3 vecToBullet = BulletScript.rb.position - PlayerScript.rb.position;
//                vecToBullet = vecToBullet.normalized;

//                PlayerScript.vel += vecToBullet * 3;

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
//                PlayerScript.mode = new PlayerStateOnGround(true);
//            }
//            else //�����łȂ��Ȃ��
//            {
//                PlayerScript.mode = new PlayerStateMidair();
//            }
//        }

//        //�{�[�����G�ꂽ��X�C���O���
//        if (BulletScript.isTouched)
//        {
//            if (BulletScript.swingEnd)
//            {
//                BulletScript.swingEnd = false;
//                PlayerScript.mode = new PlayerStateSwing_2();
//            }
//        }
//    }
//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Shot");
//    }
//}


/// <summary>
/// �v���C���[���󒆂ɂ�����
/// �N�[���^�C�����o�āA�e�̔��˂��ł���
/// </summary>
public class PlayerStateMidair : PlayerState
{
    private bool shotButton;  
    private bool OnceFallDownFlag;//�}�~���t���O
    private void Init()
    {     
        PlayerScript.refState = EnumPlayerState.MIDAIR;
        PlayerScript.midairState = MidairState.NORMAL;
        shotButton = false;
        PlayerScript.canShotState = false;
        OnceFallDownFlag = false;

        BulletScript.InvisibleBullet();
    }

    public PlayerStateMidair(bool can_shot)//�R���X�g���N�^
    {
        Init();
        PlayerScript.canShotState = can_shot;
    }

    public PlayerStateMidair(bool Jump_start, bool can_shot)//�R���X�g���N�^
    {
        Init();
        PlayerScript.canShotState = can_shot;

        if (Jump_start)
        {
            PlayerScript.isOnGround = false;
            PlayerScript.vel.y += 80;
        }
    }

    public override void UpdateState()
    {
        BulletAdjust();

        if (PlayerScript.adjustLeftStick.x > 0.01f)
        {
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (PlayerScript.adjustLeftStick.x < -0.01f)
        {
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
        }

        if (Input.GetButtonDown("Button_R"))
        {
            if (PlayerScript.canShot)
            {
                shotButton = true;
            }
        }

        //�}�~�����͉��H
        if (PlayerScript.sourceLeftStick.y < -0.7f && Mathf.Abs(PlayerScript.sourceLeftStick.x) < 0.3f)
        {
            //��x�ł����͂��ꂽ��i�v��
            PlayerScript.midairState = MidairState.FALL;
        }

    }

    public override void Move()
    {
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
        if(PlayerScript.midairState == MidairState.FALL)
        {
            //�v���C���[����Ɍ������Ă���Ƃ��͑���
            if(PlayerScript.vel.y > 0.0f)
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
        else if(PlayerScript.midairState == MidairState.NORMAL)
        {
            PlayerScript.vel += Vector3.down * PlayerScript.FALL_GRAVITY * (fixedAdjust);
            PlayerScript.vel.y = Mathf.Max(PlayerScript.vel.y, PlayerScript.MAX_FALL_SPEED * -1);
        }
    }


    public override void StateTransition()
    { 
        if (shotButton)
        {
            PlayerScript.midairState = MidairState.NONE;
            PlayerScript.mode = new PlayerStateShot_2(false);
        }

        //���n�����痧���Ă����ԂɈڍs
        if (PlayerScript.isOnGround)
        {
            PlayerScript.midairState = MidairState.NONE;
            PlayerScript.mode = new PlayerStateOnGround();
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:MidAir");
    }
}



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



/// <summary>
/// �e���V��ŃX�C���O���Ă�����
/// �ړ����x����ۑ�
/// </summary>
public class PlayerStateSwing_R_Release : PlayerState
{
    private bool finishFlag;
    private bool releaseButton;
    private bool countreButton;
    private Vector3 BulletPosition; //�{�[���̈ʒu
   
    private float betweenLength; //�J�n����_�Ԃ̋���(�����̓X�C���Ostate�ʂ��ČŒ�)
    private Vector3 startPlayerVel;�@�@�@�@�@�@ //�˓���velocity
    private float startAngle;    //�J�n���̓�_�ԃA���O��
    private float endAngle;      //�����؂藣�������p�x(start�p�x�ˑ�)
    private float minAnglerVel;  //�Œ�p���x�i�����؂藣���n�_�ɂ��鎞�j
    private float maxAnglerVel;�@//�ō��p���x (�^���Ƀv���C���[�����鎞�j
    private float nowAnglerVel;  //���݊p���x

    Vector3 LastBtoP_Angle;  //�Ō�Ɍv�������o���b�g���v���C���[�̐��K��Vector
    Vector3 AfterBtoP_Angle; //�p���x�v�Z��̃o���b�g���v���C���[�̐��K��Vector


    const float SWING_END_RATIO = 1.4f;

    public PlayerStateSwing_R_Release()  //�R���X�g���N�^
    {
        BulletPosition = BulletScript.gameObject.transform.position;

        //�v�Z�p���i�[
        startPlayerVel = BulletScript.vel;
        betweenLength = Vector3.Distance(Player.transform.position, BulletPosition);
        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);
        startAngle = endAngle = degree;

        PlayerScript.refState = EnumPlayerState.SWING;
        PlayerScript.swingState = SwingState.TOUCHED;
        PlayerScript.canShotState = false;
        PlayerScript.endSwing = false;
        PlayerScript.hangingSwing = false;
        finishFlag = false;
        releaseButton = false;
        countreButton = false;
        BulletScript.rb.isKinematic = true;
        PlayerScript.rb.velocity = Vector3.zero;
        PlayerScript.vel = Vector3.zero;

        CalculateStartVariable();
    }

    ~PlayerStateSwing_R_Release()
    {

    }

    /// <summary>
    /// �U��q����p�̊e��ϐ����v�Z
    /// </summary>
    public void CalculateStartVariable()
    {

        //�R�̒����ƃX�s�[�h����p���x���v�Z
        float angler_velocity;
        float tempY = Mathf.Min(startPlayerVel.y, 0.0f);
        angler_velocity = (Mathf.Abs(startPlayerVel.x) * 2.5f + Mathf.Abs(tempY) * 1.5f);
        angler_velocity /= (betweenLength * 2.0f * Mathf.PI);

        //�͈͓��ɕ␳
        angler_velocity = Mathf.Clamp(angler_velocity, 1.0f, 3.0f);

        nowAnglerVel = maxAnglerVel = minAnglerVel = angler_velocity;

        Debug.Log("AnglerVelocity: " + angler_velocity);

        //�o���b�g����v���C���[�̃A���O����ۑ�
        LastBtoP_Angle = AfterBtoP_Angle = (Player.transform.position - BulletPosition).normalized;

        //�؂藣���A���O���̌v�Z
        float diff_down = Mathf.Abs(startAngle - 180.0f); //�^���Ɠ˓��p�̍�
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
            endAngle -= (diff_down + diff_down);
            //�J�n�_���͍����ʒu�ɂ���
            endAngle -= 30;

            //�͈͓��ɕ␳
            endAngle = Mathf.Clamp(endAngle, 90, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
            endAngle += (diff_down + diff_down);
            //�J�n�_���͍����ʒu�ɂ���
            endAngle += 30;

            //�͈͓��ɕ␳
            endAngle = Mathf.Clamp(endAngle, 220, 270);
        }

        //�Œᑬ�͓˓����v���C���[velocity
        maxAnglerVel = minAnglerVel = angler_velocity;
        //�ō����x�͓˓��p���傫���قǑ���
        float velDiff = Mathf.Clamp01((diff_down / 90));
        maxAnglerVel += velDiff * 1.2f;
    }

    /// <summary>
    /// �ǒ��˕Ԃ莞�̊e��v�Z
    /// </summary>
    public void CalculateCounterVariable()
    {
        Debug.Log("counter:");

        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            //�v���C���[��]����
            PlayerScript.dir = PlayerMoveDir.LEFT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //�v���C���[��]����
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
        }

        //�؂藣���A���O���̌v�Z
        ReleaseAngleCalculate();
    }

    private void ReleaseAngleCalculate()
    {
        float diff_down = Mathf.Abs(endAngle - 180.0f); //�^���ƏI���p�̍�
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
            endAngle -= (diff_down + diff_down);
            //�J�n�_���͍����ʒu�ɂ���
            endAngle -= 10;

            //�͈͓��ɕ␳
            endAngle = Mathf.Clamp(endAngle, 90, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
            endAngle += (diff_down + diff_down);
            //�J�n�_��荂���ʒu�ɂ���
            endAngle += 10;

            //�͈͓��ɕ␳
            endAngle = Mathf.Clamp(endAngle, 220, 270);
        }
    }

    public void RotationPlayer()
    {

        switch (PlayerScript.swingState)
        {
            case SwingState.TOUCHED:
                float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);

                Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;
                Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);

                Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //�␳�p�N�I�[�^�j�I��

                quaternion *= adjustQua;

                if(PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    if(degree < 180)
                    {
                        quaternion *= Quaternion.Euler(0, 180, 0);
                    }
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    if (degree > 180)
                    {
                        quaternion *= Quaternion.Euler(0, 180, 0);
                    }
                }

                PlayerScript.rb.rotation = quaternion;
                break;

            case SwingState.RELEASED:
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 90, 0));
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, -90, 0));
                }
                break;
        }


    }

    public void InputButton()
    {
        if(PlayerScript.swingState != SwingState.RELEASED)
        {
            if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
            {

                releaseButton = true;
            }

        }

        if (PlayerScript.swingState == SwingState.HANGING)
        {
            if (Input.GetButtonDown("Jump"))
            {
                countreButton = true;
                Debug.Log("Press Jump");
            }
        }
        
    }





    public override void UpdateState()
    {
        //�؂藣������
        InputButton();
        
        //�e�̏ꏊ�X�V
        BulletPosition = BulletScript.rb.position;

        //�{�[���v���C���[�Ԃ̊p�x�����߂�
        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);

        //�؂藣��
        if (PlayerScript.swingState == SwingState.TOUCHED)
        {
            if (PlayerScript.endSwing)
            {
                PlayerScript.endSwing = false;
                PlayerScript.useVelocity = true;
                BulletScript.ReturnBullet();
                PlayerScript.swingState = SwingState.RELEASED;
            }

            if (PlayerScript.dir == PlayerMoveDir.RIGHT)
            {
                if ((degree < endAngle) || (releaseButton == true))
                {
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;

                    //�����ǉ�
                    //�e�ƃv���C���[�Ԃ̃x�N�g���ɒ��s����x�N�g��
                    Vector3 addVec = BulletPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, -90) * addVec;


                    float mutipleVec = (nowAnglerVel / 20.0f) + 1.0f;

                    PlayerScript.vel += addVec * mutipleVec * 40.0f * SWING_END_RATIO;
                }
            }
            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
            {
                if ((degree > endAngle) || (releaseButton == true))
                {
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;

                    //�����ǉ�
                    //�e�ƃv���C���[�Ԃ̃x�N�g���ɒ��s����x�N�g��
                    Vector3 addVec = BulletPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, 90) * addVec;


                    float mutipleVec = (nowAnglerVel / 20.0f) + 1.0f;

                    PlayerScript.vel += addVec * mutipleVec * 40.0f * SWING_END_RATIO;
                }
            }
        }

       
    }

    public override void Move()
    {
        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position); //�o���b�g����v���C���[�x�N�g��
        float deg180dif = Mathf.Abs(degree - 180); //�v���C���[����x�N�g��

        RotationPlayer();

        switch (PlayerScript.swingState)
        {
            case SwingState.TOUCHED:
                //�Ԃ牺���菈��
                if (PlayerScript.hangingSwing)
                {
                    PlayerScript.swingState = SwingState.HANGING;
                    PlayerScript.hangingSwing = false;
                }
               
                //�Z�����鏈��
                if (PlayerScript.shortSwing.isShort)
                {
                    betweenLength = PlayerScript.shortSwing.length;
                    PlayerScript.shortSwing.isShort = false;
                }

                //�p���x�v�Z
                float deg180Ratio = deg180dif / Mathf.Abs(endAngle - 180); //�^���ƍō����B�_�̔䗦
                deg180Ratio = Mathf.Clamp01(deg180Ratio); //�ꉞ�͈͓��ɕ␳
                deg180Ratio = 1 - deg180Ratio; //�^����1,�ō����B�_��0�Ƃ���

                float easeDeg180Ratio = Easing.Linear(deg180Ratio, 1.0f, 0.0f, 1.0f);

                nowAnglerVel = ((maxAnglerVel - minAnglerVel) * easeDeg180Ratio) + minAnglerVel;//�p���x�i�ʁj

                //�O��v�Z���AfterAngle�������Ă���
                LastBtoP_Angle = AfterBtoP_Angle;

                //�����p���x����
                //�����ɂ���ĉ�]�������Ⴄ
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    AfterBtoP_Angle = Quaternion.Euler(0, 0, nowAnglerVel * 1) * LastBtoP_Angle;

                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    AfterBtoP_Angle = Quaternion.Euler(0, 0, nowAnglerVel * -1) * LastBtoP_Angle;

                }

                //�{�[�����W �{ ���K��������]��A���O�� �� ����
                Vector3 pos = BulletPosition + (AfterBtoP_Angle.normalized) * betweenLength;

                PlayerScript.transform.position = pos;
                break;

            case SwingState.RELEASED:
                //�����֒e�������߂�
                float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
                vec = vec.normalized;
                BulletScript.vel = vec * 100;
                //���������ȉ��ɂȂ�����e���A�N�e�B�u
                if (interval < 4.0f)
                {
                    finishFlag = true;

                }
                break;

            case SwingState.HANGING:
                //���]����
                if (countreButton)
                { 

                    PlayerScript.swingState = SwingState.TOUCHED;
                    CalculateCounterVariable();
                    countreButton = false;
                }

                if (releaseButton)
                {
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;

                    PlayerScript.vel = Vector3.zero;
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
            PlayerScript.swingState = SwingState.NONE;
            PlayerScript.mode = new PlayerStateMidair(true);
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Swing");
    }
}

/// <summary>
/// ���[���ړ����̃N���X
/// </summary>
public class PlayerStateRail : PlayerState
{
    public PlayerStateRail()
    {
        PlayerScript.refState = EnumPlayerState.RAILING;
        PlayerScript.canShotState = false; //���ĂȂ�
        PlayerScript.vel = Vector3.zero;   //���x0
        PlayerScript.addVel = Vector3.zero;

        BulletScript.rb.velocity = Vector3.zero;
        BulletScript.vel = Vector3.zero;
        BulletScript.StopVelChange = true;
    }

    public override void UpdateState()
    {
        //�L�[���͕s��
    }

    public override void Move()
    {
        //�ړ��Ȃ�
    }

    public override void StateTransition()
    {
        //�I�������X�e�[�g
    }
}
/// <summary>
/// ���S���A�j���[�V�������̐���N���X
/// </summary>
public class PlayerStateDeath : PlayerState
{
    public PlayerStateDeath()
    {
        PlayerScript.refState = EnumPlayerState.DEATH;
        PlayerScript.canShotState = false;
        PlayerScript.vel = Vector3.zero;
        PlayerScript.addVel = Vector3.zero;



        BulletScript.rb.velocity = Vector3.zero;
        BulletScript.vel = Vector3.zero;
        BulletScript.StopVelChange = true;
    }

    public override void UpdateState()
    {
        // �t�F�[�h����
        FadeManager.Instance.SetNextFade(FADE_STATE.FADE_OUT, FADE_KIND.FADE_GAMOVER);
    }

    public override void Move()
    {
        //�ړ��Ȃ�
    }

    public override void StateTransition()
    {
        //��������h�����邱�Ƃ͂Ȃ�
        //�V�[���ύX���ăN�C�b�N���g���C�ʒu�Ƀ��|�b�v
    }

}



///�ߋ��̃X�N���v�g
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