using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStateSwing_Vel : PlayerState
{
    private bool finishFlag;
    private bool releaseButton;
    private bool countreButton;


    private float betweenLength; //開始時二点間の距離(距離はスイングstate通して固定)
    private Vector3 startPlayerVel;　　　　　　 //突入時velocity
    private float startAngle;    //開始時の二点間アングル
    private float endAngle;      //自動切り離しされる角度(start角度依存)
    private float minAnglerVel;  //最低角速度（自動切り離し地点にいる時）
    private float maxAnglerVel;　//最高角速度 (真下にプレイヤーが居る時）
    private float nowAnglerVel;  //現在角速度
    private bool startVelDownFlag; //velocityを減速させるフラグ（真下に到達したら
    private bool LongestLope;    //ロープが最大長になっているかどうか
    private bool onceSwingSound;
    private bool counterAnimFlag;
    private float counterAnimRatio;
    Vector3 LastBtoP_Angle;  //最後に計測したバレット→プレイヤーの正規化Vector
    Vector3 AfterBtoP_Angle; //角速度計算後のバレット→プレイヤーの正規化Vector

    private PlayerMoveDir firstDir; 
    public PlayerStateSwing_Vel()  //コンストラクタ
    { 
        //計算用情報格納
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
    /// 振り子制御用の各種変数を計算
    /// </summary>
    public void CalculateStartVariable_vel()
    {
        //紐の長さとスピードから角速度を計算
        float angler_velocity;
        float tempY = Mathf.Min(startPlayerVel.y, 0.0f);
        angler_velocity = (Mathf.Abs(startPlayerVel.x) * 2.5f + Mathf.Abs(tempY) * 1.5f);
        angler_velocity /= (betweenLength * 2.0f * Mathf.PI);

        //範囲内に補正
        angler_velocity = Mathf.Clamp(angler_velocity, 1.0f, 3.0f);
        nowAnglerVel = maxAnglerVel = minAnglerVel = angler_velocity;
        Debug.Log("AnglerVelocity: " + angler_velocity);

       
        //切り離しアングルの計算
        float diff_down = Mathf.Abs(startAngle - 180.0f); //真下と突入角の差
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            //終点自動切り離しポイントをy軸に対して対称にする
            endAngle -= (diff_down + diff_down);
            //開始点よりは高い位置にする
            endAngle -= 90;

            //範囲内に補正
            endAngle = Mathf.Clamp(endAngle, 90, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //終点自動切り離しポイントをy軸に対して対称にする
            endAngle += (diff_down + diff_down);
            //開始点よりは高い位置にする
            endAngle += 90;

            //範囲内に補正
            endAngle = Mathf.Clamp(endAngle, 220, 270);
        }

        //最低速は突入時プレイヤーvelocity
        maxAnglerVel = minAnglerVel = angler_velocity;
        //最高速度は突入角が大きいほど早い
        float velDiff = Mathf.Clamp01((diff_down / 90));
        maxAnglerVel = minAnglerVel * 2;
    }



    /// <summary>
    /// 振り子制御用の各種変数を計算
    /// </summary>
    public void CalculateStartVariable()
    { 
        //紐の長さとスピードから角速度を計算
        float angler_velocity;
        float tempY = Mathf.Min(startPlayerVel.y, 0.0f);
        angler_velocity = (Mathf.Abs(startPlayerVel.x) * 2.5f + Mathf.Abs(tempY) * 1.5f);
        angler_velocity /= (betweenLength * 2.0f * Mathf.PI);

        //範囲内に補正
        angler_velocity = Mathf.Clamp(angler_velocity, 1.0f, 3.0f);
        nowAnglerVel = maxAnglerVel = minAnglerVel = angler_velocity;
        Debug.Log("AnglerVelocity: " + angler_velocity);

        //バレットからプレイヤーのアングルを保存
        LastBtoP_Angle = AfterBtoP_Angle = (Player.transform.position - BulletScript.rb.position).normalized;

        //切り離しアングルの計算
        float diff_down = Mathf.Abs(startAngle - 180.0f); //真下と突入角の差
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            //終点自動切り離しポイントをy軸に対して対称にする
            endAngle -= (diff_down + diff_down);
            //開始点よりは高い位置にする
            endAngle -= 90;

            //範囲内に補正
            endAngle = Mathf.Clamp(endAngle, 95, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //終点自動切り離しポイントをy軸に対して対称にする
            endAngle += (diff_down + diff_down);
            //開始点よりは高い位置にする
            endAngle += 90;

            //範囲内に補正
            endAngle = Mathf.Clamp(endAngle, 220, 265);
        }

        //最低速は突入時プレイヤーvelocity
        maxAnglerVel = minAnglerVel = angler_velocity;
        //最高速度は突入角が大きいほど早い
        float velDiff = Mathf.Clamp01((diff_down / 90));
        maxAnglerVel = minAnglerVel * 2;
    }

    /// <summary>
    /// 壁跳ね返り時の各種計算
    /// </summary>
    public void CalculateCounterVariable()
    {
        Debug.Log("counter:");

        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            //プレイヤー回転処理
            PlayerScript.dir = PlayerMoveDir.LEFT;
            //PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //プレイヤー回転処理
            PlayerScript.dir = PlayerMoveDir.RIGHT;
            //PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
        }

        //切り離しアングルの計算
        ReleaseAngleCalculate();
    }

    private void ReleaseAngleCalculate()
    {
        float diff_down = Mathf.Abs(endAngle - 180.0f); //真下と終了角の差
        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            //終点自動切り離しポイントをy軸に対して対称にする
            endAngle -= (diff_down + diff_down);
            //開始点よりは高い位置にする
            endAngle -= 10;

            //範囲内に補正
            endAngle = Mathf.Clamp(endAngle, 95, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //終点自動切り離しポイントをy軸に対して対称にする
            endAngle += (diff_down + diff_down);
            //開始点より高い位置にする
            endAngle += 10;

            //範囲内に補正
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

                Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //補正用クオータニオン

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
            if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
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
                //壁ジャンプ用
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

        ////壁ジャンプ用
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
        //与えるベクトル
        Vector3 addVec = BulletScript.rb.position - Player.transform.position;
        addVec = addVec.normalized;
        //ボールプレイヤー間の角度を求める
        float degree = CalculationScript.TwoPointAngle360(BulletScript.rb.position, Player.transform.position);
        float deg180dif = Mathf.Abs(degree - 180);
        float deg45dif = Mathf.Min(Mathf.Abs(degree - 135), Mathf.Abs(degree - 225));

        float deg180Ratio = deg180dif / 90;       //真下と真横の比率
        float deg45Ratio = deg45dif / 45;         //斜め45度との比率


        deg180Ratio = Mathf.Clamp01(deg180Ratio); //角度が９０度以上ある場合でも補正
        deg45Ratio = Mathf.Clamp01(deg45Ratio);
        deg45Ratio = 1 - deg180Ratio;       //斜め45度を1,直角を0とする
        deg180Ratio = 1 - deg180Ratio;            //真下を1,最高到達点を0とする
        float Rvdeg180Ratio = 1 - deg180Ratio;    //真下を0,最高到達点を1とする




        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
        {
            addVec = Quaternion.Euler(0, 0, -90) * addVec;

        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            addVec = Quaternion.Euler(0, 0, 90) * addVec;
        }

        //下に向かっているときは加算するベクトルを少なくする
        if (addVec.y < 0.0f)
        {
            float AddVecRatio = Easing.EasingTypeFloat(EASING_TYPE.CUBIC_IN, deg180Ratio, 1.0f, 0.0f, 1.0f);
            float JumpRatio = Easing.EasingTypeFloat(EASING_TYPE.CUBIC_IN, deg180Ratio, 1.0f, 0.0f, 1.0f);
            float BoostRatio = Easing.EasingTypeFloat(EASING_TYPE.CUBIC_IN, deg180Ratio, 1.0f, 0.0f, 1.0f);

            float VecForce = PlayerScript.vel.magnitude;
            float AddJumpVec = (JumpRatio * 0.0f) + 50.0f;
            float AddBoostVec = (BoostRatio * 10.0f) + 0.0f;  //スピードアップのためのx方向のベクトル

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

            float VecForce = PlayerScript.vel.magnitude;   //円の半径に直行するベクトル量
            float AddJumpVec = (JumpRatio * 15.0f) + 40.0f;   //下にいかないように加算するy方向のベクトル量
            float AddBoostVec = (BoostRatio * 30.0f) + 10.0f;  //スピードアップのためのx方向のベクトル

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
        //切り離し
        if (PlayerScript.swingState == SwingState.TOUCHED)
        {
            //切り離し入力
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
        float degree = CalculationScript.TwoPointAngle360(BulletScript.rb.position, Player.transform.position) ; //バレットからプレイヤーベクトル
        float deg180dif = Mathf.Abs(degree - 180); //プレイヤーからベクトル

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
                //振り子時反転処理
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

                //壁跳ね返り処理
                if (PlayerScript.conuterSwing)
                {           
                    SoundManager.Instance.PlaySound("sound_19_2_Counter", 0.5f);
                    PlayerScript.animator.SetTrigger(PlayerScript.animHash.wallKick);
                    CalculateCounterVariable();
                    PlayerScript.conuterSwing = false;
                }

                //速度計算
                float deg180Ratio = deg180dif / 90;       //真下と真横の比率
                deg180Ratio = Mathf.Clamp01(deg180Ratio); //角度が９０度以上ある場合でも補正
                deg180Ratio = 1 - deg180Ratio; //真下を1,最高到達点を0とする

                float easeRatio = Easing.EasingTypeFloat(EASING_TYPE.SINE_OUT, deg180Ratio, 1.0f, 0.0f, 1.0f);

            
                //前回計算後のAfterAngleを持ってくる
                LastBtoP_Angle = AfterBtoP_Angle;
                

                //向きによって回転方向が違う

                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    Vector3 tempVec = BulletScript.rb.position - PlayerScript.rb.position;
                    tempVec = tempVec.normalized;
                    tempVec = Quaternion.Euler(0, 0, -90) * tempVec;

                    //振り子の最大長になっていない場合には円外に向かうようベクトルの角度を変える
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

                    //振り子の最大長になっていない場合には円外に向かうようベクトルの角度を変える
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

                //地面滑り処理
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


                //振り子の最大長になっていない場合には円外に向かうベクトルを与える
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


                //振り子よりも長くなってしまった場合は補正
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