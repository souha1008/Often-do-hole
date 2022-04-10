using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e���V��ŃX�C���O���Ă�����(��������)
/// �ړ����x����ۑ�
/// </summary>
public class PlayerStateSwing : PlayerState
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

    public PlayerStateSwing()  //�R���X�g���N�^
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

    ~PlayerStateSwing()
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
            endAngle -= 90;

            //�͈͓��ɕ␳
            endAngle = Mathf.Clamp(endAngle, 90, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
            endAngle += (diff_down + diff_down);
            //�J�n�_���͍����ʒu�ɂ���
            endAngle += 90;

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

        if(PlayerScript.AutoRelease == false)
        {
            minAnglerVel = 0.4f;
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

                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    if (degree < 180)
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

        if (PlayerScript.ReleaseMode)
        {
            if (PlayerScript.swingState == SwingState.TOUCHED)
            {
                if (Input.GetButtonUp("Button_R")) //�{�^���������Ęb������
                {
                    releaseButton = true;
                }
            }
            else if (PlayerScript.swingState == SwingState.HANGING)
            {
                if (Input.GetButtonUp("Button_R")) //�{�^���������Ęb������
                {
                    releaseButton = true;
                }
                else if (Input.GetButtonDown("Jump"))
                {
                    countreButton = true;
                    Debug.Log("Press Jump");
                }
            }
        }
        else
        {
            if (PlayerScript.swingState == SwingState.TOUCHED)
            {
                if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
                {
                    releaseButton = true;
                }
            }
            else if (PlayerScript.swingState == SwingState.HANGING)
            {
                if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
                {
                    releaseButton = true;
                }
                else if (Input.GetButtonDown("Jump"))
                {
                    countreButton = true;
                    Debug.Log("Press Jump");
                }
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
                if (releaseButton == true)
                {
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;

                    //�����ǉ�
                    //�e�ƃv���C���[�Ԃ̃x�N�g���ɒ��s����x�N�g��
                    Vector3 addVec = BulletPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, -90) * addVec;


                    //float mutipleVec = (nowAnglerVel / 20.0f) + 1.0f;
                    //PlayerScript.vel += addVec * mutipleVec * 40.0f * SWING_END_RATIO;

                    PlayerScript.vel += addVec * PlayerScript.RELEASE_FORCE;
                }
            }
            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
            {
                if (releaseButton == true)
                {
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;

                    //�����ǉ�
                    //�e�ƃv���C���[�Ԃ̃x�N�g���ɒ��s����x�N�g��
                    Vector3 addVec = BulletPosition - Player.transform.position;
                    addVec = addVec.normalized;
                    addVec = Quaternion.Euler(0, 0, 90) * addVec;


                    //float mutipleVec = (nowAnglerVel / 20.0f) + 1.0f;
                    //PlayerScript.vel += addVec * mutipleVec * 40.0f * SWING_END_RATIO;
                    PlayerScript.vel += addVec * PlayerScript.RELEASE_FORCE;
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
                //�U��q�����]����
                if(PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    if(endAngle > degree)
                    {
                        CalculateCounterVariable();
                    }
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    if(endAngle < degree)
                    {
                        CalculateCounterVariable();
                    }
                }

                //�ǒ��˕Ԃ菈��
                if (PlayerScript.hangingSwing)
                {
                    CalculateCounterVariable();
                    PlayerScript.hangingSwing = false;
                }

                //�������鏈��
                if (PlayerScript.LongRope)
                {
                    if (betweenLength < BulletScript.BULLET_ROPE_LENGTH)
                    {
                        betweenLength += 0.2f;
                        minAnglerVel *= 0.99f;
                        maxAnglerVel *= 0.99f;
                    }
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
/// �e���V��ŃX�C���O���Ă�����
/// �ړ����x����ۑ�
/// </summary>
public class PlayerStateSwing_2 : PlayerState
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

    public PlayerStateSwing_2()  //�R���X�g���N�^
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

                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    if (degree < 180)
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

        if (PlayerScript.ReleaseMode)
        {
            if (PlayerScript.swingState == SwingState.TOUCHED)
            {
                if (Input.GetButtonUp("Button_R")) //�{�^���������Ęb������
                {
                    releaseButton = true;
                }
            }
            else if (PlayerScript.swingState == SwingState.HANGING)
            {
                if (Input.GetButtonUp("Button_R")) //�{�^���������Ęb������
                {
                    releaseButton = true;
                }
                else if (Input.GetButtonDown("Jump"))
                {
                    countreButton = true;
                    Debug.Log("Press Jump");
                }
            }
        }
        else
        {
            if (PlayerScript.swingState == SwingState.TOUCHED)
            {
                if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
                {
                    releaseButton = true;
                }
            }
            else if (PlayerScript.swingState == SwingState.HANGING)
            {
                if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
                {
                    releaseButton = true;
                }
                else if (Input.GetButtonDown("Jump"))
                {
                    countreButton = true;
                    Debug.Log("Press Jump");
                }
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

                    PlayerScript.vel += addVec * PlayerScript.RELEASE_FORCE;
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

                    PlayerScript.vel += addVec * PlayerScript.RELEASE_FORCE;
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
