using UnityEngine;

public enum GO_POS_X
{
    LEFT,
    RIGHT,
    STAY
}

public enum GO_POS_Y
{
    UP,
    DOWN,
    STAY
}



public class CameraCenterPos : MonoBehaviour
{
    //[Header("プレイヤーとカメラの距離はMainCamera参照")]


    Vector3 PlayerPos = Vector3.zero;
    Vector3 OldPlayerPos = Vector3.zero;

    //[Header("カメラ距離調整")]
    //[Range(-10, -60), SerializeField, Tooltip("カメラ距離調整")] private float cameraDistanceZ ;//Zの距離  

    [Header("チェックが入っていたらY固定")]
    [SerializeField, Tooltip("チェックが入っていたらY固定")] private bool FreezeY;        //これにチェックが入っていたら分割

    [Header("”Y固定時のみ”この値でカメラ高さ調整     プラスでカメラが上へ")]
    [Range(-10, 20), SerializeField, Tooltip("”Y固定時のみ”この値でカメラ高さ調整 \nプラスでカメラが上へ")] private float cameraDistanceY;   //スティック方向を補正する（要素数で分割）値は上が0で時計回りに増加。0~360の範囲

    [Header("描画の有無")]
    [SerializeField, Tooltip("描画の有無")] private bool isDraw;   //スティック方向を補正する（要素数で分割）値は上が0で時計回りに増加。0~360の範囲


    //卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍
    //横移動
    //卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍
    float RightSavePos = 0;
    float LeftSavePos = 0;

    float DifferenceX = 0.0f;

    [Header("移動速度倍率 横")]
    [Range(0.1f, 3.0f), SerializeField, Tooltip("移動速度倍率 横")] private float SpeedMultiX = 0.5f;

    //10
    [Header("焦点とプレイヤーの距離上限 横")]
    [Range(1,15), SerializeField, Tooltip("焦点とプレイヤーの距離上限 横")] private float MAX_DIFFERENCE_X = 10;


    [Header("1F当たりの移動距離上限 横")]
    [Range(0.1f, 3.0f), SerializeField, Tooltip("1F当たりの移動距離上限 横")] private float MAX_CAMERA_MOVE_X = 0.6f;

    //方向転換して進んだ距離
    float ReturnMove_X = 0;

    //卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍
    //縦移動
    //卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍卍
    GO_POS_Y GoPosY = GO_POS_Y.DOWN;

    float DifferenceY = 0.0f;

    float SpeedMultiY = 0.5f;

    float UP_CAMEPA_SPEED = 0.08f;

    float UP_DIFFERENCE_Y = 10;
    float DOWN_DIFERENCE_Y = 0;



    


    void Start()
    {
        DifferenceX = MAX_DIFFERENCE_X;
        DifferenceY = UP_DIFFERENCE_Y;

       
    }


    void Update()
    {
        if (!isDraw)
        {
            GetComponent<Renderer>().material.color = Color.clear;
        }
        else
        {
            GetComponent<Renderer>().material.color = new Color(1,0,0,0.5f);
        }


#if true
        //分岐
        if (!FreezeY)
        {
            transform.position = PlayerMain.instance.transform.position + new Vector3(DifferenceX, DifferenceY, 0);

        }
        else
        {
            transform.position = PlayerMain.instance.transform.position + new Vector3(DifferenceX, cameraDistanceY, 0);

        }
#else
        //Yに対してカメラに合わせるよう書き換え

        float PosY = GameObject.Find("Main Camera").transform.position.y;

        if (!FreezeY)
        {
            transform.position = PlayerMain.instance.transform.position + new Vector3(DifferenceX, DifferenceY, 0);

            transform.position = new Vector3(transform.position.x, PosY, transform.position.z);

        }
        else
        {
            transform.position = PlayerMain.instance.transform.position + new Vector3(DifferenceX, cameraDistanceY, 0);

            transform.position = new Vector3(transform.position.x, PosY, transform.position.z);
        }
#endif

    }

    void FixedUpdate()
    {
        //プレイヤー座標
        PlayerPos = PlayerMain.instance.transform.position;

        FixedX();
        //分岐
        if(!FreezeY)
        {
            FixedY();
        }
        
        OldPlayerPos = PlayerMain.instance.transform.position;

    }

    void FixedX()
    {
        //進んだ量
        float TempVel = PlayerPos.x - OldPlayerPos.x;

        //カメラの移動量 
        float TempCameraMove = Mathf.Min(TempVel * SpeedMultiX, MAX_CAMERA_MOVE_X);
        TempCameraMove = Mathf.Max(TempVel * SpeedMultiX, -MAX_CAMERA_MOVE_X);

        if (TempVel > 0.1f)
        {
            //進行に応じて保存
            RightSavePos = PlayerMain.instance.transform.position.x;

            //一定以上折り返したら
            if (PlayerPos.x > LeftSavePos + ReturnMove_X)
            {
                DifferenceX = Mathf.Min(DifferenceX + TempCameraMove, MAX_DIFFERENCE_X);
            }
        }
        if (TempVel < -0.1f)
        {
            //進行に応じて保存
            LeftSavePos = PlayerMain.instance.transform.position.x;

            //一定以上折り返したら
            if (PlayerPos.x < RightSavePos - ReturnMove_X)
            {
                DifferenceX = Mathf.Max(DifferenceX + TempCameraMove, -MAX_DIFFERENCE_X);
            }
        }

    }

    void FixedY()
    {
        //進んだ量
        float TempVel = PlayerPos.y - OldPlayerPos.y;

        //カメラの移動量 
        float TempCameraMove = Mathf.Abs(TempVel) * SpeedMultiY;

#if false
        if (TempVel > 0.1f)
        {
            GoPosY = GO_POS_Y.DOWN;
        }
        else if(TempVel <= 0.0f)
        {
            GoPosY = GO_POS_Y.UP;
        }
        else
        {
            GoPosY = GO_POS_Y.STAY;
        }

        switch (GoPosY)
        {
            case GO_POS_Y.UP:
                DifferenceY = Mathf.Min(UP_DIFFERENCE_Y, DifferenceY + TempCameraMove);

                break;
            case GO_POS_Y.DOWN:
                DifferenceY = Mathf.Max(DOWN_DIFERENCE_Y, DifferenceY - TempCameraMove);
                break;
            default:
                break;
        }
#endif

        //上昇中下がって地に足付くと上がる

        //上昇中
        if (TempVel > 0.1f)
        {
            GoPosY = GO_POS_Y.DOWN;
        }
        //地上
        else if (PlayerMain.instance.isOnGround)
        {
            GoPosY = GO_POS_Y.UP;
        }
        else
        {
            GoPosY = GO_POS_Y.STAY;
        }

        switch (GoPosY)
        {
            case GO_POS_Y.UP:
                DifferenceY = Mathf.Min(UP_DIFFERENCE_Y, DifferenceY + UP_CAMEPA_SPEED);

                break;
            case GO_POS_Y.DOWN:
                DifferenceY = Mathf.Max(DOWN_DIFERENCE_Y, DifferenceY - TempCameraMove);
                break;
            default:
                break;
        }

    }


    public void ManualUpdate()
    {
        Update();
    }



#if false

    GO_POS_X GoPos = GO_POS_X.STAY;
    float Difference = 0.0f;
    
    float MAX_DIFFERENCE = 10;

    float CameraMoveSpeed = 0.0f;
    float MAX_SPEED = 0.2f;
    float ADD_SPEED = 0.01f;

    int RightCnt = 0;
    int LeftCnt = 0;

    Vector3 OldPlayerPos = Vector3.zero;

    //targetオブジェクトデータ
    private GameObject targetObj;

    //target位置データ
    private Vector3 targetPos;

    public float cameraPosY = 5.3f;
    public float cameraDistanceZ = -30;//Zの距離

    void Start()
    {
        //targetオブジェクトを取得
        targetObj = GameObject.Find("PlayerMain");
        targetPos = targetObj.transform.position;
        
    }


    void Update()
    {

       

        //プレイヤーに追従するだけ
        targetPos = targetObj.transform.position;
        transform.position = targetObj.transform.position + new Vector3(Difference,10,cameraDistanceZ);

    }

    void FixedUpdate()
    {
        CheckGoPos();
        CheckDifference();

        OldPlayerPos = PlayerMain.instance.transform.position;
    }



    public void ManualUpdate()
    {
        Update();
    }

    void CheckGoPos()
    {
        Vector2 Stick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Stick.x > 0.2f)
        {
            LeftCnt = 0;
            RightCnt++;

            if(RightCnt < 10)
            {
                GoPos = GO_POS_X.STAY;
            }
            else
            {
                GoPos = GO_POS_X.RIGHT;
            }
        }
        else if (Stick.x < -0.2f)
        {
            RightCnt = 0;
            LeftCnt++;

            if (LeftCnt < 10)
            {
                GoPos = GO_POS_X.STAY;
            }
            else
            {
                GoPos = GO_POS_X.LEFT;
            }
        }
        else
        {
            //LeftCnt = 0;
            //RightCnt = 0;
        }

    }

    void CheckDifference ()
    {
        switch (GoPos)
        {
            

            case GO_POS_X.RIGHT:
                CameraMoveSpeed = Mathf.Min(CameraMoveSpeed + ADD_SPEED, MAX_SPEED);
                Difference = Mathf.Min(Difference + CameraMoveSpeed, MAX_DIFFERENCE);
                break;

            case GO_POS_X.LEFT:
                CameraMoveSpeed = Mathf.Min(CameraMoveSpeed + ADD_SPEED, MAX_SPEED);
                Difference = Mathf.Max(Difference - CameraMoveSpeed, -MAX_DIFFERENCE);
                break;

            case GO_POS_X.STAY:
                CameraMoveSpeed = 0.0f;
                break;
        }

        if(Difference == MAX_DIFFERENCE || Difference == -MAX_DIFFERENCE)
        {
            CameraMoveSpeed = 0.0f;
        }

    }

#endif

}


