using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStateSwing_Vel : PlayerState
{
    private bool finishFlag;
    private bool releaseButton;
    private bool countreButton;


    private float betweenLength; //�J�n����_�Ԃ̋���(�����̓X�C���Ostate�ʂ��ČŒ�)
    private Vector3 startPlayerVel;�@�@�@�@�@�@ //�˓���velocity
    private float startAngle;    //�J�n���̓�_�ԃA���O��
    private float endAngle;      //�����؂藣�������p�x(start�p�x�ˑ�)
    private float minAnglerVel;  //�Œ�p���x�i�����؂藣���n�_�ɂ��鎞�j
    private float maxAnglerVel;�@//�ō��p���x (�^���Ƀv���C���[�����鎞�j
    private float nowAnglerVel;  //���݊p���x
    private bool startVelDownFlag; //velocity������������t���O�i�^���ɓ��B������
    private bool LongestLope;    //���[�v���ő咷�ɂȂ��Ă��邩�ǂ���
    private bool onceSwingSound;
    private bool counterAnimFlag;
    private float counterAnimRatio;
    Vector3 LastBtoP_Angle;  //�Ō�Ɍv�������o���b�g���v���C���[�̐��K��Vector
    Vector3 AfterBtoP_Angle; //�p���x�v�Z��̃o���b�g���v���C���[�̐��K��Vector

    private PlayerMoveDir firstDir; 
    public PlayerStateSwing_Vel()  //�R���X�g���N�^
    { 
        //�v�Z�p���i�[
        startPlayerVel = BulletScript.vel;
        firstDir = PlayerScript.dir;
        startVelDownFlag = false;

        PlayerScript.refState = EnumPlayerState.SWING;
        PlayerScript.swingState = SwingState.TOUCHED;
        PlayerScript.canShotState = false;
        PlayerScript.endSwing = false;
        PlayerScript.conuterSwing = false;
        finishFlag = false;
        releaseButton = false;
        countreButton = false;
        onceSwingSound = false;
        counterAnimFlag = false;
        counterAnimRatio = 0.0f;
        BulletScript.SetBulletState(EnumBulletState.STOP);

        PlayerScript.useVelocity = true;


        PlayerScript.ResetAnimation();
        PlayerScript.animator.SetBool(PlayerScript.animHash.isSwing, true);
        PlayerScript.animator.SetFloat(PlayerScript.animHash.KickFloat, 0.0f);
        CalculateStartVariable();

        PlayerScript.counterTimer = 1.0f;

    }


    /// <summary>
    /// �U��q����p�̊e��ϐ����v�Z
    /// </summary>
    public void CalculateStartVariable_vel()
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
        maxAnglerVel = minAnglerVel * 2;
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
        LastBtoP_Angle = AfterBtoP_Angle = (Player.transform.position - BulletScript.rb.position).normalized;

        //�؂藣���A���O���̌v�Z
        float diff_down = Mathf.Abs(startAngle - 180.0f); //�^���Ɠ˓��p�̍�
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
            endAngle -= (diff_down + diff_down);
            //�J�n�_���͍����ʒu�ɂ���
            endAngle -= 90;

            //�͈͓��ɕ␳
            endAngle = Mathf.Clamp(endAngle, 95, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
            endAngle += (diff_down + diff_down);
            //�J�n�_���͍����ʒu�ɂ���
            endAngle += 90;

            //�͈͓��ɕ␳
            endAngle = Mathf.Clamp(endAngle, 220, 265);
        }

        //�Œᑬ�͓˓����v���C���[velocity
        maxAnglerVel = minAnglerVel = angler_velocity;
        //�ō����x�͓˓��p���傫���قǑ���
        float velDiff = Mathf.Clamp01((diff_down / 90));
        maxAnglerVel = minAnglerVel * 2;
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
            //PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //�v���C���[��]����
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            //PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
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
            endAngle = Mathf.Clamp(endAngle, 95, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //�I�_�����؂藣���|�C���g��y���ɑ΂��đΏ̂ɂ���
            endAngle += (diff_down + diff_down);
            //�J�n�_��荂���ʒu�ɂ���
            endAngle += 10;

            //�͈͓��ɕ␳
            endAngle = Mathf.Clamp(endAngle, 220, 265);
        }

        //if (PlayerScript.AutoRelease == false)
        //{
        //    minAnglerVel = 0.4f;
        //}
    }

    public void RotationPlayer()
    {

        switch (PlayerScript.swingState)
        {
            case SwingState.TOUCHED:
                float degree = CalculationScript.TwoPointAngle360(BulletScript.rb.position, Player.transform.position);
#if false
                Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;
                Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);
#else
                Quaternion quaternion = Quaternion.LookRotation(ChainManager.instance.PlayerAngle);

                Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //�␳�p�N�I�[�^�j�I��

                quaternion *= adjustQua;

                if (firstDir == PlayerMoveDir.RIGHT)
                {
                    if (degree < 180)
                    {
                        quaternion *= Quaternion.Euler(0, 180, 0);
                    }
                }
                else if (firstDir == PlayerMoveDir.LEFT)
                {
                    if (degree > 180)
                    {
                        quaternion *= Quaternion.Euler(0, 180, 0);
                    }
                }
#endif
                PlayerScript.rb.rotation = quaternion;
                break;
        }


    }

    public void InputButton()
    {
        if (PlayerScript.swingState == SwingState.TOUCHED)
        {
            if (Input.GetButton("Button_R") == false) //�{�^��������Ă�����
            {
                releaseButton = true;
            }
        }
    }

    public void AnimFrameSetting()
    {
        const float RayLength = 10.0f;


        float degree = CalculationScript.TwoPointAngle360(BulletScript.rb.position, Player.transform.position);
        float animFrame = 0.0f;

       

        if (firstDir == PlayerMoveDir.RIGHT)
        {
            animFrame = (degree  - 95) / 170;
            animFrame = Mathf.Clamp01(animFrame);
            animFrame = 1 - animFrame;
   
            Ray ray = new Ray(PlayerScript.rb.position, Vector3.right);
            Debug.DrawRay(ray.origin, ray.direction * RayLength, Color.magenta, 0, true);
            if (Physics.Raycast(PlayerScript.rb.position, Vector3.right, RayLength, LayerMask.GetMask("Platform")))
            {
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    if (degree < 240)
                    {
                        counterAnimFlag = true;
                    }
                    else
                    {
                        counterAnimFlag = false;
                    }

                }
                else
                {
                    if (degree < 120)
                    {
                        counterAnimFlag = true;
                    }
                    else
                    {
                        counterAnimFlag = false;
                    }
                }
            }        
        }
        else if(firstDir == PlayerMoveDir.LEFT)
        {
            animFrame = (degree - 95) / 170;
            animFrame = Mathf.Clamp01(animFrame);

            Ray ray = new Ray(PlayerScript.rb.position, Vector3.right);
            Debug.DrawRay(ray.origin, ray.direction * RayLength, Color.magenta, 0, true);
            if (Physics.Raycast(PlayerScript.rb.position, Vector3.right, RayLength, LayerMask.GetMask("Platform")))
            {
                //�ǃW�����v�p
                if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    if (degree > 120)
                    {
                        counterAnimFlag = true;
                    }
                    else
                    {
                        counterAnimFlag = false;
                    }

                }
                else
                {
                    if (degree > 160)
                    {
                        counterAnimFlag = true;
                    }
                    else
                    {
                        counterAnimFlag = false;
                    }
                }
            }
        }


        if (counterAnimFlag)
        {
            if (firstDir == PlayerScript.dir)
            {
                counterAnimRatio = Mathf.Min(counterAnimRatio + 0.05f, 1.0f);      
                PlayerScript.animator.SetFloat(PlayerScript.animHash.KickFloat, counterAnimRatio);
            }
        }
        else
        {
            counterAnimRatio = Mathf.Max(counterAnimRatio - 0.05f, 0.0f);
            PlayerScript.animator.SetFloat(PlayerScript.animHash.KickFloat, counterAnimRatio);
        }

        ////�ǃW�����v�p
        //if (counterAnimFlag)
        //{
        //    if (firstDir == PlayerScript.dir)
        //    {
        //        counterAnimRatio = Mathf.Min(counterAnimRatio + 0.05f, 1.0f);
        //        PlayerScript.animator.SetFloat(PlayerScript.animHash.KickFloat, counterAnimRatio);
        //    }
        //}
        //else
        //{
        //    counterAnimRatio = Mathf.Max(counterAnimRatio - 0.05f, 0.0f);
        //    PlayerScript.animator.SetFloat(PlayerScript.animHash.KickFloat, counterAnimRatio);
        //}

        Debug.Log("KickRatio" + counterAnimRatio);

        if (firstDir == PlayerScript.dir)
        {
            PlayerScript.animator.Play("Swing.swingGo_Kick", -1, animFrame);
        }
        else
        {
            animFrame = 1 - animFrame;
            PlayerScript.animator.Play("Swing.swingBack_Kick", -1, animFrame);
        }

       
    }

    public Vector3 ReleaseForceCalicurale()
    {
        //�^����x�N�g��
        Vector3 addVec = BulletScript.rb.position - Player.transform.position;
        addVec = addVec.normalized;
        //�{�[���v���C���[�Ԃ̊p�x�����߂�
        float degree = CalculationScript.TwoPointAngle360(BulletScript.rb.position, Player.transform.position);
        float deg180dif = Mathf.Abs(degree - 180);
        float deg45dif = Mathf.Min(Mathf.Abs(degree - 135), Mathf.Abs(degree - 225));

        float deg180Ratio = deg180dif / 90;       //�^���Ɛ^���̔䗦
        float deg45Ratio = deg45dif / 45;         //�΂�45�x�Ƃ̔䗦


        deg180Ratio = Mathf.Clamp01(deg180Ratio); //�p�x���X�O�x�ȏ゠��ꍇ�ł��␳
        deg45Ratio = Mathf.Clamp01(deg45Ratio);
        deg45Ratio = 1 - deg180Ratio;       //�΂�45�x��1,���p��0�Ƃ���
        deg180Ratio = 1 - deg180Ratio;            //�^����1,�ō����B�_��0�Ƃ���
        float Rvdeg180Ratio = 1 - deg180Ratio;    //�^����0,�ō����B�_��1�Ƃ���




        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            addVec = Quaternion.Euler(0, 0, -90) * addVec;

        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            addVec = Quaternion.Euler(0, 0, 90) * addVec;
        }

        //���Ɍ������Ă���Ƃ��͉��Z����x�N�g�������Ȃ�����
        if (addVec.y < 0.0f)
        {
            float AddVecRatio = Easing.EasingTypeFloat(EASING_TYPE.CUBIC_IN, deg180Ratio, 1.0f, 0.0f, 1.0f);
            float JumpRatio = Easing.EasingTypeFloat(EASING_TYPE.CUBIC_IN, deg180Ratio, 1.0f, 0.0f, 1.0f);
            float BoostRatio = Easing.EasingTypeFloat(EASING_TYPE.CUBIC_IN, deg180Ratio, 1.0f, 0.0f, 1.0f);

            float VecForce = PlayerScript.vel.magnitude;
            float AddJumpVec = (JumpRatio * 0.0f) + 50.0f;
            float AddBoostVec = (BoostRatio * 10.0f) + 0.0f;  //�X�s�[�h�A�b�v�̂��߂�x�����̃x�N�g��

            addVec.y *= 0.0f;
            addVec *= VecForce;
            addVec.y += AddJumpVec;
            addVec.x += AddBoostVec * Mathf.Sign(PlayerScript.vel.x);
        }
        else
        {
            float AddVecRatio = Easing.EasingTypeFloat(EASING_TYPE.CUBIC_IN, deg180Ratio, 1.0f, 0.0f, 1.0f);
            float JumpRatio = Easing.EasingTypeFloat(EASING_TYPE.CUBIC_IN, Rvdeg180Ratio, 1.0f, 0.0f, 1.0f);
            float BoostRatio = Easing.EasingTypeFloat(EASING_TYPE.CUBIC_IN, Rvdeg180Ratio, 1.0f, 0.0f, 1.0f);
            //Debug.Log("easeRatio : " + AddVecRatio);

            float VecForce = PlayerScript.vel.magnitude;   //�~�̔��a�ɒ��s����x�N�g����
            float AddJumpVec = (JumpRatio * 15.0f) + 40.0f;   //���ɂ����Ȃ��悤�ɉ��Z����y�����̃x�N�g����
            float AddBoostVec = (BoostRatio * 30.0f) + 10.0f;  //�X�s�[�h�A�b�v�̂��߂�x�����̃x�N�g��

            addVec.y *= 0.3f;
            addVec *= VecForce;
            addVec.y += AddJumpVec;

            addVec.x += AddBoostVec * Mathf.Sign(PlayerScript.vel.x);
        }

        return addVec;

    }

    private void soundSwing()
    {
        if (onceSwingSound == false)
        {
            float degree = CalculationScript.TwoPointAngle360(BulletScript.rb.position, Player.transform.position);
            if (PlayerScript.dir == PlayerMoveDir.RIGHT)
            {
                if(degree > 250)
                {
                    onceSwingSound = true;
                    SoundManager.Instance.PlaySound("sound_15_Swing", 1.0f);
                }
            }
            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
            {
                if (degree < 110)
                {
                    onceSwingSound = true;
                    SoundManager.Instance.PlaySound("sound_15_Swing", 1.0f);
                }
            }
        }
        else
        {

        }
    }

    public override void UpdateState()
    {
        //�؂藣��
        if (PlayerScript.swingState == SwingState.TOUCHED)
        {
            //�؂藣������
            InputButton();

            AnimFrameSetting();
            soundSwing();
            if (PlayerScript.endSwing)
            {
                finishFlag = true;
                if (PlayerScript.forciblySwingSaveVelocity)
                {
                    PlayerScript.vel = ReleaseForceCalicurale();
                }
                else
                {
                    PlayerScript.vel = Vector3.zero;
                }
            }

            if (releaseButton == true)
            {
                if (PlayerScript.forciblySwingNextFollow)
                {
                    PlayerScript.vel = (BulletScript.rb.position - PlayerScript.rb.position ).normalized * 50.0f;
                }
                else
                {
                    PlayerScript.vel = ReleaseForceCalicurale();
                }

                PlayerScript.useVelocity = true;
                finishFlag = true;
            }
        }
    }

    public override void Move()
    {
        float degree = CalculationScript.TwoPointAngle360(BulletScript.rb.position, Player.transform.position) ; //�o���b�g����v���C���[�x�N�g��
        float deg180dif = Mathf.Abs(degree - 180); //�v���C���[����x�N�g��

        if(startVelDownFlag == false)
        {
            if(PlayerScript.dir == PlayerMoveDir.RIGHT)
            {
                if (degree < 180)
                {
                    startVelDownFlag = true;
                }
            }
            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
            {
                if (degree > 180)
                {
                    startVelDownFlag = true;
                }
            }
        }

        RotationPlayer();

        switch (PlayerScript.swingState)
        {
            case SwingState.TOUCHED:
                //�U��q�����]����
                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    if (endAngle > degree)
                    {
                        CalculateCounterVariable();
                        onceSwingSound = false;
                        counterAnimFlag = false;
                       
                    }
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    if (endAngle < degree)
                    {
                        CalculateCounterVariable();
                        onceSwingSound = false;
                        counterAnimFlag = false;
                    }
                }

                //�ǒ��˕Ԃ菈��
                if (PlayerScript.conuterSwing)
                {           
                    SoundManager.Instance.PlaySound("sound_19_2_Counter", 0.5f);
                    PlayerScript.animator.SetTrigger(PlayerScript.animHash.wallKick);
                    CalculateCounterVariable();
                    PlayerScript.conuterSwing = false;
                }

                //���x�v�Z
                float deg180Ratio = deg180dif / 90;       //�^���Ɛ^���̔䗦
                deg180Ratio = Mathf.Clamp01(deg180Ratio); //�p�x���X�O�x�ȏ゠��ꍇ�ł��␳
                deg180Ratio = 1 - deg180Ratio; //�^����1,�ō����B�_��0�Ƃ���

                float easeRatio = Easing.EasingTypeFloat(EASING_TYPE.SINE_OUT, deg180Ratio, 1.0f, 0.0f, 1.0f);

            
                //�O��v�Z���AfterAngle�������Ă���
                LastBtoP_Angle = AfterBtoP_Angle;
                

                //�����ɂ���ĉ�]�������Ⴄ

                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    Vector3 tempVec = BulletScript.rb.position - PlayerScript.rb.position;
                    tempVec = tempVec.normalized;
                    tempVec = Quaternion.Euler(0, 0, -90) * tempVec;

                    //�U��q�̍ő咷�ɂȂ��Ă��Ȃ��ꍇ�ɂ͉~�O�Ɍ������悤�x�N�g���̊p�x��ς���
                    if (LongestLope == false)
                    {
                        tempVec = Quaternion.Euler(0, 0, -30) * tempVec;
                    }

                    if (startVelDownFlag)
                    {
                        PlayerScript.vel = tempVec * easeRatio * 65.0f + (tempVec * 5.0f);
                    }
                    else
                    {
# if true
                        PlayerScript.vel = tempVec * easeRatio * 15.0f + (tempVec * 40.0f);
#else

#endif
                    }
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    Vector3 tempVec = BulletScript.rb.position - PlayerScript.rb.position;
                    tempVec = tempVec.normalized;
                    tempVec = Quaternion.Euler(0, 0, 90) * tempVec;

                    //�U��q�̍ő咷�ɂȂ��Ă��Ȃ��ꍇ�ɂ͉~�O�Ɍ������悤�x�N�g���̊p�x��ς���
                    if (LongestLope == false)
                    {
                        tempVec = Quaternion.Euler(0, 0, 30) * tempVec;

                    }
                    
                    if (startVelDownFlag)
                    {
                        PlayerScript.vel = tempVec * easeRatio * 65.0f + (tempVec * 5.0f);
                    }
                    else
                    {
                        PlayerScript.vel = tempVec * 40.0f;
                    }
                }

                //�n�ʊ��菈��
                if (PlayerScript.SlideSwing)
                {
                    Vector3 lateralVel = Vector3.zero;

                    if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                    {
                        lateralVel.x = PlayerScript.vel.magnitude * 0.8f;
                    }
                    else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                    {
                        lateralVel.x = PlayerScript.vel.magnitude * 0.8f * -1;
                    }

                    PlayerScript.vel = lateralVel;
                    PlayerScript.SlideSwing = false;
                }

                // if (PlayerScript.floorVel.magnitude < 0.01f)
                //{
                //    if(LongestLope == false)
                //    {
                //        Vector3 longerVec = PlayerScript.rb.position - BulletScript.rb.position;
                //        longerVec = longerVec.normalized;

                //        float outForce = 20.0f;
                //        longerVec *= outForce;

                //        PlayerScript.vel += longerVec;
                //    }
                //}


                //�U��q�̍ő咷�ɂȂ��Ă��Ȃ��ꍇ�ɂ͉~�O�Ɍ������x�N�g����^����
                //if (LongestLope == false)
                //{
                //    if (PlayerScript.SlideSwing == false)
                //    {
                //        if (PlayerScript.floorVel.magnitude < 0.01f)
                //        {
                //            Vector3 longerVec = PlayerScript.rb.position - BulletScript.rb.position;
                //            longerVec = longerVec.normalized;

                //            float outForce = 20.0f;
                //            longerVec *= outForce;

                //            PlayerScript.vel += longerVec;
                //        }
                //    }
                //}


                //�U��q���������Ȃ��Ă��܂����ꍇ�͕␳
                if (Vector3.Distance(PlayerScript.rb.position, BulletScript.rb.position) > BulletScript.BULLET_ROPE_LENGTH)
                {
                    LongestLope = true;
                    float ang = CalculationScript.UnityTwoPointAngle360(BulletScript.rb.position, PlayerScript.rb.position);
                    Vector3 adjustPos = BulletScript.rb.position;
                    adjustPos.x += Mathf.Cos(ang * Mathf.Deg2Rad) * BulletScript.BULLET_ROPE_LENGTH;
                    adjustPos.y += Mathf.Sin(ang * Mathf.Deg2Rad) * BulletScript.BULLET_ROPE_LENGTH;
                    PlayerScript.rb.position = adjustPos;
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
            PlayerScript.animator.SetBool(PlayerScript.animHash.isSwing, false);
            PlayerScript.swingState = SwingState.NONE;
            BulletScript.SetBulletState(EnumBulletState.RETURN);
            
            if (PlayerScript.forciblySwingNextFollow)
            {
                PlayerScript.mode = new PlayerStateShot(true);
            }
            else
            {
                PlayerScript.mode = new PlayerStateMidair(true, MidairState.NORMAL);
            }
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Swing");
    }
}