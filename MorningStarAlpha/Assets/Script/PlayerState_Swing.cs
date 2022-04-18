using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾が天井でスイングしている状態(往復あり)
/// 移動速度等を保存
/// </summary>
//public class PlayerStateSwing : PlayerState
//{
//    private bool finishFlag;
//    private bool releaseButton;
//    private bool countreButton;
//    private Vector3 BulletPosition; //ボールの位置

//    private float betweenLength; //開始時二点間の距離(距離はスイングstate通して固定)
//    private Vector3 startPlayerVel;　　　　　　 //突入時velocity
//    private float startAngle;    //開始時の二点間アングル
//    private float endAngle;      //自動切り離しされる角度(start角度依存)
//    private float minAnglerVel;  //最低角速度（自動切り離し地点にいる時）
//    private float maxAnglerVel;　//最高角速度 (真下にプレイヤーが居る時）
//    private float nowAnglerVel;  //現在角速度

//    Vector3 LastBtoP_Angle;  //最後に計測したバレット→プレイヤーの正規化Vector
//    Vector3 AfterBtoP_Angle; //角速度計算後のバレット→プレイヤーの正規化Vector


//    const float SWING_END_RATIO = 1.4f;

//    public PlayerStateSwing()  //コンストラクタ
//    {
//        BulletPosition = BulletScript.gameObject.transform.position;

//        //計算用情報格納
//        startPlayerVel = BulletScript.vel;
//        betweenLength = Vector3.Distance(Player.transform.position, BulletPosition);
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);
//        startAngle = endAngle = degree;

//        PlayerScript.refState = EnumPlayerState.SWING;
//        PlayerScript.swingState = SwingState.TOUCHED;
//        PlayerScript.canShotState = false;
//        PlayerScript.endSwing = false;
//        PlayerScript.hangingSwing = false;
//        finishFlag = false;
//        releaseButton = false;
//        countreButton = false;
//        BulletScript.rb.isKinematic = true;
//        PlayerScript.rb.velocity = Vector3.zero;
//        PlayerScript.vel = Vector3.zero;

//        CalculateStartVariable();

        
//    }

//    ~PlayerStateSwing()
//    {

//    }

//    /// <summary>
//    /// 振り子制御用の各種変数を計算
//    /// </summary>
//    public void CalculateStartVariable()
//    {

//        //紐の長さとスピードから角速度を計算
//        float angler_velocity;
//        float tempY = Mathf.Min(startPlayerVel.y, 0.0f);
//        angler_velocity = (Mathf.Abs(startPlayerVel.x) * 2.5f + Mathf.Abs(tempY) * 1.5f);
//        angler_velocity /= (betweenLength * 2.0f * Mathf.PI);

//        //範囲内に補正
//        angler_velocity = Mathf.Clamp(angler_velocity, 1.0f, 3.0f);

//        nowAnglerVel = maxAnglerVel = minAnglerVel = angler_velocity;

//        Debug.Log("AnglerVelocity: " + angler_velocity);

//        //バレットからプレイヤーのアングルを保存
//        LastBtoP_Angle = AfterBtoP_Angle = (Player.transform.position - BulletPosition).normalized;

//        //切り離しアングルの計算
//        float diff_down = Mathf.Abs(startAngle - 180.0f); //真下と突入角の差
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle -= (diff_down + diff_down);
//            //開始点よりは高い位置にする
//            endAngle -= 90;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 90, 140);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle += (diff_down + diff_down);
//            //開始点よりは高い位置にする
//            endAngle += 90;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 220, 270);
//        }

//        //最低速は突入時プレイヤーvelocity
//        maxAnglerVel = minAnglerVel = angler_velocity;
//        //最高速度は突入角が大きいほど早い
//        float velDiff = Mathf.Clamp01((diff_down / 90));
//        maxAnglerVel =  minAnglerVel *2;
//    }

//    /// <summary>
//    /// 壁跳ね返り時の各種計算
//    /// </summary>
//    public void CalculateCounterVariable()
//    {
//        Debug.Log("counter:");

//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //プレイヤー回転処理
//            PlayerScript.dir = PlayerMoveDir.LEFT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //プレイヤー回転処理
//            PlayerScript.dir = PlayerMoveDir.RIGHT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
//        }

//        //切り離しアングルの計算
//        ReleaseAngleCalculate();
//    }

//    private void ReleaseAngleCalculate()
//    {
//        float diff_down = Mathf.Abs(endAngle - 180.0f); //真下と終了角の差
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle -= (diff_down + diff_down);
//            //開始点よりは高い位置にする
//            endAngle -= 10;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 90, 140);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle += (diff_down + diff_down);
//            //開始点より高い位置にする
//            endAngle += 10;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 220, 270);
//        }

//        if(PlayerScript.AutoRelease == false)
//        {
//            minAnglerVel = 0.4f;
//        }      
//    }

//    public void RotationPlayer()
//    {

//        switch (PlayerScript.swingState)
//        {
//            case SwingState.TOUCHED:
//                float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);

//                Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;
//                Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);

//                Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //補正用クオータニオン

//                quaternion *= adjustQua;

//                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//                {
//                    if (degree < 180)
//                    {
//                        quaternion *= Quaternion.Euler(0, 180, 0);
//                    }
//                }
//                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//                {
//                    if (degree > 180)
//                    {
//                        quaternion *= Quaternion.Euler(0, 180, 0);
//                    }
//                }

//                PlayerScript.rb.rotation = quaternion;
//                break;

//            case SwingState.RELEASED:
//                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//                {
//                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 90, 0));
//                }
//                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//                {
//                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, -90, 0));
//                }
//                break;
//        }


//    }

//    public void InputButton()
//    {

//        if (PlayerScript.ReleaseMode)
//        {
//            if (PlayerScript.swingState == SwingState.TOUCHED)
//            {
//                if (Input.GetButtonUp("Button_R")) //ボタンを押して話したら
//                {
//                    releaseButton = true;
//                }
//            }
//            else if (PlayerScript.swingState == SwingState.HANGING)
//            {
//                if (Input.GetButtonUp("Button_R")) //ボタンを押して話したら
//                {
//                    releaseButton = true;
//                }
//                else if (Input.GetButtonDown("Jump"))
//                {
//                    countreButton = true;
//                    Debug.Log("Press Jump");
//                }
//            }
//        }
//        else
//        {
//            if (PlayerScript.swingState == SwingState.TOUCHED)
//            {
//                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
//                {
//                    releaseButton = true;
//                }
//            }
//            else if (PlayerScript.swingState == SwingState.HANGING)
//            {
//                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
//                {
//                    releaseButton = true;
//                }
//                else if (Input.GetButtonDown("Jump"))
//                {
//                    countreButton = true;
//                    Debug.Log("Press Jump");
//                }
//            }

//        }

//    }





//    public override void UpdateState()
//    {
//        //切り離し入力
//        InputButton();

//        //弾の場所更新
//        BulletPosition = BulletScript.rb.position;

//        //ボールプレイヤー間の角度を求める
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);

//        //切り離し
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
//                if (releaseButton == true)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    PlayerScript.swingState = SwingState.RELEASED;

//                    //勢い追加
//                    //弾とプレイヤー間のベクトルに直行するベクトル
//                    Vector3 addVec = BulletPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, -90) * addVec;


//                    //float mutipleVec = (nowAnglerVel / 20.0f) + 1.0f;
//                    //PlayerScript.vel += addVec * mutipleVec * 40.0f * SWING_END_RATIO;

//                    PlayerScript.vel += addVec * PlayerScript.RELEASE_FORCE;
//                }
//            }
//            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//            {
//                if (releaseButton == true)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    PlayerScript.swingState = SwingState.RELEASED;

//                    //勢い追加
//                    //弾とプレイヤー間のベクトルに直行するベクトル
//                    Vector3 addVec = BulletPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, 90) * addVec;


//                    //float mutipleVec = (nowAnglerVel / 20.0f) + 1.0f;
//                    //PlayerScript.vel += addVec * mutipleVec * 40.0f * SWING_END_RATIO;
//                    PlayerScript.vel += addVec * PlayerScript.RELEASE_FORCE;
//                }
//            }
//        }
//    }

//    public override void Move()
//    {
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position); //バレットからプレイヤーベクトル
//        float deg180dif = Mathf.Abs(degree - 180); //プレイヤーからベクトル

//        RotationPlayer();

//        switch (PlayerScript.swingState)
//        {
//            case SwingState.TOUCHED:
//                //振り子時反転処理
//                if(PlayerScript.dir == PlayerMoveDir.RIGHT)
//                {
//                    if(endAngle > degree)
//                    {
//                        CalculateCounterVariable();
//                    }
//                }
//                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//                {
//                    if(endAngle < degree)
//                    {
//                        CalculateCounterVariable();
//                    }
//                }

//                //壁跳ね返り処理
//                if (PlayerScript.hangingSwing)
//                {
//                    CalculateCounterVariable();
//                    PlayerScript.hangingSwing = false;
//                }

//                //長くする処理
//                if (PlayerScript.LongRope)
//                {
//                    if (betweenLength < BulletScript.BULLET_ROPE_LENGTH)
//                    {
//                        betweenLength += 0.2f;
//                        minAnglerVel *= 0.99f;
//                        maxAnglerVel *= 0.99f;
//                    }
//                }

//                ////短くする処理
//                //if (PlayerScript.shortSwing.isShort)
//                //{
//                //    betweenLength = PlayerScript.shortSwing.length;
//                //    PlayerScript.shortSwing.isShort = false;
//                //}

//                //角速度計算
//                float deg180Ratio = deg180dif / Mathf.Abs(endAngle - 180); //真下と最高到達点の比率
//                deg180Ratio = Mathf.Clamp01(deg180Ratio); //一応範囲内に補正
//                deg180Ratio = 1 - deg180Ratio; //真下を1,最高到達点を0とする

//                float easeDeg180Ratio = Easing.Linear(deg180Ratio, 1.0f, 0.0f, 1.0f);

//                nowAnglerVel = ((maxAnglerVel - minAnglerVel) * easeDeg180Ratio) + minAnglerVel;//角速度（量）

//                //前回計算後のAfterAngleを持ってくる
//                LastBtoP_Angle = AfterBtoP_Angle;

//                //↑を角速度分回す
//                //向きによって回転方向が違う
//                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//                {
//                    AfterBtoP_Angle = Quaternion.Euler(0, 0, nowAnglerVel * 1) * LastBtoP_Angle;

//                }
//                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//                {
//                    AfterBtoP_Angle = Quaternion.Euler(0, 0, nowAnglerVel * -1) * LastBtoP_Angle;

//                }

//                //ボール座標 ＋ 正規化した回転後アングル ＊ 長さ
//                Vector3 pos = BulletPosition + (AfterBtoP_Angle.normalized) * betweenLength;

//                PlayerScript.transform.position = pos;
//                break;

//            case SwingState.RELEASED:
//                //自分へ弾を引き戻す
//                float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
//                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
//                vec = vec.normalized;
//                BulletScript.vel = vec * 100;
//                //距離が一定以下になったら弾を非アクティブ
//                if (interval < 4.0f)
//                {
//                    finishFlag = true;

//                }
//                break;

//            case SwingState.HANGING:
//                //反転処理
//                if (countreButton)
//                {
//                    PlayerScript.swingState = SwingState.TOUCHED;
//                    CalculateCounterVariable();
//                    countreButton = false;
//                }

//                if (releaseButton)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    PlayerScript.swingState = SwingState.RELEASED;

//                    PlayerScript.vel = Vector3.zero;
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
//            PlayerScript.mode = new PlayerStateMidair(true);
//        }
//    }

//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Swing");
//    }
//}


///// <summary>
///// 弾が天井でスイングしている状態
///// 移動速度等を保存
///// </summary>
//public class PlayerStateSwing_2 : PlayerState
//{
//    private bool finishFlag;
//    private bool releaseButton;
//    private bool countreButton;
//    private Vector3 BulletPosition; //ボールの位置

//    private float betweenLength; //開始時二点間の距離(距離はスイングstate通して固定)
//    private Vector3 startPlayerVel;　　　　　　 //突入時velocity
//    private float startAngle;    //開始時の二点間アングル
//    private float endAngle;      //自動切り離しされる角度(start角度依存)
//    private float minAnglerVel;  //最低角速度（自動切り離し地点にいる時）
//    private float maxAnglerVel;　//最高角速度 (真下にプレイヤーが居る時）
//    private float nowAnglerVel;  //現在角速度

//    Vector3 LastBtoP_Angle;  //最後に計測したバレット→プレイヤーの正規化Vector
//    Vector3 AfterBtoP_Angle; //角速度計算後のバレット→プレイヤーの正規化Vector


//    const float SWING_END_RATIO = 1.4f;

//    public PlayerStateSwing_2()  //コンストラクタ
//    {
//        BulletPosition = BulletScript.gameObject.transform.position;

//        //計算用情報格納
//        startPlayerVel = BulletScript.vel;
//        betweenLength = Vector3.Distance(Player.transform.position, BulletPosition);
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);
//        startAngle = endAngle = degree;

//        PlayerScript.refState = EnumPlayerState.SWING;
//        PlayerScript.swingState = SwingState.TOUCHED;
//        PlayerScript.canShotState = false;
//        PlayerScript.endSwing = false;
//        PlayerScript.hangingSwing = false;
//        finishFlag = false;
//        releaseButton = false;
//        countreButton = false;
//        BulletScript.rb.isKinematic = true;
//        PlayerScript.rb.velocity = Vector3.zero;
//        PlayerScript.vel = Vector3.zero;

//        CalculateStartVariable();
//    }


//    /// <summary>
//    /// 振り子制御用の各種変数を計算
//    /// </summary>
//    public void CalculateStartVariable()
//    {

//        //紐の長さとスピードから角速度を計算
//        float angler_velocity;
//        float tempY = Mathf.Min(startPlayerVel.y, 0.0f);
//        angler_velocity = (Mathf.Abs(startPlayerVel.x) * 2.5f + Mathf.Abs(tempY) * 1.5f);
//        angler_velocity /= (betweenLength * 2.0f * Mathf.PI);

//        //範囲内に補正
//        angler_velocity = Mathf.Clamp(angler_velocity, 1.0f, 3.0f);

//        nowAnglerVel = maxAnglerVel = minAnglerVel = angler_velocity;

//        Debug.Log("AnglerVelocity: " + angler_velocity);

//        //バレットからプレイヤーのアングルを保存
//        LastBtoP_Angle = AfterBtoP_Angle = (Player.transform.position - BulletPosition).normalized;

//        //切り離しアングルの計算
//        float diff_down = Mathf.Abs(startAngle - 180.0f); //真下と突入角の差
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle -= (diff_down + diff_down);
//            //開始点よりは高い位置にする
//            endAngle -= 30;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 90, 140);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle += (diff_down + diff_down);
//            //開始点よりは高い位置にする
//            endAngle += 30;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 220, 270);
//        }

//        //最低速は突入時プレイヤーvelocity
//        maxAnglerVel = minAnglerVel = angler_velocity;
//        //最高速度は突入角が大きいほど早い
//        float velDiff = Mathf.Clamp01((diff_down / 90));
//        maxAnglerVel += velDiff * 1.2f;
//    }

//    /// <summary>
//    /// 壁跳ね返り時の各種計算
//    /// </summary>
//    public void CalculateCounterVariable()
//    {
//        Debug.Log("counter:");

//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //プレイヤー回転処理
//            PlayerScript.dir = PlayerMoveDir.LEFT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, -90, 0);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //プレイヤー回転処理
//            PlayerScript.dir = PlayerMoveDir.RIGHT;
//            PlayerScript.rb.rotation = Quaternion.Euler(0, 90, 0);
//        }

//        //切り離しアングルの計算
//        ReleaseAngleCalculate();
//    }

//    private void ReleaseAngleCalculate()
//    {
//        float diff_down = Mathf.Abs(endAngle - 180.0f); //真下と終了角の差
//        if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle -= (diff_down + diff_down);
//            //開始点よりは高い位置にする
//            endAngle -= 10;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 90, 140);
//        }
//        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//        {
//            //終点自動切り離しポイントをy軸に対して対称にする
//            endAngle += (diff_down + diff_down);
//            //開始点より高い位置にする
//            endAngle += 10;

//            //範囲内に補正
//            endAngle = Mathf.Clamp(endAngle, 220, 270);
//        }
//    }

//    public void RotationPlayer()
//    {

//        switch (PlayerScript.swingState)
//        {
//            case SwingState.TOUCHED:
//                float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);

//                Vector3 vecToPlayer = BulletScript.rb.position - PlayerScript.rb.position;
//                Quaternion quaternion = Quaternion.LookRotation(vecToPlayer);

//                Quaternion adjustQua = Quaternion.Euler(90, 0, 0); //補正用クオータニオン

//                quaternion *= adjustQua;

//                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//                {
//                    if (degree < 180)
//                    {
//                        quaternion *= Quaternion.Euler(0, 180, 0);
//                    }
//                }
//                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//                {
//                    if (degree > 180)
//                    {
//                        quaternion *= Quaternion.Euler(0, 180, 0);
//                    }
//                }

//                PlayerScript.rb.rotation = quaternion;
//                break;

//            case SwingState.RELEASED:
//                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//                {
//                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, 90, 0));
//                }
//                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//                {
//                    PlayerScript.rb.MoveRotation(Quaternion.Euler(0, -90, 0));
//                }
//                break;
//        }


//    }

//    public void InputButton()
//    {

//        if (PlayerScript.ReleaseMode)
//        {
//            if (PlayerScript.swingState == SwingState.TOUCHED)
//            {
//                if (Input.GetButtonUp("Button_R")) //ボタンを押して話したら
//                {
//                    releaseButton = true;
//                }
//            }
//            else if (PlayerScript.swingState == SwingState.HANGING)
//            {
//                if (Input.GetButtonUp("Button_R")) //ボタンを押して話したら
//                {
//                    releaseButton = true;
//                }
//                else if (Input.GetButtonDown("Jump"))
//                {
//                    countreButton = true;
//                    Debug.Log("Press Jump");
//                }
//            }
//        }
//        else
//        {
//            if (PlayerScript.swingState == SwingState.TOUCHED)
//            {
//                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
//                {
//                    releaseButton = true;
//                }
//            }
//            else if (PlayerScript.swingState == SwingState.HANGING)
//            {
//                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
//                {
//                    releaseButton = true;
//                }
//                else if (Input.GetButtonDown("Jump"))
//                {
//                    countreButton = true;
//                    Debug.Log("Press Jump");
//                }
//            }

//        }

//    }





//    public override void UpdateState()
//    {
//        //切り離し入力
//        InputButton();

//        //弾の場所更新
//        BulletPosition = BulletScript.rb.position;

//        //ボールプレイヤー間の角度を求める
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);

//        //切り離し
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
//                if ((degree < endAngle) || (releaseButton == true))
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    PlayerScript.swingState = SwingState.RELEASED;

//                    //勢い追加
//                    //弾とプレイヤー間のベクトルに直行するベクトル
//                    Vector3 addVec = BulletPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, -90) * addVec;


//                    float mutipleVec = (nowAnglerVel / 20.0f) + 1.0f;

//                    PlayerScript.vel += addVec * PlayerScript.RELEASE_FORCE;
//                }
//            }
//            else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//            {
//                if ((degree > endAngle) || (releaseButton == true))
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    PlayerScript.swingState = SwingState.RELEASED;

//                    //勢い追加
//                    //弾とプレイヤー間のベクトルに直行するベクトル
//                    Vector3 addVec = BulletPosition - Player.transform.position;
//                    addVec = addVec.normalized;
//                    addVec = Quaternion.Euler(0, 0, 90) * addVec;


//                    float mutipleVec = (nowAnglerVel / 20.0f) + 1.0f;

//                    PlayerScript.vel += addVec * PlayerScript.RELEASE_FORCE;
//                }
//            }
//        }


//    }

//    public override void Move()
//    {
//        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position); //バレットからプレイヤーベクトル
//        float deg180dif = Mathf.Abs(degree - 180); //プレイヤーからベクトル

//        RotationPlayer();

//        switch (PlayerScript.swingState)
//        {
//            case SwingState.TOUCHED:
//                //ぶら下がり処理
//                if (PlayerScript.hangingSwing)
//                {
//                    PlayerScript.swingState = SwingState.HANGING;
//                    PlayerScript.hangingSwing = false;
//                }

//                ////短くする処理
//                //if (PlayerScript.shortSwing.isShort)
//                //{
//                //    betweenLength = PlayerScript.shortSwing.length;
//                //    PlayerScript.shortSwing.isShort = false;
//                //}

//                //角速度計算
//                float deg180Ratio = deg180dif / Mathf.Abs(endAngle - 180); //真下と最高到達点の比率
//                deg180Ratio = Mathf.Clamp01(deg180Ratio); //一応範囲内に補正
//                deg180Ratio = 1 - deg180Ratio; //真下を1,最高到達点を0とする

//                float easeDeg180Ratio = Easing.Linear(deg180Ratio, 1.0f, 0.0f, 1.0f);

//                nowAnglerVel = ((maxAnglerVel - minAnglerVel) * easeDeg180Ratio) + minAnglerVel;//角速度（量）

//                //前回計算後のAfterAngleを持ってくる
//                LastBtoP_Angle = AfterBtoP_Angle;

//                //↑を角速度分回す
//                //向きによって回転方向が違う
//                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
//                {
//                    AfterBtoP_Angle = Quaternion.Euler(0, 0, nowAnglerVel * 1) * LastBtoP_Angle;

//                }
//                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
//                {
//                    AfterBtoP_Angle = Quaternion.Euler(0, 0, nowAnglerVel * -1) * LastBtoP_Angle;

//                }

//                //ボール座標 ＋ 正規化した回転後アングル ＊ 長さ
//                Vector3 pos = BulletPosition + (AfterBtoP_Angle.normalized) * betweenLength;

//                PlayerScript.transform.position = pos;
//                break;

//            case SwingState.RELEASED:
//                //自分へ弾を引き戻す
//                float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
//                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
//                vec = vec.normalized;
//                BulletScript.vel = vec * 100;
//                //距離が一定以下になったら弾を非アクティブ
//                if (interval < 4.0f)
//                {
//                    finishFlag = true;

//                }
//                break;

//            case SwingState.HANGING:
//                //反転処理
//                if (countreButton)
//                {

//                    PlayerScript.swingState = SwingState.TOUCHED;
//                    CalculateCounterVariable();
//                    countreButton = false;
//                }

//                if (releaseButton)
//                {
//                    PlayerScript.useVelocity = true;
//                    BulletScript.ReturnBullet();
//                    PlayerScript.swingState = SwingState.RELEASED;

//                    PlayerScript.vel = Vector3.zero;
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
//            PlayerScript.mode = new PlayerStateMidair(true);
//        }
//    }

//    public override void DebugMessage()
//    {
//        Debug.Log("PlayerState:Swing");
//    }
//}


public class PlayerStateSwing_Vel : PlayerState
{
    private bool finishFlag;
    private bool releaseButton;
    private bool countreButton;
    private Vector3 BulletPosition; //ボールの位置

    private float betweenLength; //開始時二点間の距離(距離はスイングstate通して固定)
    private Vector3 startPlayerVel;　　　　　　 //突入時velocity
    private float startAngle;    //開始時の二点間アングル
    private float endAngle;      //自動切り離しされる角度(start角度依存)
    private float minAnglerVel;  //最低角速度（自動切り離し地点にいる時）
    private float maxAnglerVel;　//最高角速度 (真下にプレイヤーが居る時）
    private float nowAnglerVel;  //現在角速度
    private bool startVelDownFlag; //velocityを減速させるフラグ（真下に到達したら

    Vector3 LastBtoP_Angle;  //最後に計測したバレット→プレイヤーの正規化Vector
    Vector3 AfterBtoP_Angle; //角速度計算後のバレット→プレイヤーの正規化Vector

    private PlayerMoveDir firstDir; 
    const float SWING_END_RATIO = 1.4f;

    public PlayerStateSwing_Vel()  //コンストラクタ
    {
        BulletPosition = BulletScript.gameObject.transform.position;

        //計算用情報格納
        startPlayerVel = BulletScript.vel;
        firstDir = PlayerScript.dir;
        betweenLength = Vector3.Distance(Player.transform.position, BulletPosition);
        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);
        startAngle = endAngle = degree;
        startVelDownFlag = false;

        PlayerScript.refState = EnumPlayerState.SWING;
        PlayerScript.swingState = SwingState.TOUCHED;
        PlayerScript.canShotState = false;
        PlayerScript.endSwing = false;
        PlayerScript.hangingSwing = false;
        finishFlag = false;
        releaseButton = false;
        countreButton = false;
        BulletScript.rb.isKinematic = true;

        PlayerScript.useVelocity = true;

        PlayerScript.animator.SetBool("isSwing", true);

        CalculateStartVariable();

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

        //バレットからプレイヤーのアングルを保存
        LastBtoP_Angle = AfterBtoP_Angle = (Player.transform.position - BulletPosition).normalized;

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
        LastBtoP_Angle = AfterBtoP_Angle = (Player.transform.position - BulletPosition).normalized;

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
            endAngle = Mathf.Clamp(endAngle, 90, 140);
        }
        else if (PlayerScript.dir == PlayerMoveDir.LEFT)
        {
            //終点自動切り離しポイントをy軸に対して対称にする
            endAngle += (diff_down + diff_down);
            //開始点より高い位置にする
            endAngle += 10;

            //範囲内に補正
            endAngle = Mathf.Clamp(endAngle, 220, 270);
        }

        if (PlayerScript.AutoRelease == false)
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
                if (Input.GetButtonUp("Button_R")) //ボタンを押して話したら
                {
                    releaseButton = true;
                }
            }
            else if (PlayerScript.swingState == SwingState.HANGING)
            {
                if (Input.GetButtonUp("Button_R")) //ボタンを押して話したら
                {
                    releaseButton = true;
                }
            }
        }
        else
        {
            if (PlayerScript.swingState == SwingState.TOUCHED)
            {
                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
                {
                    releaseButton = true;
                }
            }
            else if (PlayerScript.swingState == SwingState.HANGING)
            {
                if (Input.GetButton("Button_R") == false) //ボタンが離れていたら
                {
                    releaseButton = true;
                }
            }

        }
    }

    public override void UpdateState()
    {
        //切り離し入力
        InputButton();

        //弾の場所更新
        BulletPosition = BulletScript.rb.position;

        //ボールプレイヤー間の角度を求める
        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position);

        //切り離し
        if (PlayerScript.swingState == SwingState.TOUCHED)
        {
            if (PlayerScript.endSwing)
            {
                PlayerScript.endSwing = false;
                PlayerScript.useVelocity = true;
                BulletScript.ReturnBullet();
                PlayerScript.swingState = SwingState.RELEASED;
            }

            if (releaseButton == true)
            {

                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
               
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;

                    //勢い追加(速度が低すぎる場合に一定以上に補正
                    if (PlayerScript.vel.magnitude < 50.0f)
                    {
                        Vector3 addVec = BulletPosition - Player.transform.position;
                        addVec = addVec.normalized;
                        addVec = Quaternion.Euler(0, 0, -90) * addVec;
                        PlayerScript.vel = addVec * 50.0f;
                    }
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {              
                    PlayerScript.useVelocity = true;
                    BulletScript.ReturnBullet();
                    PlayerScript.swingState = SwingState.RELEASED;


                    //勢い追加(速度が低すぎる場合に一定以上に補正
                    if (PlayerScript.vel.magnitude < 50.0f)
                    {
                        Vector3 addVec = BulletPosition - Player.transform.position;
                        addVec = addVec.normalized;
                        addVec = Quaternion.Euler(0, 0, 90) * addVec;
                        PlayerScript.vel = addVec * 50.0f;
                    }
                }
            }
        }
    }

    public override void Move()
    {
        float degree = CalculationScript.TwoPointAngle360(BulletPosition, Player.transform.position); //バレットからプレイヤーベクトル
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
            else if (PlayerScript.dir == PlayerMoveDir.RIGHT)
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
                    }
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    if (endAngle < degree)
                    {
                        CalculateCounterVariable();
                    }
                }

                //壁跳ね返り処理
                if (PlayerScript.hangingSwing)
                {
                    PlayerScript.animator.SetTrigger("wallKick");
                    CalculateCounterVariable();
                    PlayerScript.hangingSwing = false;
                }

                //長くする処理
                //if (PlayerScript.LongRope)
                //{
                //    if (betweenLength < BulletScript.BULLET_ROPE_LENGTH)
                //    {
                //        betweenLength += 0.2f;
                //        minAnglerVel *= 0.99f;
                //        maxAnglerVel *= 0.99f;
                //    }
                //}


                //速度計算
                float deg180Ratio = deg180dif / 90;       //真下と真横の比率
                deg180Ratio = Mathf.Clamp01(deg180Ratio); //角度が９０度以上ある場合でも補正
                deg180Ratio = 1 - deg180Ratio; //真下を1,最高到達点を0とする

                Debug.Log(deg180Ratio);

                float easeRatio = Easing.EasingTypeFloat(EASING_TYPE.SINE_OUT, deg180Ratio, 1.0f, 0.0f, 1.0f);

            
                //前回計算後のAfterAngleを持ってくる
                LastBtoP_Angle = AfterBtoP_Angle;


                //向きによって回転方向が違う

                if (PlayerScript.dir == PlayerMoveDir.RIGHT)
                {
                    Vector3 tempVec = BulletPosition - PlayerScript.rb.position;
                    tempVec = tempVec.normalized;
                    tempVec = Quaternion.Euler(0, 0, -90) * tempVec;

                    if (startVelDownFlag)
                    {
                        PlayerScript.vel = tempVec * easeRatio * 70.0f + (tempVec * 5.0f);
                    }
                    else
                    {
                        PlayerScript.vel = tempVec * 50.0f;
                    }
                }
                else if (PlayerScript.dir == PlayerMoveDir.LEFT)
                {
                    Vector3 tempVec = BulletPosition - PlayerScript.rb.position;
                    tempVec = tempVec.normalized;
                    tempVec = Quaternion.Euler(0, 0, 90) * tempVec;

                    if (startVelDownFlag)
                    {
                        PlayerScript.vel = tempVec * easeRatio * 70.0f + (tempVec * 5.0f);
                    }
                    else
                    {
                        PlayerScript.vel = tempVec * 50.0f;
                    }
                }

                //振り子の最大長になっていない場合には円外に向かうベクトルを与える
                if(Vector3.Distance(PlayerScript.rb.position, BulletScript.rb.position) < BulletScript.BULLET_ROPE_LENGTH - 0.01f)
                {
                    Vector3 longerVec = PlayerScript.rb.position - BulletScript.rb.position;
                    longerVec = longerVec.normalized;

                    float outForce = 20.0f;
                    longerVec *= outForce;

                    PlayerScript.vel += longerVec;
                }

                //振り子の最大長になっていない場合には円外に向かうようベクトルの角度を変える


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

                //振り子よりも長くなってしまった場合は補正
                if (Vector3.Distance(PlayerScript.rb.position, BulletScript.rb.position) > BulletScript.BULLET_ROPE_LENGTH)
                {
                    float ang = CalculationScript.UnityTwoPointAngle360(BulletScript.rb.position, PlayerScript.rb.position);
                    Vector3 adjustPos = BulletScript.rb.position;
                    adjustPos.x += Mathf.Cos(ang * Mathf.Deg2Rad) * BulletScript.BULLET_ROPE_LENGTH;
                    adjustPos.y += Mathf.Sin(ang * Mathf.Deg2Rad) * BulletScript.BULLET_ROPE_LENGTH;
                    PlayerScript.rb.position = adjustPos;
                }

                break;

            case SwingState.RELEASED:
                //自分へ弾を引き戻す
                float interval = Vector3.Distance(PlayerScript.transform.position, BulletScript.transform.position);
                Vector3 vec = PlayerScript.rb.position - BulletScript.rb.position;
                vec = vec.normalized;
                BulletScript.vel = vec * 100;
                //距離が一定以下になったら弾を非アクティブ
                if (interval < 4.0f)
                {
                    finishFlag = true;

                }
                break;

            case SwingState.HANGING:
                //反転処理
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
            PlayerScript.animator.SetBool("isSwing", false);
            PlayerScript.swingState = SwingState.NONE;
            PlayerScript.mode = new PlayerStateMidair(true);
        }
    }

    public override void DebugMessage()
    {
        Debug.Log("PlayerState:Swing");
    }
}